using System;
using System.Collections.Generic;
using PolylinesComparer.Model;
using System.Linq;

namespace PolylinesComparer
{
    /// <summary>
    /// Сравнение линий
    /// </summary>
    public class LineComparerService
    {
        /// <summary>
        /// Сравнение двух линий
        /// </summary>
        /// <param name="firstLine">Первая сравниваемая линия</param>
        /// <param name="secondLine">Вторая сравниваемая линия</param>
        /// <param name="precision">Шаг сетки</param>
        /// <param name="compliance">Ожидаемая степень соответствия, где 1 - полное соответствие</param>
        /// <returns>ИСТИНА - если линии совпали</returns>
        public bool LineCompare2D(List<Coordinate> firstLine, List<Coordinate> secondLine, double precision,
            double compliance)
        {
            if (!(compliance > 0 && compliance <=1))
                throw new Exception($"Степень соответствия должна принимать значение в диапазоне (0;1], актуальное значение {compliance}");

            if (firstLine.Count == 0 && secondLine.Count == 0)
                return true;
            if (firstLine.Count == 0 || secondLine.Count == 0)
                return false;

            // Найти точку, которая станет началом координат
            var unated = firstLine.Concat(secondLine).ToList();
            var minX = unated.Min(n => n.Lon) - precision * 0.1;
            var minY = unated.Min(n => n.Lat) - precision * 0.1;
            var origin = new Coordinate(minX, minY);

            var comparer = new LineSpatialIndexesService(precision, origin);
            return Compare2D(firstLine, secondLine, comparer) >= compliance;
        }

        /// <summary>
        /// Сравнение двух линий
        /// </summary>
        /// <param name="firstLine">Первая сравниваемая линия</param>
        /// <param name="secondLine">Вторая сравниваемая линия</param>
        /// <param name="precision">Шаг сетки</param>
        /// <param name="compliance">Ожидаемая степень соответствия, где 1 - полное соответствие</param>
        /// <param name="origin">Точка начала координат</param>
        /// <returns>ИСТИНА - если линии совпали</returns>
        public bool LineCompare2D(List<Coordinate> firstLine, List<Coordinate> secondLine, double precision,
            double compliance, Coordinate origin)
        {
            if (!(compliance > 0 && compliance <= 1))
                throw new Exception($"Степень соответствия должна принимать значение в диапазоне (0;1], актуальное значение {compliance}");

            if (firstLine.Count == 0 && secondLine.Count == 0)
                return true;
            if (firstLine.Count == 0 || secondLine.Count == 0)
                return false;

            var comparer = new LineSpatialIndexesService(precision, origin);
            return Compare2D(firstLine, secondLine, comparer) >= compliance;
        }

        /// <summary>
        /// Сравнение двух линий в трёхмерном пространстве
        /// </summary>
        /// <param name="firstLine">Первая сравниваемая линия</param>
        /// <param name="secondLine">Вторая сравниваемая линия</param>
        /// <param name="precision">Шаг сетки</param>
        /// <param name="compliance">Ожидаемая степень соответствия, где 1 - полное соответствие</param>
        /// <returns>ИСТИНА - если линии совпали</returns>
        public bool LineCompare3D(List<Coordinate> firstLine, List<Coordinate> secondLine, double precision,
            double compliance)
        {
            if (!(compliance > 0 && compliance <= 1))
                throw new Exception($"Степень соответствия должна принимать значение в диапазоне (0;1], актуальное значение {compliance}");

            if (firstLine.Count == 0 && secondLine.Count == 0)
                return true;
            if (firstLine.Count == 0 || secondLine.Count == 0)
                return false;

            // Найти точку, которая станет началом координат
            var unated = firstLine.Concat(secondLine).ToList();
            var minX = unated.Min(n => n.Lon) - precision * 0.1;
            var minY = unated.Min(n => n.Lat) - precision * 0.1;
            var minZ = unated.Min(n => n.H) - precision * 0.1;
            var origin = new Coordinate(minX, minY, minZ);

            var comparer = new LineSpatialIndexesService(precision, origin);
            return Compare3D(firstLine, secondLine, comparer) >= compliance;
        }

        /// <summary>
        /// Сравнение двух линий в трёхмерном пространстве
        /// </summary>
        /// <param name="firstLine">Первая сравниваемая линия</param>
        /// <param name="secondLine">Вторая сравниваемая линия</param>
        /// <param name="precision">Шаг сетки</param>
        /// <param name="compliance">Ожидаемая степень соответствия, где 1 - полное соответствие</param>
        /// <param name="origin">Точка начала координат</param>
        /// <returns>ИСТИНА - если линии совпали</returns>
        public bool LineCompare3D(List<Coordinate> firstLine, List<Coordinate> secondLine, double precision,
            double compliance, Coordinate origin)
        {
            if (!(compliance > 0 && compliance <= 1))
                throw new Exception($"Степень соответствия должна принимать значение в диапазоне (0;1], актуальное значение {compliance}");

            if (firstLine.Count == 0 && secondLine.Count == 0)
                return true;
            if (firstLine.Count == 0 || secondLine.Count == 0)
                return false;

            var comparer = new LineSpatialIndexesService(precision, origin);
            return Compare3D(firstLine, secondLine, comparer) >= compliance;
        }

        private double Compare2D(List<Coordinate> firstLine, List<Coordinate> secondLine, LineSpatialIndexesService comparer)
        {
            var firstIndex = comparer.GetLineSpatial2DIndexes(firstLine);
            var lastIndex = comparer.GetLineSpatial2DIndexes(secondLine);

            var allColl = firstIndex.Count; // Общее количество различных элементов 
            var interColl = 0; // Количество элементов, которые есть в обоих множествах
            foreach (var elem in lastIndex)
            {
                if (firstIndex.Any(n => n.Column == elem.Column && n.Row == elem.Row))
                    interColl++;
                else
                    allColl++;
            }
            return (double)interColl / allColl;
        }

        private double Compare3D(List<Coordinate> firstLine, List<Coordinate> secondLine, LineSpatialIndexesService comparer)
        {
            var firstIndex = comparer.GetLineSpatial3DIndexes(firstLine);
            var lastIndex = comparer.GetLineSpatial3DIndexes(secondLine);

            var allColl = firstIndex.Count; // Общее количество различных элементов 
            var interColl = 0; // Количество элементов, которые есть в обоих множествах
            foreach (var elem in lastIndex)
            {
                if (firstIndex.Any(n => n.Column == elem.Column && n.Row == elem.Row && n.Layer == elem.Layer))
                    interColl++;
                else
                    allColl++;
            }
            return (double)interColl / allColl;
        }
    }
}
