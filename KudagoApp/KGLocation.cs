using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poster
{
    /// <summary>
    /// Местоположение объекта
    /// </summary>
    [Serializable]
    public class KGLocation
    {
        /// <summary>
        /// Координата по X
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Координата по Y
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Основной конструктор местоположения
        /// </summary>
        /// <param name="x">Координата по X</param>
        /// <param name="y">Координата по Y</param>
        public KGLocation(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Считает расстояние до объекта
        /// </summary>
        public double Length
        {
            get
            {
                double deltaX = Math.Abs(KGAgregator.myLocation.X - X);
                double deltaY = Math.Abs(KGAgregator.myLocation.Y - Y);
                return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            }
        }

        public override string ToString()
        {
            return string.Format("{0:0.000000}; {1:0.000000}", X, Y);
        }

    }
}
