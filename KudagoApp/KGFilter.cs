using System;


namespace Poster
{
    /// <summary>
    /// Фильтр для поиска вариантов с Kudago
    /// </summary>
    class KGFilter
    {

        /// <summary>
        /// Категория
        /// </summary>
        public KGCategory Category { get; private set; }

        /// <summary>
        /// Дата варианта
        /// </summary>
        public DateTime Date { get; private set; }

        /// <summary>
        /// Максимальная цена
        /// </summary>
        public decimal Price { get; private set; }

        /// <summary>
        /// Допустимый рейтинг
        /// </summary>
        public KGItem.ORating Rating { get; private set; }

        /// <summary>
        /// Радиус поиска (в км)
        /// </summary>
        public int Radius { get; private set; }

        /// <summary>
        /// Основной конструктор
        /// </summary>
        /// <param name="cat">Тип варианта</param>
        /// <param name="date">Дата варианта</param>
        /// <param name="price">Максимальная цена</param>
        /// <param name="rating">Допустимый рейтинг</param>
        /// <param name="rad">Радиус поиска (в км.)</param>
        public KGFilter(KGCategory cat, DateTime date, decimal price, KGItem.ORating rating, int rad)
        {
            Category = cat;
            Date = date;
            Price = price;
            Rating = rating;
            Radius = rad;
        }
    }
}
