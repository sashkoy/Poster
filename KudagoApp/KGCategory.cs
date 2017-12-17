using System;
using System.ComponentModel;

namespace Poster
{
    /// <summary>
    /// Категория
    /// </summary>
    [Serializable]
    public class KGCategory : IComparable
    {

        /// <summary>
        /// Тип события/места
        /// </summary>

        /// <summary>
        /// Типы категорий
        /// </summary>
        public enum CType
        {
            [Description("Заголовок-пустышка")]
            Header,

            [Description("Событие")]
            Event,

            [Description("Место")]
            Place
        }

        /// <summary>
        /// Имя для отображения
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Имя для фильтра
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Тип категории
        /// </summary>
        public CType Type { get; set; }

        /// <summary>
        /// Основной конструктор
        /// </summary>
        /// <param name="name">Имя для отображения</param>
        /// <param name="slug">Имя для фильтра</param>
        public KGCategory(string name, CType type = CType.Header, string slug = "")
        {
            Name = char.ToUpper(name[0]).ToString() + name.Substring(1);
            Slug = slug;
            Type = type;
        }

        public override string ToString()
        {
            return Name;
        }

        public int CompareTo(object obj)
        {
            return Name.CompareTo(obj);
        }
    }
}
