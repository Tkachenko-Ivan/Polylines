using PolylinesComparer.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace PolylinesComparer
{
    /// <summary>
    /// Создание пространственного индекса для полилинии
    /// </summary>
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public class LineSpatialIndexesService
    {
        private readonly double _precision;

        private readonly Coordinate _origin;

        private readonly bool _hasH;

        /// <summary>
        /// Создание пространственного индекса 
        /// </summary>
        /// <param name="precision">Шаг сетки пространственного индекса</param>
        /// <param name="origin">Начало координат</param>
        /// <param name="hasH">Работа с координатами в пространстве (3D)</param>
        public LineSpatialIndexesService(double precision, Coordinate origin, bool hasH)
        {
            _precision = precision;
            _origin = origin;
            _hasH = hasH;
        }

        public List<GridCell> GetLineSpatialIndexes(List<Coordinate> line)
        {
            var result = new List<GridCell>();
            if (!line.Any())
                return result;

            var prevX = line[0].Lon - _origin.Lon;
            var prevY = line[0].Lat - _origin.Lat;
            var prevCoordinate = _hasH ? new Coordinate(prevX, prevY, line[0].H - _origin.H) : new Coordinate(prevX, prevY);

            CellFromPoint(prevCoordinate, result);
            for (int i = 1; i < line.Count; i++)
            {
                var currentX = line[i].Lon - _origin.Lon;
                var currentY = line[i].Lat - _origin.Lat;
                var currentCoordinate = _hasH
                    ? new Coordinate(currentX, currentY, line[i].H - _origin.H)
                    : new Coordinate(currentX, currentY);

                if (_hasH)
                    CellsFromLine3D(prevCoordinate, currentCoordinate, ref result);
                else
                    FillingCells(prevCoordinate, currentCoordinate, ref result);

                CellFromPoint(currentCoordinate, result);
                prevCoordinate = currentCoordinate;
            }
            
            return result;
        }

        /// <summary>
        /// Заполняет ячейку сетки по координатам точки
        /// </summary>
        private void CellFromPoint(Coordinate сoordinate, List<GridCell> result)
        {
            var distX = (int) Math.Floor(сoordinate.Lon / _precision); // Колонка справа
            var distY = (int) Math.Floor(сoordinate.Lat / _precision); // Строка сверху

            if (_hasH)
            {
                var distZ = (int)Math.Floor(сoordinate.H / _precision); // Слой сверху

                AddToResult(distX, distY, distZ, result);

                if (сoordinate.Lon % _precision == 0)
                    AddToResult(distX - 1, distY, distZ, result);
                if (сoordinate.Lat % _precision == 0)
                    AddToResult(distX, distY - 1, distZ, result);
                if (сoordinate.H % _precision == 0)
                    AddToResult(distX, distY, distZ - 1, result);

                if (сoordinate.Lon % _precision == 0 && сoordinate.Lat % _precision == 0)
                    AddToResult(distX - 1, distY - 1, distZ, result);
                if (сoordinate.Lon % _precision == 0 && сoordinate.H % _precision == 0)
                    AddToResult(distX - 1, distY, distZ - 1, result);
                if (сoordinate.Lat % _precision == 0 && сoordinate.H % _precision == 0)
                    AddToResult(distX, distY - 1, distZ - 1, result);

                if (сoordinate.Lon % _precision == 0 && сoordinate.Lat % _precision == 0 && сoordinate.H % _precision == 0 )
                    AddToResult(distX - 1, distY - 1, distZ - 1, result);
            }
            else
            {
                AddToResult(distX, distY, result);

                if (сoordinate.Lon % _precision == 0)
                    AddToResult(distX - 1, distY, result);
                if (сoordinate.Lat % _precision == 0)
                    AddToResult(distX, distY - 1, result);
                if (сoordinate.Lat % _precision == 0 && сoordinate.Lon % _precision == 0)
                    AddToResult(distX - 1, distY - 1, result);
            }
        }

        /// <summary>
        /// Построение индекса линии в пространстве
        /// </summary>
        /// <remarks>
        /// Вместо трёхмерного пространства рассматривается две проекции, тем самым исключая аппликату из расчёта
        /// </remarks>
        private void CellsFromLine3D(Coordinate prevCoordinate, Coordinate сoordinate, ref List<GridCell> result)
        {
            // Уравнение прямой между этими двумя точками
            double a, b, c, d;
            LineEquation(prevCoordinate, сoordinate, out a, out b, out c, out d);

            var subResultProject1 = new List<GridCell>();
            var subResultProject2 = new List<GridCell>();

            if (Math.Abs(b) < Math.Abs(a) && Math.Abs(b) < Math.Abs(c))
            {
                // По Y
                // Проекция на OXY (OX - абсцисса; OY - ордината)
                FillingCells(a, b, d, prevCoordinate.Lon, prevCoordinate.Lat, сoordinate.Lon, сoordinate.Lat, subResultProject1);
                // Проекция на OYZ (OY - абсцисса; OZ - ордината)
                FillingCells(b, c, d, prevCoordinate.Lat, prevCoordinate.H, сoordinate.Lat, сoordinate.H, subResultProject2);

                result.AddRange(subResultProject1.Join(subResultProject2, p1 => p1.Row, p2 => p2.Column,
                    (p1, p2) => new GridCell(p2.Column, p1.Column, p2.Row)).ToList());
            }
            else if (Math.Abs(a) < Math.Abs(b) && Math.Abs(a) < Math.Abs(c))
            {
                // По X
                // Проекция на OXY (OX - абсцисса; OY - ордината)
                FillingCells(a, b, d, prevCoordinate.Lon, prevCoordinate.Lat, сoordinate.Lon, сoordinate.Lat, subResultProject1);
                // Проекция на OXZ (OX - абсцисса; OZ - ордината)
                FillingCells(a, c, d, prevCoordinate.Lon, prevCoordinate.H, сoordinate.Lon, сoordinate.H, subResultProject2);

                result.AddRange(subResultProject1.Join(subResultProject2, p1 => p1.Column, p2 => p2.Column,
                    (p1, p2) => new GridCell(p1.Row, p1.Column, p2.Row)).ToList());
            }
            else
            {
                // По Z
                // Проекция на OXZ (OX - абсцисса; OZ - ордината)
                FillingCells(a, c, d, prevCoordinate.Lon, prevCoordinate.H, сoordinate.Lon, сoordinate.H, subResultProject1);
                // Проекция на OYZ (OY - абсцисса; OZ - ордината)
                FillingCells(b, c, d, prevCoordinate.Lat, prevCoordinate.H, сoordinate.Lat, сoordinate.H, subResultProject2);

                result.AddRange(subResultProject1.Join(subResultProject2, p1 => p1.Row, p2 => p2.Row,
                    (p1, p2) => new GridCell(p2.Column, p1.Column, p1.Row)).ToList());
            }
        }

        /// <summary>
        /// Построение индекса линии на плоскости
        /// </summary>
        private void FillingCells(Coordinate prevCoordinate, Coordinate сoordinate, ref List<GridCell> result)
        {
            // Уравнение прямой между этими двумя точками
            double a, b, c;
            LineEquation(prevCoordinate, сoordinate, out a, out b, out c);

            FillingCells(a, b, c, prevCoordinate.Lon, prevCoordinate.Lat, сoordinate.Lon, сoordinate.Lat, result);
        }

        /// <summary>
        /// Заполняет ячейки сетки, которые лежат на прямой между первой и второй точкой на плоскости
        /// </summary>
        private void FillingCells(double a, double b, double c, double prevDistX, double prevDistY, double distX, double distY, List<GridCell> result)
        {
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
            var xGrid = ++stColumn * _precision; // Коорданата X правой границы сетки
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
                xGrid = ++stColumn * _precision;
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
                xGrid = --stColumn * _precision;
            }
        }

        private void IncreasesByY(double a, double b, double c, double prevDistY, double distY,
            List<GridCell> result)
        {
            var stRow = (int)Math.Floor(prevDistY / _precision); // Строка, которой принадлежит начальная точка
            var yGrid = ++stRow * _precision; // Коорданата Y верхней границы сетки
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
                yGrid = ++stRow * _precision;
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
                yGrid = --stRow * _precision;
            }
        }


        private void AddToResult(int column, int row, List<GridCell> result)
        {
            if (column != -1 && row != -1)
                if (!result.Any(n => n.Column == column && n.Row == row))
                    result.Add(new GridCell(row, column));
        }

        private void AddToResult(int column, int row, int layer, List<GridCell> result)
        {
            if (column != -1 && row != -1 && layer != -1)
                if (!result.Any(n => n.Column == column && n.Row == row && n.Layer == layer))
                    result.Add(new GridCell(row, column, layer));
        }

        /// <summary>
        /// По двум точкам определяет константы для уравнения прямой на плоскости
        /// </summary>
        /// <remarks>
        /// Ax + By + C = 0
        /// </remarks>
        public static void LineEquation(Coordinate firstPoint, Coordinate secondPoint, out double a, out double b, out double c)
        {
            double x1 = firstPoint.Lon;
            double x2 = secondPoint.Lon;
            double y1 = firstPoint.Lat;
            double y2 = secondPoint.Lat;

            a = y2 - y1;
            b = x1 - x2;
            c = x2 * y1 - y2 * x1;
        }

        /// <summary>
        /// По двум точкам определяет константы для уравнения прямой в пространстве
        /// </summary>
        /// <remarks>
        /// Ax + By + Cz + D = 0
        /// </remarks>
        public static void LineEquation(Coordinate firstPoint, Coordinate secondPoint, out double a, out double b,
            out double c, out double d)
        {
            double x1 = firstPoint.Lon;
            double x2 = secondPoint.Lon;
            double y1 = firstPoint.Lat;
            double y2 = secondPoint.Lat;
            double z1 = firstPoint.H;
            double z2 = secondPoint.H;

            a = y2 * z2 - y2 * z1 - y1 * z2 + y1 * z1;
            b = x2 * z1 - x2 * z2 + x1 * z2 - x1 * z1;
            c = x2 * y1 - x2 * y2 + x1 * y2 - x1 * y1;
            d = y1*x2*z2 - x1*y2*z2 + z1*y2*x2 - 2*y1*x2*z1 + x1*y1*z1;
        }
    }
}
