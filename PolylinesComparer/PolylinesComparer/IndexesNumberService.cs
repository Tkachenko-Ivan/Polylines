using System.Collections.Generic;
using System.Linq;
using PolylinesComparer.Model;

namespace PolylinesComparer
{
    /// <summary>
    /// Подсчёт количества одинаковых линий
    /// </summary>
    public class IndexesNumberService
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
            var lineComparerService = new LineComparerService();

            // Заполнение матрицы
            bool[,] matrix = new bool[lines.Count, lines.Count];
            for (int i = 0; i < lines.Count; i++)
            {
                matrix[i, i] = true;
                for (int j = i + 1; j < lines.Count; j++)
                {
                    matrix[i, j] = lineComparerService.LineCompare2D(lines[i], lines[j], precision, compliance);
                    matrix[j, i] = matrix[i, j];
                }
            }

            return Distaff(matrix);
        }

        /// <summary>
        /// Группирует линии по соответствию пространственных индексов (строит кластеры) и подсчитывает количество кластеров
        /// </summary>
        /// <param name="lines">Список линий</param>
        /// <param name="precision">Шаг сетки</param>
        /// <param name="compliance">Ожидаемая степень соответствия, где 1 - полное соответствие</param>
        /// <param name="origin">Начало координат</param>
        /// <returns>Количество кластеров</returns>
        public int DifferentIndexesNumber2D(List<List<Coordinate>> lines, double precision, double compliance,
            Coordinate origin)
        {
            var lineComparerService = new LineComparerService();
            var lineSpatialIndexesService = new LineSpatialIndexesService(precision, origin);

            // Проиндексировать
            var indexes = lines.Select(t => lineSpatialIndexesService.GetLineSpatial2DIndexes(t)).ToList();

            // Заполнение матрицы
            bool[,] matrix = new bool[lines.Count, lines.Count];
            for (int i = 0; i < lines.Count; i++)
            {
                matrix[i, i] = true;
                for (int j = i + 1; j < lines.Count; j++)
                {
                    matrix[i, j] = lineComparerService.Compare2D(indexes[i], indexes[j], compliance);
                    matrix[j, i] = matrix[i, j];
                }
            }

            return Distaff(matrix);
        }

        /// <summary>
        /// Группирует линии по соответствию трёхмерных пространственных индексов (строит кластеры) и подсчитывает количество кластеров
        /// </summary>
        /// <param name="lines">Список линий</param>
        /// <param name="precision">Шаг сетки</param>
        /// <param name="compliance">Ожидаемая степень соответствия, где 1 - полное соответствие</param>
        /// <returns>Количество кластеров</returns>
        public int DifferentIndexesNumber3D(List<List<Coordinate>> lines, double precision, double compliance)
        {
            var lineComparerService = new LineComparerService();

            // Заполнение матрицы
            bool[,] matrix = new bool[lines.Count, lines.Count];
            for (int i = 0; i < lines.Count; i++)
            {
                matrix[i, i] = true;
                for (int j = i + 1; j < lines.Count; j++)
                {
                    matrix[i, j] = lineComparerService.LineCompare3D(lines[i], lines[j], precision, compliance);
                    matrix[j, i] = matrix[i, j];
                }
            }

            return Distaff(matrix);
        }

        /// <summary>
        /// Группирует линии по соответствию трёхмерных пространственных индексов (строит кластеры) и подсчитывает количество кластеров
        /// </summary>
        /// <param name="lines">Список линий</param>
        /// <param name="precision">Шаг сетки</param>
        /// <param name="compliance">Ожидаемая степень соответствия, где 1 - полное соответствие</param>
        /// <param name="origin">Начало координат</param>
        /// <returns>Количество кластеров</returns>
        public int DifferentIndexesNumber3D(List<List<Coordinate>> lines, double precision, double compliance,
            Coordinate origin)
        {
            var lineComparerService = new LineComparerService();
            var lineSpatialIndexesService = new LineSpatialIndexesService(precision, origin);

            // Проиндексировать
            var indexes = lines.Select(t => lineSpatialIndexesService.GetLineSpatial3DIndexes(t)).ToList();

            // Заполнение матрицы
            bool[,] matrix = new bool[lines.Count, lines.Count];
            for (int i = 0; i < lines.Count; i++)
            {
                matrix[i, i] = true;
                for (int j = i + 1; j < lines.Count; j++)
                {
                    matrix[i, j] = lineComparerService.Compare3D(indexes[i], indexes[j], compliance);
                    matrix[j, i] = matrix[i, j];
                }
            }

            return Distaff(matrix);
        }

        /// <summary>
        /// Рассортировка объектов по множествам
        /// </summary>
        /// <param name="matrix">Матрица отношений между объектами</param>
        /// <returns>количество множеств</returns>
        private int Distaff(bool[,] matrix)
        {
            var groups = new List<List<int>>();

            var count = matrix.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                // Если объект порождает группу из самого себя, то single - ИСТИНА
                bool single = true;

                var newGroups = new List<List<int>>();
                // Пытаемся добавить объект в существующие группы
                foreach (var group in groups)
                {
                    var newGroup = new List<int>();
                    foreach (var element in group)
                        if (matrix[i, element])
                            newGroup.Add(element);

                    if (newGroup.Count == group.Count)
                    {
                        // Добавить элемент в текущую группу
                        group.Add(i);
                        single = false;
                    }
                    else if (newGroup.Any())
                    {
                        // Элемент порождает новую группу из части элементов текущей
                        newGroup.Add(i);
                        newGroups.Add(newGroup);
                        single = false;
                    }
                }

                if (single)
                    // Элемент не был добавлен ни к одной группе
                    groups.Add(new List<int> {i});
                else
                    for (int j = 0; j < newGroups.Count; j++)
                    {
                        bool unique = true;

                        // Проверить, является ли порождённая группа подмножеством другой порождённой группы
                        for (int l = j + 1; l < newGroups.Count; l++)
                            if (newGroups[j].Intersect(newGroups[l]).Count() == newGroups[j].Count)
                            {
                                unique = false;
                                break;
                            }

                        if (!unique)
                            continue;

                        // Проверить, является ли порождённая группа подмножеством другой группы из списка
                        foreach (var existing in groups)
                            if (newGroups[j].Intersect(existing).Count() == newGroups[j].Count)
                            {
                                unique = false;
                                break;
                            }

                        // Группа прошла все проверки - добавить в список
                        if (unique)
                            groups.Add(newGroups[j]);
                    }
            }
            return groups.Count;
        }
    }
}