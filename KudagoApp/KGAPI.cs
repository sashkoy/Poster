using System.Text;
using System.Windows;
using xNet;

namespace Poster
{
    /// <summary>
    /// Сущность для отправки запросов к API
    /// </summary>
    static class KGAPI
    {
        /// <summary>
        /// Объект для произведения запросов к API
        /// </summary>
        static HttpRequest requester = new HttpRequest();

        /// <summary>
        /// Отправляет GET запрос к API
        /// </summary>
        /// <param name="url">URL</param>
        public static string Get(string url)
        {
            try
            {
                requester.Reconnect = true;
                requester.IgnoreProtocolErrors = true;
                requester.ReconnectDelay = 300;
                requester.ReconnectLimit = 3;
                requester.CharacterSet = Encoding.UTF8;
                string r = requester
                    .Get(url).ToString();
                requester.Close();
                return r;
            }
            catch
            {
                MessageBox.Show("Нет подключения к интернету!");
                return null;
            }
        }

        /// <summary>
        /// Отправляет Post запрос к API
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="rp">Параметры запроса</param>
        public static string Post(string url, string rp)
        {
            try
            {
                requester.Reconnect = true;
                requester.IgnoreProtocolErrors = true;
                requester.ReconnectDelay = 300;
                requester.ReconnectLimit = 3;
                requester.CharacterSet = Encoding.UTF8;
                string r = requester
                    .Post(url, rp, "application/json").ToString();
                requester.Close();
                return r;
            }
            catch
            {
                MessageBox.Show("Нет подключения к интернету!");
                return null;
            }
        }
    }
}
