using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poster
{
    /// <summary>
    /// Описывает сущность предлагаемого варианта события/места.
    /// </summary>
    [Serializable]
    public class KGItem
    {

        #region Enums

        /// <summary>
        /// Варианты рейтинга событий/мест
        /// </summary>
        public enum ORating
        {

            [Description("0+")]
            R0,

            [Description("6+")]
            R6,

            [Description("12+")]
            R12,

            [Description("16+")]
            R16,

            [Description("18+")]
            R18,
        }
        #endregion


        public KGCategory Type { get; }

        /// <summary>
        /// Дата предлагаемого варианта
        /// </summary>
        public DateTime date;
        public string Date { get { return date.ToShortDateString(); } }

        /// <summary>
        /// Цена предлагаемого варианта
        /// </summary>
        public decimal Price { get; }

        /// <summary>
        /// Рейтинг предлагаемого варианта
        /// </summary>
        ORating rating;
        public string Rating
        {
            get
            {
                return Enumerations.GetDescription(rating);
            }
        }

        /// <summary>
        /// Преобразует string в enum
        /// </summary>
        /// <param name="raw">Исходная строка</param>
        public static ORating Str2Rating(string raw)
        {
            switch (raw)
            {
                case "18+": return ORating.R18;
                case "16+": return ORating.R16;
                case "12+": return ORating.R12;
                case "6+": return ORating.R6;
                default: return ORating.R0;
            }
        }



        /// <summary>
        /// Описание предлагаемого варианта
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///  Местоположение предлагаемого варианта
        /// </summary>
        public KGLocation Location { get; set; }

        /// <summary>
        /// ID места
        /// </summary>
        public int PlaceID { get; set; } = 0;

        /// <summary>
        /// ID события
        /// </summary>
        public int EventID { get; set; } = 0;

        /// <summary>
        /// Основной конструктор предлагаемого варианта.
        /// </summary>
        /// <param name="type">Тип</param>
        /// <param name="date">Дата</param>
        /// <param name="price">Цена</param>
        /// <param name="rating">Рейтинг</param>
        public KGItem(KGCategory type, DateTime date, decimal price, ORating rating, string desc, KGLocation loc)
        {
            Type = type;
            this.date = date;
            Price = price;
            this.rating = rating;
            Description = desc;
            Location = loc;
        }



    }
}
