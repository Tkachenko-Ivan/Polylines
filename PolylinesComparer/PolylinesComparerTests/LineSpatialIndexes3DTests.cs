using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolylinesComparer;
using PolylinesComparer.Model;

namespace PolylinesComparerTests
{
    /// <summary>
    /// Тестирование создания пространственных индексов
    /// </summary>
    [TestClass]
    public class LineSpatialIndexes3DTests
    {
        /// <summary>
        /// Простой тест на построение пространственного индекса
        /// </summary>
        /// <remarks>
        /// Тест должен показать что оно вообще работает, и что не появляется избыточных записей
        /// </remarks>
        [TestMethod]
        public void SimpleIndexes3DTest()
        {
            var indexesService = new LineSpatialIndexesService(10, new Coordinate(0, 0, 0));

            var line = new List<Coordinate> { new Coordinate(5, 5, 5), new Coordinate(15, 18, 25) };
            var index = indexesService.GetLineSpatial3DIndexes(line);

            Assert.AreEqual(5, index.Count, "Неверное количество ячеек индекса");
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 0 && sp.Layer == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 0 && sp.Layer == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 0 && sp.Layer == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 1 && sp.Layer == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 1 && sp.Layer == 2));
        }

        /// <summary>
        /// Тест на то, как строится индекс, если наибольшее изменение происходит по оси OX,
        ///     однако, начальная точка расположена так, что первое перечечение происходит
        ///     с плоскостью параллельной OXZ (вместо OYZ, как в идеальном случае)
        /// </summary>
        /// <remarks>
        /// Тест должен показать что ничего не выпадает из индекса, при соединении проекций
        /// </remarks>
        [TestMethod]
        public void IntersectIndexes3DTest()
        {
            var indexesService = new LineSpatialIndexesService(10, new Coordinate(0, 0, 0));

            var line = new List<Coordinate> { new Coordinate(4, 8, 7), new Coordinate(16, 14, 14) };
            var index = indexesService.GetLineSpatial3DIndexes(line);

            Assert.AreEqual(4, index.Count, "Неверное количество ячеек индекса");
            Assert.IsTrue(index.Any(sp => sp.Row == 0 && sp.Column == 0 && sp.Layer == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 0 && sp.Layer == 0));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 0 && sp.Layer == 1));
            Assert.IsTrue(index.Any(sp => sp.Row == 1 && sp.Column == 1 && sp.Layer == 1));
        }
    }
}