using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolylinesComparer;
using PolylinesComparer.Model;

namespace PolylinesComparerTests
{
    /// <summary>
    /// Тестирование построения индекса для значений лежащих в отрицательной зоне пространственного индекса
    /// </summary>
    [TestClass]
    public class NegativeIndexTests
    {
        /// <summary>
        /// Тест на построение индекса для значений меньше чем начало координат.
        /// Координаты как положительные так и отрицательные.
        /// Начало системы координта положительное число.
        /// </summary>
        [TestMethod]
        public void NegativeIndexPositiveOriginTest2D()
        {
            // Линия возрастает по X
            var indexesService = new LineSpatialIndexesService(10, new Coordinate(20, 20));
            var line = new List<Coordinate>
            {
                new Coordinate(-19, -19),
                new Coordinate(89, 59)
            };
            var index = indexesService.GetLineSpatial2DIndexes(line);
            Assert.AreEqual(18, index.Count, "Неверное количество ячеек индекса");
            Assert.IsTrue(index.Any(sp => sp.Row == -4 && sp.Column == -4));
            Assert.IsTrue(index.Any(sp => sp.Row == -4 && sp.Column == -3));
            Assert.IsTrue(index.Any(sp => sp.Row == -3 && sp.Column == -3));
            Assert.IsTrue(index.Any(sp => sp.Row == -3 && sp.Column == -2));
            Assert.IsTrue(index.Any(sp => sp.Row == -2 && sp.Column == -2));
            Assert.IsTrue(index.Any(sp => sp.Row == -2 && sp.Column == -1));
            Assert.IsTrue(index.Any(sp => sp.Row == -2 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == -1 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == -1 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 2));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 2));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 3));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 4));
            Assert.IsTrue(index.Any(sp => sp.Row == 2 && sp.Column == 4));
            Assert.IsTrue(index.Any(sp => sp.Row == 2 && sp.Column == 5));
            Assert.IsTrue(index.Any(sp => sp.Row == 3 && sp.Column == 5));
            Assert.IsTrue(index.Any(sp => sp.Row == 3 && sp.Column == 6));

            // Линия убывает по X
            indexesService = new LineSpatialIndexesService(10, new Coordinate(20, 20));
            line = new List<Coordinate>
            {
                new Coordinate(89, 59),
                new Coordinate(-19, -19)
            };
            index = indexesService.GetLineSpatial2DIndexes(line);
            Assert.AreEqual(18, index.Count, "Неверное количество ячеек индекса");
            Assert.IsTrue(index.Any(sp => sp.Row == -4 && sp.Column == -4));
            Assert.IsTrue(index.Any(sp => sp.Row == -4 && sp.Column == -3));
            Assert.IsTrue(index.Any(sp => sp.Row == -3 && sp.Column == -3));
            Assert.IsTrue(index.Any(sp => sp.Row == -3 && sp.Column == -2));
            Assert.IsTrue(index.Any(sp => sp.Row == -2 && sp.Column == -2));
            Assert.IsTrue(index.Any(sp => sp.Row == -2 && sp.Column == -1));
            Assert.IsTrue(index.Any(sp => sp.Row == -2 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == -1 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == -1 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 2));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 2));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 3));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 4));
            Assert.IsTrue(index.Any(sp => sp.Row == 2 && sp.Column == 4));
            Assert.IsTrue(index.Any(sp => sp.Row == 2 && sp.Column == 5));
            Assert.IsTrue(index.Any(sp => sp.Row == 3 && sp.Column == 5));
            Assert.IsTrue(index.Any(sp => sp.Row == 3 && sp.Column == 6));
        }

        /// <summary>
        /// Тест на построение индекса для значений меньше чем начало координат в 3D пространстве.
        /// Координаты как положительные так и отрицательные.  
        /// Начало системы координта положительное число.
        /// </summary>
        /// <remarks>
        /// Решил упростить тест, чтобы не сравнивать безумное количество ячеек, 
        ///     а просто проверить правильно ли опредеились слои по z 
        /// </remarks>
        [TestMethod]
        public void NegativeIndexPositiveOriginTest3D()
        {
            var indexesService = new LineSpatialIndexesService(20, new Coordinate(20, 20, 20));
            var line = new List<Coordinate>
            {
                new Coordinate(-19, -19, -9),
                new Coordinate(89, 59, 79)
            };

            var index = indexesService.GetLineSpatial3DIndexes(line);
            var rows = index.Select(ind => ind.Layer).Distinct().ToList();
            Assert.AreEqual(5, rows.Count, "Неверное количество слоёв");
            Assert.IsTrue(rows.Any(sp => sp == -2));
            Assert.IsTrue(rows.Any(sp => sp == -1));
            Assert.IsTrue(rows.Any(sp => sp == 0));
            Assert.IsTrue(rows.Any(sp => sp == 1));
            Assert.IsTrue(rows.Any(sp => sp == 2));
        }

        /// <summary>
        /// Тест на построение индекса для значений меньше чем начало координат.
        /// Координаты как положительные так и отрицательные.  
        /// Начало системы координта отрицательное число.
        /// </summary>
        [TestMethod]
        public void NegativeIndexNegativeOriginTest2D()
        {
            // Отрицательные координаты - линия возрастает по X
            var indexesService = new LineSpatialIndexesService(10, new Coordinate(-20, -20));
            var line = new List<Coordinate>
            {
                new Coordinate(-59, -59),
                new Coordinate(49, 19)
            };
            var index = indexesService.GetLineSpatial2DIndexes(line);
            Assert.AreEqual(18, index.Count, "Неверное количество ячеек индекса");
            Assert.IsTrue(index.Any(sp => sp.Row == -4 && sp.Column == -4));
            Assert.IsTrue(index.Any(sp => sp.Row == -4 && sp.Column == -3));
            Assert.IsTrue(index.Any(sp => sp.Row == -3 && sp.Column == -3));
            Assert.IsTrue(index.Any(sp => sp.Row == -3 && sp.Column == -2));
            Assert.IsTrue(index.Any(sp => sp.Row == -2 && sp.Column == -2));
            Assert.IsTrue(index.Any(sp => sp.Row == -2 && sp.Column == -1));
            Assert.IsTrue(index.Any(sp => sp.Row == -2 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == -1 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == -1 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 2));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 2));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 3));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 4));
            Assert.IsTrue(index.Any(sp => sp.Row == 2 && sp.Column == 4));
            Assert.IsTrue(index.Any(sp => sp.Row == 2 && sp.Column == 5));
            Assert.IsTrue(index.Any(sp => sp.Row == 3 && sp.Column == 5));
            Assert.IsTrue(index.Any(sp => sp.Row == 3 && sp.Column == 6));

            // Отрицательные координаты - линия убывает по X
            indexesService = new LineSpatialIndexesService(10, new Coordinate(-20, -20));
            line = new List<Coordinate>
            {
                new Coordinate(49, 19),
                new Coordinate(-59, -59)
            };
            index = indexesService.GetLineSpatial2DIndexes(line);
            Assert.AreEqual(18, index.Count, "Неверное количество ячеек индекса");
            Assert.IsTrue(index.Any(sp => sp.Row == -4 && sp.Column == -4));
            Assert.IsTrue(index.Any(sp => sp.Row == -4 && sp.Column == -3));
            Assert.IsTrue(index.Any(sp => sp.Row == -3 && sp.Column == -3));
            Assert.IsTrue(index.Any(sp => sp.Row == -3 && sp.Column == -2));
            Assert.IsTrue(index.Any(sp => sp.Row == -2 && sp.Column == -2));
            Assert.IsTrue(index.Any(sp => sp.Row == -2 && sp.Column == -1));
            Assert.IsTrue(index.Any(sp => sp.Row == -2 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == -1 && sp.Column == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == -1 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 2));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 2));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 3));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 4));
            Assert.IsTrue(index.Any(sp => sp.Row == 2 && sp.Column == 4));
            Assert.IsTrue(index.Any(sp => sp.Row == 2 && sp.Column == 5));
            Assert.IsTrue(index.Any(sp => sp.Row == 3 && sp.Column == 5));
            Assert.IsTrue(index.Any(sp => sp.Row == 3 && sp.Column == 6));
        }

        /// <summary>
        /// Тест на построение индекса для значений меньше чем начало координат в 3D пространстве.
        /// Координаты как положительные так и отрицательные.  
        /// Начало системы координта отрицательное число.
        /// </summary>
        /// <remarks>
        /// Решил упростить тест, чтобы не сравнивать безумное количество ячеек, 
        ///     а просто проверить правильно ли опредеились слои по z 
        /// </remarks>
        [TestMethod]
        public void NegativeIndexNegativeOriginTest3D()
        {
            var indexesService = new LineSpatialIndexesService(20, new Coordinate(-20, -20, -20));
            var line = new List<Coordinate>
            {
                new Coordinate(49, 19, 39),
                new Coordinate(-59, -59, -49)
            };

            var index = indexesService.GetLineSpatial3DIndexes(line);
            var rows = index.Select(ind => ind.Layer).Distinct().ToList();
            Assert.AreEqual(5, rows.Count, "Неверное количество слоёв");
            Assert.IsTrue(rows.Any(sp => sp == -2));
            Assert.IsTrue(rows.Any(sp => sp == -1));
            Assert.IsTrue(rows.Any(sp => sp == 0));
            Assert.IsTrue(rows.Any(sp => sp == 1));
            Assert.IsTrue(rows.Any(sp => sp == 2));
        }
    }
}