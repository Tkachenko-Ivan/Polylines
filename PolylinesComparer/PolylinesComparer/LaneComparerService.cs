using System.Collections.Generic;
using PolylinesComparer.Model;
using System.Linq;

namespace PolylinesComparer
{
    /// <summary>
    /// Сравнение линий
    /// </summary>
    class LaneComparerService
    {
        /// <summary>
        /// Сравнение двух линий
        /// </summary>
        /// <param name="firstLine">Первая сравниваемая линия</param>
        /// <param name="secondLine">Вторая сравниваемая линия</param>
        /// <param name="precision">Шаг сетки</param>
        /// <param name="compliance">Ожидаемая степень соответствия, где 1 - полное соответствие</param>
        /// <returns>ИСТИНА - если линии совпали</returns>
        public bool LaneCompare2D(List<Coordinate> firstLine, List<Coordinate> secondLine, double precision,
            double compliance)
        {
            if (firstLine.Count == 0 && secondLine.Count == 0)
                return true;
            if (firstLine.Count == 0 || secondLine.Count == 0)
                return false;

            // Найти точку, которая станет началом координат
            var unated = firstLine.Concat(secondLine).ToList();
            var minX = unated.Min(n => n.Lon);
            var minY = unated.Min(n => n.Lat);
            var origin = new Coordinate(minX, minY);

            // Создание сервиса, которому выпала участь сравнивать эти линии
            var comparer = new LineSpatialIndexesService(precision, origin);

            var firstIndex = comparer.GetLineSpatial2DIndexes(firstLine);
            var lastIndex = comparer.GetLineSpatial2DIndexes(secondLine);

            var allColl = firstIndex.Count; // Общее количество различных элементов 
            var interColl = 0; // Количество элементов, которые есть в обоих множествах
            foreach (var elem in firstIndex)
            {
                if (!lastIndex.Any(n => n.Column == elem.Column && n.Row == elem.Row))
                    allColl++;
                else
                    interColl++;
            }

            return (double) interColl / allColl >= compliance;
        }

        public bool LaneCompare3D(List<Coordinate> firstLine, List<Coordinate> secondLine, double precision,
            double compliance)
        {
            if (firstLine.Count == 0 && secondLine.Count == 0)
                return true;
            if (firstLine.Count == 0 || secondLine.Count == 0)
                return false;

            // Найти точку, которая станет началом координат
            var unated = firstLine.Concat(secondLine).ToList();
            var minX = unated.Min(n => n.Lon);
            var minY = unated.Min(n => n.Lat);
            var minZ = unated.Min(n => n.H);
            var origin = new Coordinate(minX, minY, minZ);

            // Создание сервиса, которому выпала участь сравнивать эти линии
            var comparer = new LineSpatialIndexesService(precision, origin);

            var firstIndex = comparer.GetLineSpatial3DIndexes(firstLine);
            var lastIndex = comparer.GetLineSpatial3DIndexes(secondLine);

            var allColl = firstIndex.Count; // Общее количество различных элементов 
            var interColl = 0; // Количество элементов, которые есть в обоих множествах
            foreach (var elem in firstIndex)
            {
                if (!lastIndex.Any(n => n.Column == elem.Column && n.Row == elem.Row && n.Layer == elem.Layer))
                    allColl++;
                else
                    interColl++;
            }

            return (double) interColl / allColl >= compliance;
        }
    }
}
