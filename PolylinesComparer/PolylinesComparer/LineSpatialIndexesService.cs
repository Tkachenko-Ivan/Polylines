using PolylinesComparer.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PolylinesComparer
{
    /// <summary>
    /// Создание пространственного индекса для полилинии
    /// </summary>
    class LineSpatialIndexesService
    {
        private readonly double _precision;

        private readonly Coordinate _origin;

        /// <summary>
        /// Создание пространственного индекса 
        /// </summary>
        /// <param name="precision">Шаг сетки пространственного индекса</param>
        /// <param name="origin">Начало координат</param>
        public LineSpatialIndexesService(double precision, Coordinate origin)
        {
            _precision = precision;
            _origin = origin;
        }

        public List<GridCell> GetLineSpatialIndexes(List<Coordinate> line)
        {
            var result = new List<GridCell>();
            if (!line.Any())
                return result;

            var prevX = line[0].Lon - _origin.Lon;
            var prevY = line[0].Lat - _origin.Lat;

            CellFromPoint(prevX, prevY, result);
            for (int i = 1; i < line.Count; i++)
            {
                var currentX = line[i].Lon - _origin.Lon;
                var currentY = line[i].Lat - _origin.Lat;

                CellsFromLine(prevX, prevY, currentX, currentY, result);
                CellFromPoint(currentX, currentY, result);

                prevX = currentX;
                prevY = currentY;
            }
            return result;
        }

        /// <summary>
        /// Заполняет ячейку сетки по координатам точки
        /// </summary>
        private void CellFromPoint(double x, double y, List<GridCell> result)
        {
            var distX = (int)Math.Floor(x / _precision); // Колонка справа
            var distY = (int)Math.Floor(y / _precision); // Строка сверху
            AddToResult(distX, distY, result);

            if (x % _precision == 0)
                AddToResult(distX - 1, distY, result);
            if (y % _precision == 0)
                AddToResult(distX, distY - 1, result);
            if (y % _precision == 0 && x % _precision == 0)
                AddToResult(distX - 1, distY - 1, result);
        }

        /// <summary>
        /// Заполняет ячейки сетки, которые лежат на прямой между первой и второй точкой
        /// </summary>
        private void CellsFromLine(double prevDistX, double prevDistY, double distX, double distY, List<GridCell> result)
        {
            // Уравнение прямой между этими двумя точками
            double a, b, c;
            LineEquation(new Coordinate { Lat = prevDistY, Lon = prevDistX },
                new Coordinate { Lat = distY, Lon = distX }, out a, out b, out c);

            if (Math.Abs(b) < Math.Abs(a))
            {
                if (a > 0)
                    IncreasesByY(a, b, c, prevDistY, distY, result);
                else
                    DecreasesByY(a, b, c, prevDistY, distY, result);
            }
            else
            {
                if (b < 0)
                    IncreasesByX(a, b, c, prevDistX, distX, result);
                else
                    DecreasesByX(a, b, c, prevDistX, distX, result);
            }
        }

        private void IncreasesByX(double a, double b, double c, double prevDistX, double distX,
            List<GridCell> result)
        {
            var stColumn = (int)Math.Floor(prevDistX / _precision); // Колонка, которой принадлежит начальная точка
            var xGrid = (++stColumn) * _precision; // Коорданата X правой границы сетки
            while (xGrid <= distX)
            {
                var y = -(a * xGrid + c) / b; // Значение Y в этой точке
                var stRow = (int)Math.Floor(y / _precision); // Строка, которой принадлежит точка

                AddToResult(stColumn - 1, stRow, result);
                AddToResult(stColumn, stRow, result);
                if (y % _precision == 0)
                {
                    AddToResult(stColumn - 1, stRow - 1, result);
                    AddToResult(stColumn, stRow - 1, result);
                }
                xGrid = (++stColumn) * _precision;
            }
        }

        private void DecreasesByX(double a, double b, double c, double prevDistX, double distX,
            List<GridCell> result)
        {
            var stColumn = (int)Math.Floor(prevDistX / _precision); // Колонка, которой принадлежит начальная точка
            var xGrid = stColumn * _precision; // Коорданата X левой границы сетки
            while (xGrid >= distX)
            {
                var y = -(a * xGrid + c) / b; // Значение Y в этой точке
                var stRow = (int)Math.Floor(y / _precision); // Строка, которой принадлежит точка

                AddToResult(stColumn, stRow, result);
                AddToResult(stColumn - 1, stRow, result);
                if (y % _precision == 0)
                {
                    AddToResult(stColumn, stRow - 1, result);
                    AddToResult(stColumn - 1, stRow - 1, result);
                }
                xGrid = (--stColumn) * _precision;
            }
        }

        private void IncreasesByY(double a, double b, double c, double prevDistY, double distY,
            List<GridCell> result)
        {
            var stRow = (int)Math.Floor(prevDistY / _precision); // Строка, которой принадлежит начальная точка
            var yGrid = (++stRow) * _precision; // Коорданата Y верхней границы сетки
            while (yGrid <= distY)
            {
                var x = -(b * yGrid + c) / a; // Значение X в этой точке
                var stColumn = (int)Math.Floor(x / _precision); // Колонка, которой принадлежит точка

                AddToResult(stColumn, stRow - 1, result);
                AddToResult(stColumn, stRow, result);
                if (x % _precision == 0)
                {
                    AddToResult(stColumn - 1, stRow - 1, result);
                    AddToResult(stColumn - 1, stRow, result);
                }
                yGrid = (++stRow) * _precision;
            }
        }

        private void DecreasesByY(double a, double b, double c, double prevDistY, double distY,
            List<GridCell> result)
        {
            var stRow = (int)Math.Floor(prevDistY / _precision); // Строка, которой принадлежит начальная точка
            var yGrid = stRow * _precision; // Коорданата нижней Y границы сетки
            while (yGrid >= distY)
            {
                var x = -(b * yGrid + c) / a; // Значение X в этой точке
                var stColumn = (int)Math.Floor(x / _precision); // Колонка, которой принадлежит точка

                AddToResult(stColumn, stRow, result);
                AddToResult(stColumn, stRow - 1, result);
                if (x % _precision == 0)
                {
                    AddToResult(stColumn - 1, stRow, result);
                    AddToResult(stColumn - 1, stRow - 1, result);
                }
                yGrid = (--stRow) * _precision;
            }
        }

        private void AddToResult(int column, int row, List<GridCell> result)
        {
            if (column != -1 && row != -1)
                if (!result.Any(n => n.Column == column && n.Row == row))
                    result.Add(new GridCell { Column = column, Row = row });
        }

        /// <summary>
        /// По двум точкам определяет константы для уравнения прямой
        /// </summary>
        public static void LineEquation(Coordinate firstPoint, Coordinate secondPoint, out double a, out double b, out double c)
        {
            a = secondPoint.Lat - firstPoint.Lat;
            b = firstPoint.Lon - secondPoint.Lon;
            c = secondPoint.Lon * firstPoint.Lat - secondPoint.Lat * firstPoint.Lon;
        }
    }
}
