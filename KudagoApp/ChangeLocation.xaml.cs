using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Management;
using Newtonsoft.Json;

namespace Poster
{
    /// <summary>
    /// Логика взаимодействия для ChangeLocation.xaml
    /// </summary>
    public partial class ChangeLocation : Window
    {
        MainWindow mw;

        /// <summary>
        /// Инициализация формы
        /// </summary>
        /// <param name="mw">Родительская форма</param>
        public ChangeLocation(MainWindow mw)
        {
            this.mw = mw;
            InitializeComponent();
        }

        /// <summary>
        /// После загрузки формы
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            posX.Value = KGAgregator.myLocation.X;
            posY.Value = KGAgregator.myLocation.Y;
        }

        /// <summary>
        /// Кнопка "применить" / клик
        /// </summary>
        private void apply_Click(object sender, RoutedEventArgs e)
        {
            KGAgregator.myLocation.X = posX.Value ?? KGAgregator.myLocation.X;
            KGAgregator.myLocation.Y = posY.Value ?? KGAgregator.myLocation.Y;
            this.Close();
            mw.LocRefresh();
        }


        /// <summary>
        /// Список действующих MAC адресов
        /// </summary>
        /// <returns></returns>
        public string[] GetMACAddresss()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration where IPEnabled=true");
            IEnumerable<ManagementObject> objects = searcher.Get().Cast<ManagementObject>();
            string[] mac = (from o in objects orderby o["IPConnectionMetric"] select o["MACAddress"].ToString()).ToArray();
            return mac;
        }


        /// <summary>
        /// Нахождение локации
        /// </summary>
        private void findLoc_Click(object sender, RoutedEventArgs e)
        {
            var macAddrs = GetMACAddresss(); // Получение MAC адресов.
            var adrs = new string[macAddrs.Length];
            // Синтетический JSON
            for (int i = 0; i < adrs.Length; ++i)
            {
                adrs[i] = "{ \"macAddress\": \"" + macAddrs[i].ToLower() + "\" }";
            }
            // Синтетический JSON в полном виде.
            string rq = "{\"wifiAccessPoints\": [" + string.Join(",", adrs) + "]}";
            var raw = KGAPI.Post("https://www.googleapis.com/geolocation/v1/geolocate?key=AIzaSyDT83j5TQTnpmnoiogxLNRUNT26As-fVoQ",
                rq);
            var na = (dynamic)JsonConvert.DeserializeObject(raw); // As Object
            posX.Value = (double)na.location.lat;
            posY.Value = (double)na.location.lng;
            MessageBox.Show("Координаты успешно найдены.");
        }
    }
}
