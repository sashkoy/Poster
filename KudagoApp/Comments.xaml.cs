using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Windows;

namespace Poster
{
    /// <summary>
    /// Логика взаимодействия для Comments.xaml
    /// </summary>
    public partial class Comments : Window
    {
        KGItem item;
        public Comments(KGItem item)
        {
            InitializeComponent();
            this.item = item;
            this.Title = string.Format("Комментарии про [{0}]", item.Type.Name);
        }


        /// <summary>
        /// Получает комментарии об объекта и показывает их.
        /// </summary>
        public void ParseComments()
        {
            int id = item.Type.Type == KGCategory.CType.Event ? item.EventID : item.PlaceID;
            commentsBlock.Text = string.Format("Загрузка [{0}]...", id);
            string raw = KGAPI.Get(string.Format("https://kudago.com/public-api/v1.3/events/{0}/comments/?lang=&fields=user,text",
                id));
            var a = (dynamic)JsonConvert.DeserializeObject(raw);
            commentsBlock.Text = string.Empty;
            // А существуют ли комментарии?
            if (a.results != null && ((JArray)a.results).Count != 0)
            {
                // Выведем в pretty форме.
                foreach (var i in a.results)
                {
                    string name = (string)i.user.name;
                    string cm = (string)i.text;
                    commentsBlock.Text += string.Format("[{2}]{1}{3}{1}{0}{1}",
                        new string('_', 50),
                        Environment.NewLine,
                        name,
                        cm);
                }
            }
            else
            {
                this.Close();
                MessageBox.Show("Комментарии отсутствуют :(", this.Title);
            }
        }

        /// <summary>
        /// После загрузки формы.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ParseComments();
        }
    }
}
