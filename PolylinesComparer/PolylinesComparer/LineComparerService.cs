using System.Collections.Generic;
using System.Globalization;
using PolylinesComparer.Model;
using System.Linq;

namespace PolylinesComparer
{
    /// <summary>
    /// Сравнение линий
    /// </summary>
    class LineComparerService
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
            if (firstLine.Count == 0 && secondLine.Count == 0)
                return true;
            if (firstLine.Count == 0 || secondLine.Count == 0)
                return false;

            var comparer = new LineSpatialIndexesService(precision, origin);
            return Compare3D(firstLine, secondLine, comparer) >= compliance;
        }

        /// <summary>
        /// Группирует линии по соответствию пространственных индексов (строит кластеры) и подсчитывает количество кластеров
        /// </summary>
        /// <param name="lines">Список линий</param>
        /// <param name="precision">Шаг сетки</param>
        /// <param name="compliance">Ожидаемая степень соответствия, где 1 - полное соответствие</param>
        /// <returns>Количество кластеров</returns>
        public int DifferentIndexesNumber2D(List<List<Coordinate>> lines, double precision, double compliance)
        {
            // Заполнение матрицы
            bool[,] matrix = new bool[lines.Count, lines.Count];
            for (int i = 0; i < lines.Count; i++)
            {
                matrix[i, i] = true;
                for (int j = i + 1; j < lines.Count; j++)
                {
                    matrix[i, j] = LineCompare2D(lines[i], lines[j], precision, compliance);
                    matrix[j, i] = matrix[i, j];
                }
            }

            int count = 0;
            var excludeIndex = new List<int>();
            for (int i = 0; i < lines.Count; i++)
            {
                if (excludeIndex.Contains(i))
                    continue;

                count++;

                var mask = BitMaskOfRow(matrix, i, excludeIndex);

                var excludeSubIndex = new List<int>();
                for (int j = i + 1; j < lines.Count; j++)
                {
                    var maskC = BitMaskOfRow(matrix, j, excludeIndex);
                    if (mask == maskC)
                        excludeSubIndex.Add(j);
                }

                excludeIndex.Add(i);
                excludeIndex.AddRange(excludeSubIndex);
            }

            return count;
        }

        private string BitMaskOfRow(bool[,] matrix, int ri, List<int> excludeIndex)
        {
            var count = matrix.GetLength(0);
            var result = "";
            for (int j = 0; j < count; j++)
                if (!excludeIndex.Contains(j))
                    result += matrix[ri, j] ? "1" : "0";
            return result;
        }

        private double Compare2D(List<Coordinate> firstLine, List<Coordinate> secondLine, LineSpatialIndexesService comparer)
        {
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
            return (double)interColl / allColl;
        }

        private double Compare3D(List<Coordinate> firstLine, List<Coordinate> secondLine, LineSpatialIndexesService comparer)
        {
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
            return (double)interColl / allColl;
        }
    }
}
