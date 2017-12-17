using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Globalization;

namespace Poster
{
    /// <summary>
    /// Получает данные с Kudago через API
    /// </summary>
    static class KGAgregator
    {

        /// <summary>
        /// Ваша локация
        /// </summary>
        public static KGLocation myLocation = new KGLocation(55.537704, 37.531595);

        /// <summary>
        /// Максимальное кол-во результатов выдачи
        /// </summary>
        const int RESULTS_MAX = 100;

        /// <summary>
        /// Типы событий
        /// </summary>
        public static List<KGCategory> GetEventTypes()
        {
            var result = new List<KGCategory>();
            string raw = KGAPI.Get("https://kudago.com/public-api/v1.2/event-categories/?lang=ru");
            foreach (var i in (dynamic)JsonConvert.DeserializeObject(raw))
            {
                result.Add(new KGCategory((string)i.name, KGCategory.CType.Event, (string)i.slug));
            }
            return result.OrderBy(x => x.Name).ToList();
        }

        /// <summary>
        /// Типы мест
        /// </summary>
        public static List<KGCategory> GetPlaceTypes()
        {
            var result = new List<KGCategory>();
            string raw = KGAPI.Get("https://kudago.com/public-api/v1.2/place-categories/?lang=ru");
            foreach (var i in (dynamic)JsonConvert.DeserializeObject(raw))
            {
                result.Add(new KGCategory((string)i.name, KGCategory.CType.Place, (string)i.slug));
            }
            return result.OrderBy(x => x.Name).ToList();
        }

        /// <summary>
        /// Получает список вариантов по заданному фильтру.
        /// </summary>
        /// <param name="filter">Фильтр для поиска</param>
        public static List<KGItem> GetOptions(KGFilter filter)
        {

            var results = new List<KGItem>();
            if (filter.Category.Type == KGCategory.CType.Event)
            {
                // Ищем событие...
                string raw = KGAPI.Get(
                    string.Format(
                        "https://kudago.com/public-api/v1.2/events/?actual_since={1}&actual_until={2}&fields=title,description,place,age_restriction,price,id&categories={0}&lat={3}&lon={4}&radius={5}&page_size=" + RESULTS_MAX,
                        filter.Category.Slug,
                        Utilites.Date2Unix(filter.Date),
                        Utilites.Date2Unix(filter.Date.AddHours(24)),
                        myLocation.X.ToString(new CultureInfo("en-US")),
                        myLocation.Y.ToString(new CultureInfo("en-US")),
                        filter.Radius * 1000));


                var a = (dynamic)JsonConvert.DeserializeObject(raw);
                foreach (var i in a.results)
                {
                    // Проверим цену мероприятия
                    bool ok = true;
                    decimal pre_price = 0;
                    decimal price = 0;
                    // Разобьем на части строку и найдем все числовые значения.
                    string[] price_raw = ((string)i.price).Split(' ');
                    // Попробуем получить максимальное числовое представление цены и сравнить с фильтром.
                    foreach (var s in price_raw)
                    {
                        if (decimal.TryParse(s, out pre_price))
                        {
                            if (pre_price > price)
                                price = pre_price;
                            if (price > filter.Price)
                            {
                                ok = false;
                                break;
                            }

                        }
                    }

                    // Иначе вход свободный. 
                    // Проверим возрастное ограничение мероприятия
                    string rat = (string)i.age_restriction ?? "0";
                    rat = rat.Replace("0", "0+"); // Для внутренней поддержки.
                    var ratI = KGItem.Str2Rating(rat);
                    // Проверяем возрастной рейтинг непосредственно (enum -> int compare)
                    if ((int)ratI > (int)filter.Rating)
                    {
                        ok = false;
                    }


                    //Проверяем.
                    if (ok)
                    {
                        KGItem t = new KGItem(new KGCategory(
                            (string)i.title,
                            KGCategory.CType.Event),
                            filter.Date.Date,
                            price,
                            ratI,
                            Utilites.StripHTML((string)i.description),
                            new KGLocation(0D, 0D));
                        // Запишем ID варианта чтобы в последствии найти координаты по возможности
                        t.PlaceID = (int)i.place.id;
                        // Чтобы в последствии подгрузить комментарии
                        t.EventID = (int)i.id;

                        results.Add(t);
                    }
                }
            }

            if (filter.Category.Type == KGCategory.CType.Place)
            {
                // Ищем место...
                // https://kudago.com/public-api/v1.3/places/21000/?lang=&fields=coords
                string raw = KGAPI.Get(
                    string.Format(
                        "https://kudago.com/public-api/v1.2/places/?fields=title,description,coords,id&categories={0}&lat={1}&lon={2}&radius={3}&page_size=" + RESULTS_MAX,
                        filter.Category.Slug,
                        myLocation.X.ToString(new CultureInfo("en-US")),
                        myLocation.Y.ToString(new CultureInfo("en-US")),
                        filter.Radius * 1000));

                var a = (dynamic)JsonConvert.DeserializeObject(raw);
                foreach (var i in a.results)
                {
                    KGItem t = new KGItem(new KGCategory(
                        (string)i.title,
                        KGCategory.CType.Place),
                        filter.Date.Date,
                        0M,
                        KGItem.ORating.R0,
                        Utilites.StripHTML((string)i.description),
                        new KGLocation((double)i.coords.lat, (double)i.coords.lon));
                    // Чтобы в последствии подгрузить комментарии
                    t.PlaceID = (int)i.id;
                    results.Add(t);
                }
            }
            if (filter.Category.Type == KGCategory.CType.Event)
            {
                // Попробуем отыскать координаты мест
                int[] ids = results.Select(x => x.PlaceID).ToArray();
                ids = ids.Distinct().ToArray();

                string query = string.Join(",", ids);
                string n = KGAPI.Get(string.Format("https://kudago.com/public-api/v1.3/places/?fields=id,coords&ids={0}",
                           query));
                var na = (dynamic)JsonConvert.DeserializeObject(n); // As Object
                Dictionary<int, KGLocation> id2loc = new Dictionary<int, KGLocation>();
                foreach (var tr in na.results)
                {
                    var crd = tr.coords; // As Coord Object
                    int id = (int)tr.id; // As Id Object
                    id2loc[id] = new KGLocation((double)crd.lat, (double)crd.lon);
                }
                results.ForEach(x =>
                {
                    if (id2loc.Keys.Contains(x.PlaceID))
                        x.Location = id2loc[x.PlaceID];
                });
            }
            return results;
        }
    }
}
