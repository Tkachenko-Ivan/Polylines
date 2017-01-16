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
        public bool LaneCompare(List<Coordinate> firstLine, List<Coordinate> secondLine, double precision, double compliance)
        {
            if (firstLine.Count == 0 && secondLine.Count == 0)
                return true;
            if (firstLine.Count == 0 || secondLine.Count == 0)
                return false;

            //bool is3D = firstLine.HasH && secondLine.HasH;

            // Найти точку, которая станет началом координат
            Coordinate origin;
            var unated = firstLine.Concat(secondLine).ToList();
            var minX = unated.Min(n => n.Lon);
            var minY = unated.Min(n => n.Lat);
            /*if (is3D)
            {
                // Для трёхмерного пространства
                var minZ = unated.Min(n => n.H);
                origin = new Coordinate(minX, minY, minZ);
            }
            else*/
                origin = new Coordinate(minX, minY);

            // Создание сервиса, которому выпала участь сравнивать эти линии
            var comparer = new LineSpatialIndexesService(precision, origin, false);

            var firstIndex = comparer.GetLineSpatialIndexes(firstLine);
            var lastIndex = comparer.GetLineSpatialIndexes(secondLine);

            var allColl = firstIndex.Count; // Общее количество различных элементов 
            var interColl = 0; // Количество элементов, которые есть в обоих множествах
            foreach (var elem in firstIndex)
            {
                if (!lastIndex.Any(n => n.Column == elem.Column && n.Row == elem.Row))
                    allColl++;
                else
                    interColl++;
            }

            if ((double)interColl / allColl >= compliance)
                return true;
            return false;
        }
    }
}
