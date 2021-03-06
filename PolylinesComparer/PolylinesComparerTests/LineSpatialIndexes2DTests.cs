﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolylinesComparer;
using PolylinesComparer.Model;

namespace PolylinesComparerTests
{
    /// <summary>
    /// Тестирование создания пространственных индексов на плоскости
    /// </summary>
    [TestClass]
    public class LineSpatialIndexes2DTests
    {
        /// <summary>
        /// Простой тест для линии возрастающей по осям OX и OY 
        ///     (по OX интенсивнее, по OY в границах одной строки)
        /// </summary>
        /// <remarks>
        /// Тест должен показать что оно вообще работает, и что не появляется избыточных записей
        /// </remarks>
        [TestMethod]
        public void SimpleIndexes2DTest()
        {
            var indexesService = new LineSpatialIndexesService(10, new Coordinate(0, 0));

            var line = new List<Coordinate> {new Coordinate(17, 12), new Coordinate(57, 18)};
            var index = indexesService.GetLineSpatial2DIndexes(line);

            Assert.AreEqual(5, index.Count, "Неверное количество ячеек индекса");
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 2));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 3));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 4));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 5));
        }

        /// <summary>
        /// Тест на построение индекса линии, которая пересечёт некоторые ячейки пространственной сетки 
        ///     либо только слева, либо, наоборот, только справа
        /// </summary>
        /// <remarks>
        /// Тест должен показать что ячейки попадают в индекс независимо от того,
        ///     как их пересекает линия
        /// </remarks>
        [TestMethod]
        public void LeftOrRightIntersectIndexes2DTest()
        {
            var indexesService = new LineSpatialIndexesService(10, new Coordinate(0, 0));

            var line = new List<Coordinate> {new Coordinate(13, 8), new Coordinate(34, 25)};
            var index = indexesService.GetLineSpatial2DIndexes(line);

            Assert.AreEqual(5, index.Count, "Неверное количество ячеек индекса");
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 2));
            Assert.IsTrue(index.Any(sp => sp.Row == 2 && sp.Column == 2));
            Assert.IsTrue(index.Any(sp => sp.Row == 2 && sp.Column == 3));
        }

        /// <summary>
        /// Тест на построение индекса линии, которая рассматривается по оси OX,
        ///     однако не имеет для начальной и конечной ячейки пересечения с её 
        ///     вертикальными границами (характерно для концов отрезка)
        /// </summary>
        /// <remarks>
        /// Тест должен показать, что ячейки попадут в индекс,
        ///     даже если отсутствует пересечение с границей сетки
        /// </remarks>
        [TestMethod]
        public void NotIntersectVerticalLinesIndexes2DTest()
        {
            var indexesService = new LineSpatialIndexesService(10, new Coordinate(0, 0));

            var line = new List<Coordinate> {new Coordinate(4, 8), new Coordinate(26, 22)};
            var index = indexesService.GetLineSpatial2DIndexes(line);

            Assert.AreEqual(5, index.Count, "Неверное количество ячеек индекса");
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 2));
            Assert.IsTrue(index.Any(sp => sp.Row == 2 && sp.Column == 2));
        }

        /// <summary>
        /// Тест на построение индекса линии, 
        ///     которая полностью умещается в одной единственной ячейке пространственного индекса
        /// </summary>
        /// <remarks>
        /// Тест должен показать что эта ячейка попала в индекс.
        /// </remarks>
        [TestMethod]
        public void LineInCellIndexes2DTest()
        {
            var indexesService = new LineSpatialIndexesService(10, new Coordinate(0, 0));

            var line = new List<Coordinate>
            {
                new Coordinate(11, 11),
                new Coordinate(13, 17),
                new Coordinate(17, 18),
                new Coordinate(18, 15),
                new Coordinate(17, 11)
            };
            var index = indexesService.GetLineSpatial2DIndexes(line);

            Assert.AreEqual(1, index.Count, "Неверное количество ячеек индекса");
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 1));
        }

        /// <summary>
        /// Тест на построение индекса, когда промежуточный участок не 
        ///     имеет пересчений с сеткой пространственного индекса
        /// </summary>
        /// <remarks>
        /// Тест должен показать что такие участки не выпадают из индекса
        /// </remarks>
        [TestMethod]
        public void SegmentinCellIndexes2DTest()
        {
            var indexesService = new LineSpatialIndexesService(10, new Coordinate(0, 0));

            var line = new List<Coordinate>
            {
                new Coordinate(5, 5),
                new Coordinate(15, 12),
                new Coordinate(15, 18),
                new Coordinate(25, 25)
            };
            var index = indexesService.GetLineSpatial2DIndexes(line);

            Assert.AreEqual(5, index.Count, "Неверное количество ячеек индекса");
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 2 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 2 && sp.Column == 2));
        }

        /// <summary>
        /// Тестирование построения индекса линии,
        ///     в которой встречаются все возможные направления линий
        /// </summary>
        /// <remarks>
        /// Тест должен показать что пространственный индекс работает
        ///     как для возрастающих по X или Y линий, так и для убывающих
        ///     во всех комбинациях
        /// </remarks>
        [TestMethod]
        public void ComplexIndexes2DTest()
        {
            var indexesService = new LineSpatialIndexesService(10, new Coordinate(0, 0));

            #region Возрастает по OY интенсивнее, чем убывает по OX

            var line = new List<Coordinate>
            {
                new Coordinate(25, 5),
                new Coordinate(5, 45)
            };
            var index = indexesService.GetLineSpatial2DIndexes(line);

            Assert.AreEqual(7, index.Count, "Неверное количество ячеек индекса");
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 2));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 2));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 2 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 3 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 3 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 4 && sp.Column == 0));

            #endregion

            #region  Возрастает по OY, не изменяется по OX

            line = new List<Coordinate>
            {
                new Coordinate(5, 5),
                new Coordinate(5, 45)
            };
            index = indexesService.GetLineSpatial2DIndexes(line);

            Assert.AreEqual(5, index.Count, "Неверное количество ячеек индекса");
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 2 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 3 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 4 && sp.Column == 0));

            #endregion

            #region Возрастает по OY интенсивнее, чем возрастает по OX

            line = new List<Coordinate>
            {
                new Coordinate(5, 5),
                new Coordinate(25, 45)
            };
            index = indexesService.GetLineSpatial2DIndexes(line);

            Assert.AreEqual(7, index.Count, "Неверное количество ячеек индекса");
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 2 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 3 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 3 && sp.Column == 2));
            Assert.IsTrue(index.Any(sp => sp.Row == 4 && sp.Column == 2));

            #endregion

            #region Убывает по OY интенсивнее, чем убывает по OX

            line = new List<Coordinate>
            {
                new Coordinate(25, 45),
                new Coordinate(5, 5)
            };
            index = indexesService.GetLineSpatial2DIndexes(line);

            Assert.AreEqual(7, index.Count, "Неверное количество ячеек индекса");
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 2 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 3 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 3 && sp.Column == 2));
            Assert.IsTrue(index.Any(sp => sp.Row == 4 && sp.Column == 2));

            #endregion

            #region Убывает по OY, не изменяется по OX

            line = new List<Coordinate>
            {
                new Coordinate(5, 45),
                new Coordinate(5, 5)
            };
            index = indexesService.GetLineSpatial2DIndexes(line);

            Assert.AreEqual(5, index.Count, "Неверное количество ячеек индекса");
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 2 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 3 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 4 && sp.Column == 0));

            #endregion

            #region Убывает по OY интенсивнее, чем возрастает по OX

            line = new List<Coordinate>
            {
                new Coordinate(5, 45),
                new Coordinate(25, 5)
            };
            index = indexesService.GetLineSpatial2DIndexes(line);

            Assert.AreEqual(7, index.Count, "Неверное количество ячеек индекса");
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 2));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 2));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 2 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 3 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 3 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 4 && sp.Column == 0));

            #endregion

            #region Возрастает по OX интенсивнее, чем возрастает по OY

            line = new List<Coordinate>
            {
                new Coordinate(5, 5),
                new Coordinate(45, 25)
            };
            index = indexesService.GetLineSpatial2DIndexes(line);

            Assert.AreEqual(7, index.Count, "Неверное количество ячеек индекса");
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 2));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 3));
            Assert.IsTrue(index.Any(sp => sp.Row == 2 && sp.Column == 3));
            Assert.IsTrue(index.Any(sp => sp.Row == 2 && sp.Column == 4));

            #endregion

            #region Возрастает по OX, не изменяется по OY

            line = new List<Coordinate>
            {
                new Coordinate(5, 5),
                new Coordinate(45, 5)
            };
            index = indexesService.GetLineSpatial2DIndexes(line);

            Assert.AreEqual(5, index.Count, "Неверное количество ячеек индекса");
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 2));
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 3));
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 4));

            #endregion

            #region Возрастает по OX интенсивнее, чем убывает по OY

            line = new List<Coordinate>
            {
                new Coordinate(5, 25),
                new Coordinate(45, 5)
            };
            index = indexesService.GetLineSpatial2DIndexes(line);

            Assert.AreEqual(7, index.Count, "Неверное количество ячеек индекса");
            Assert.IsTrue(index.Any(sp => sp.Row == 2 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 2 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 2));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 3));
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 3));
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 4));

            #endregion

            #region Убывает по OX интенсивнее, чем возрастает по OY

            line = new List<Coordinate>
            {
                new Coordinate(45, 5),
                new Coordinate(5, 25)
            };
            index = indexesService.GetLineSpatial2DIndexes(line);

            Assert.AreEqual(7, index.Count, "Неверное количество ячеек индекса");
            Assert.IsTrue(index.Any(sp => sp.Row == 2 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 2 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 2));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 3));
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 3));
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 4));

            #endregion

            #region Убывает по OX, не изменяется по OY

            line = new List<Coordinate>
            {
                new Coordinate(45, 5),
                new Coordinate(5, 5)
            };
            index = indexesService.GetLineSpatial2DIndexes(line);

            Assert.AreEqual(5, index.Count, "Неверное количество ячеек индекса");
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 2));
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 3));
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 4));

            #endregion

            #region Убывает по OX интенсивнее, чем убывает по OY

            line = new List<Coordinate>
            {
                new Coordinate(45, 25),
                new Coordinate(5, 5)
            };
            index = indexesService.GetLineSpatial2DIndexes(line);

            Assert.AreEqual(7, index.Count, "Неверное количество ячеек индекса");
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 2));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 3));
            Assert.IsTrue(index.Any(sp => sp.Row == 2 && sp.Column == 3));
            Assert.IsTrue(index.Any(sp => sp.Row == 2 && sp.Column == 4));

            #endregion
        }

        /// <summary>
        /// Тестирование построения индекса линии, 
        ///     когда граничные точки которой, лежат прямо на сетке пространственного индекса
        /// </summary>
        /// <remarks>
        /// Тест должен показать что все смежные ячейки попали в индекс. 
        /// </remarks>
        [TestMethod]
        public void PointOnGridIndexes2DTest()
        {
            var indexesService = new LineSpatialIndexesService(10, new Coordinate(0, 0));

            var line = new List<Coordinate>
            {
                new Coordinate(10, 5),
                new Coordinate(20, 5)
            };
            var index = indexesService.GetLineSpatial2DIndexes(line);

            Assert.AreEqual(3, index.Count, "Неверное количество ячеек индекса");
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 2));
        }

        /// <summary>
        /// Тестирование построения индекса линии,
        ///     когда граничные точки которой, лежат на пересечении линий
        ///     сетки пространственного индекса
        /// </summary>
        [TestMethod]
        public void PointOnCrossbreedIndexes2DTest()
        {
            var indexesService = new LineSpatialIndexesService(10, new Coordinate(0, 0));

            var line = new List<Coordinate>
            {
                new Coordinate(10, 10),
                new Coordinate(30, 30)
            };
            var index = indexesService.GetLineSpatial2DIndexes(line);

            Assert.AreEqual(10, index.Count, "Неверное количество ячеек индекса");
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 2));
            Assert.IsTrue(index.Any(sp => sp.Row == 2 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 2 && sp.Column == 2));
            Assert.IsTrue(index.Any(sp => sp.Row == 2 && sp.Column == 3));
            Assert.IsTrue(index.Any(sp => sp.Row == 3 && sp.Column == 2));
            Assert.IsTrue(index.Any(sp => sp.Row == 3 && sp.Column == 3));
        }
    }
}