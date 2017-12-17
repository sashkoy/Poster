using System;
using System.Text.RegularExpressions;

namespace Poster
{
    /// <summary>
    /// Представляет утилиты общего назначения
    /// </summary>
    public static class Utilites
    {
        /// <summary>
        /// Конвертирует дату в UnixTime
        /// </summary>
        /// <param name="date">Исходная дата</param>
        /// <returns>Возвращает штамп времени</returns>
        public static long Date2Unix(DateTime date)
        {
            return (long)(date.Subtract(new DateTime(1970, 1, 1, 0, 0, 0))).TotalSeconds;
        }

        /// <summary>
        /// Убирает HTML теги из строки
        /// </summary>
        /// <param name="input">Исходная строка</param>
        /// <returns></returns>
        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
    }
}
