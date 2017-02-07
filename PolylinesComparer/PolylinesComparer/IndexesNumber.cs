using System.Collections.Generic;
using PolylinesComparer.Model;

namespace PolylinesComparer
{
    /// <summary>
    /// Подсчёт количества одинаковых линий
    /// </summary>
    public class IndexesNumber
    {
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
                    matrix[i, j] = new LineComparerService().LineCompare2D(lines[i], lines[j], precision, compliance);
                    matrix[j, i] = matrix[i, j];
                }
            }

            return Distaff(matrix);
        }

        public int DifferentIndexesNumber2D(List<List<Coordinate>> lines, double precision, double compliance, Coordinate origin)
        {
            // Заполнение матрицы
            bool[,] matrix = new bool[lines.Count, lines.Count];
            for (int i = 0; i < lines.Count; i++)
            {
                matrix[i, i] = true;
                for (int j = i + 1; j < lines.Count; j++)
                {
                    matrix[i, j] = new LineComparerService().LineCompare2D(lines[i], lines[j], precision, compliance, origin);
                    matrix[j, i] = matrix[i, j];
                }
            }

            return Distaff(matrix);
        }

        public int DifferentIndexesNumber3D(List<List<Coordinate>> lines, double precision, double compliance)
        {
            // Заполнение матрицы
            bool[,] matrix = new bool[lines.Count, lines.Count];
            for (int i = 0; i < lines.Count; i++)
            {
                matrix[i, i] = true;
                for (int j = i + 1; j < lines.Count; j++)
                {
                    matrix[i, j] = new LineComparerService().LineCompare3D(lines[i], lines[j], precision, compliance);
                    matrix[j, i] = matrix[i, j];
                }
            }

            return Distaff(matrix);
        }

        public int DifferentIndexesNumber3D(List<List<Coordinate>> lines, double precision, double compliance, Coordinate origin)
        {
            // Заполнение матрицы
            bool[,] matrix = new bool[lines.Count, lines.Count];
            for (int i = 0; i < lines.Count; i++)
            {
                matrix[i, i] = true;
                for (int j = i + 1; j < lines.Count; j++)
                {
                    matrix[i, j] = new LineComparerService().LineCompare3D(lines[i], lines[j], precision, compliance, origin);
                    matrix[j, i] = matrix[i, j];
                }
            }

            return Distaff(matrix);
        }

        private int Distaff(bool[,] matrix)
        {
            var count = matrix.GetLength(0);

            int number = 0;
            var excludeIndex = new List<int>();
            for (int i = 0; i < count; i++)
            {
                if (excludeIndex.Contains(i))
                    continue;

                number++;

                var mask = BitMaskOfRow(matrix, i, excludeIndex);

                var excludeSubIndex = new List<int>();
                for (int j = i + 1; j < count; j++)
                {
                    var maskC = BitMaskOfRow(matrix, j, excludeIndex);
                    if (mask == maskC)
                        excludeSubIndex.Add(j);
                }

                excludeIndex.Add(i);
                excludeIndex.AddRange(excludeSubIndex);
            }

            return number;
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
    }
}