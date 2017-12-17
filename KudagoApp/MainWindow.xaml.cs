using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Input;
namespace Poster
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private BackgroundWorker filterItemsGetter; // Воркер для получения фильтра
        private BackgroundWorker itemsGetter;       // Воркер для получения вариантов
        List<KGCategory> aggr;                      // Агрегатор

        /// <summary>
        /// Текущие элементы в таблицы событий и мест
        /// </summary>
        public List<KGItem> items = new List<KGItem>();


        /// <summary>
        /// Основной рандомизатор.
        /// </summary>
        public Random Randomizer = new Random();

        /// <summary>
        /// Текущие элементы в таблице избранного
        /// </summary>
        [Serializable]
        static class Favorite
        {
            public static List<KGItem> Items = new List<KGItem>();
        }

        /// <summary>
        /// Инициализация главного окна
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            filterItemsGetter = ((BackgroundWorker)this.FindResource("filterItemsGetter"));
            filterItemsGetter.RunWorkerAsync();

            itemsGetter = ((BackgroundWorker)this.FindResource("itemsGetter"));

        }

        /// <summary>
        /// Получает данные для фильтра
        /// </summary>
        private void filterItemsGetter_DoWork(object sender, DoWorkEventArgs e)
        {

            // Подцепим типы через API
            List<KGCategory> eventTypes = KGAgregator.GetEventTypes();
            List<KGCategory> placeTypes = KGAgregator.GetPlaceTypes();

            // Агрегируем типы в единый список
            aggr = new List<KGCategory>();
            aggr.Add(new KGCategory("Любое событие", KGCategory.CType.Event));
            aggr.Add(new KGCategory("Любое место", KGCategory.CType.Place));
            aggr.AddRange(eventTypes);
            aggr.AddRange(placeTypes);
        }

        /// <summary>
        /// Прокидывает данные для фильтра
        /// </summary>
        private void filterItemsGetter_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            optionType.Items.Clear();
            optionType.ItemsSource = aggr;
            optionType.Items.Refresh();
            optionType.SelectedIndex = 0;
        }


        /// <summary>
        /// Получает список вариантов
        /// </summary>
        private void itemsGetter_DoWork(object sender, DoWorkEventArgs e)
        {
            var obj = (Tuple<KGCategory, DateTime, decimal, KGItem.ORating, int>)e.Argument;
            KGFilter filter = new KGFilter(obj.Item1,
                obj.Item2,
                obj.Item3,
                obj.Item4,
                obj.Item5);
            items = KGAgregator.GetOptions(filter);
        }

        /// <summary>
        /// Прокидывает список вариантов
        /// </summary>
        private void itemsGetter_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            eventsList.ItemsSource = items;
            eventsList.Items.Refresh();
            filterApply_Button.Content = "Применить";
            filterApply_Button.IsEnabled = true;
        }



        /// <summary>
        /// Клик по кнопке применения фильтра для поиска
        /// </summary>
        private void filterApply_Button_Click(object sender, RoutedEventArgs e)
        {
            while (itemsGetter.IsBusy) { }

            filterApply_Button.Content = "...";
            filterApply_Button.IsEnabled = false;

            KGCategory type = (KGCategory)optionType.SelectedItem;
            decimal price = (decimal)eventPrice.Value;
            KGItem.ORating rating = KGItem.Str2Rating(eventRating.SelectionBoxItem.ToString());
            int radius = (int)eventRadius.Value;
            var date = eventDate.SelectedDate.Value.Date;
            itemsGetter.RunWorkerAsync(new Tuple<KGCategory, DateTime, decimal, KGItem.ORating, int>
                (type, date, price, rating, radius));
        }

        /// <summary>
        /// Событие загрузки основной таблицы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventsList_Loaded(object sender, RoutedEventArgs e)
        {
            eventsList.ItemsSource = items;
        }

        private void eventType_Loaded(object sender, RoutedEventArgs e)
        {
            // Пустая имплементация.
        }

        /// <summary>
        /// Сохраняет список избранного в файл.
        /// </summary>
        void FavoriteSerialize()
        {
            FileStream fs = new FileStream("favorite.list", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, Favorite.Items);
            fs.Close();
        }

        /// <summary>
        /// Открывает сохраненный список избранного из файла.
        /// </summary>
        void FavoriteDeserialize()
        {
            if (File.Exists("favorite.list"))
            {
                try
                {
                    FileStream fs = new FileStream("favorite.list", FileMode.Open, FileAccess.Read, FileShare.Read);
                    if (fs.Length != 0)
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        Favorite.Items = (List<KGItem>)bf.Deserialize(fs);
                        fs.Close();
                        favoriteList.ItemsSource = Favorite.Items;
                        favoriteList.Items.Refresh();
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// После загрузки основной формы
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            FavoriteDeserialize();
            // Дата по умолчанию - сегодня.
            eventDate.SelectedDate = DateTime.Now.Date;
        }


        private void eventsList_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Пустая имплементация.
        }

        /// <summary>
        /// Двойной клик по строкам в основной таблице.
        /// </summary>
        private void eventsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var a = (KGItem)eventsList.SelectedItem;
            if (a != null)
            {
                Comments cmt = new Comments(a);
                cmt.Owner = this;
                cmt.Show();
            }
        }

        /// <summary>
        /// Удаление из избранного / клик.
        /// </summary>
        private void DeleteFavorite_Click(object sender, RoutedEventArgs e)
        {
            var a = (KGItem)eventsList.SelectedItem;
            if (a != null)
            {
                Favorite.Items.Remove(a);
                favoriteList.Items.Refresh();
                MessageBox.Show(string.Format("[{0}] удален из избранного.", a.Type.Name));
            }
        }

        /// <summary>
        /// Добалавление в избраннное / клик.
        /// </summary>
        private void EventToFavorite_Click(object sender, RoutedEventArgs e)
        {
            var a = (KGItem)eventsList.SelectedItem;
            if (a != null)
            {
                Favorite.Items.Add(a);
                favoriteList.ItemsSource = Favorite.Items;
                favoriteList.Items.Refresh();
                MessageBox.Show(string.Format("[{0}] добавлено в избранное!", a.Type.Name));
            }
        }

        /// <summary>
        /// Клик по кнопке очищения избранного
        /// </summary>
        private void button_Click(object sender, RoutedEventArgs e)
        {
            Favorite.Items.Clear();
            favoriteList.ItemsSource = Favorite.Items;
            favoriteList.Items.Refresh();
            MessageBox.Show(string.Format("Список избранного очищен."));
        }

        /// <summary>
        /// После закрытия основной формы
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            FavoriteSerialize();
        }

        /// <summary>
        /// Кнопка для смены локации
        /// </summary>
        private void locationChange_Button_Click(object sender, RoutedEventArgs e)
        {
            ChangeLocation cl = new ChangeLocation(this);
            cl.Owner = this;
            cl.Show();
        }

        /// <summary>
        /// Обновляет отрисовку координат на главной форме
        /// </summary>
        public void LocRefresh()
        {
            location_Label.Content = KGAgregator.myLocation.ToString();
        }
    }
}
