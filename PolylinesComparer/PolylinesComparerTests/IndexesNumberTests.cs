using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolylinesComparer;
using PolylinesComparer.Model;

namespace PolylinesComparerTests
{
    /// <summary>
    /// Тестирование определения количества уникальных полилиний
    /// </summary>
    [TestClass]
    public class IndexesNumberTests
    {
        /// <summary>
        /// Тестируется провильность формирвоания кластеров, в том числе для одиночных узлов
        /// </summary>
        /// <remarks>
        /// Тест должен показать что одиноко лежащие элемены не были потеряны, и все кластеры были учтены
        /// </remarks>
        [TestMethod]
        public void SimpleTest()
        {
            var indexesService = new IndexesNumberService();

            var line1 = new List<Coordinate> { new Coordinate(15, 2), new Coordinate(45, 2) };
            var line2 = new List<Coordinate> { new Coordinate(25, 2), new Coordinate(45, 2) };
            var line3 = new List<Coordinate> { new Coordinate(15, 2), new Coordinate(45, 2) };
            var line4 = new List<Coordinate> { new Coordinate(25, 2), new Coordinate(55, 2) };
            var line5 = new List<Coordinate> { new Coordinate(25, 25), new Coordinate(55, 25) };
            var line6 = new List<Coordinate> { new Coordinate(5, 2), new Coordinate(35, 2) };
            var line7 = new List<Coordinate> { new Coordinate(55, 25), new Coordinate(85, 25) };
            var line8 = new List<Coordinate> { new Coordinate(0, 2), new Coordinate(25, 2) };
            var line9 = new List<Coordinate> { new Coordinate(35, 2), new Coordinate(65, 2) };

            var index =
                indexesService.DifferentIndexesNumber2D(
                    new List<List<Coordinate>> { line1, line2, line3, line4, line5, line6, line7, line8, line9 }, 10, 0.6, new Coordinate(0, 0));
            Assert.AreEqual(6, index, "Неверное количество кластеров");
        }

        /// <summary>
        /// Тест для проверки работы при условии что все объекты одной из группы входят также в другие группы
        /// </summary>
        /// <remarks>
        /// Тест должен показать что группировка прошла правильно и группа не была потеряна
        /// </remarks>
        [TestMethod]
        public void IndexesIntersectionTest()
        {
            var indexesService = new IndexesNumberService();

            // Обход начинается с группы которая полностью пересекается другими группами
            var line1 = new List<Coordinate> {new Coordinate(15, 2), new Coordinate(45, 2)};
            var line2 = new List<Coordinate> {new Coordinate(25, 2), new Coordinate(55, 2)};
            var line3 = new List<Coordinate> {new Coordinate(15, 2), new Coordinate(45, 2)};
            var line4 = new List<Coordinate> {new Coordinate(25, 2), new Coordinate(55, 2)};
            var line5 = new List<Coordinate> {new Coordinate(5, 2), new Coordinate(35, 2)};
            var line6 = new List<Coordinate> {new Coordinate(35, 2), new Coordinate(65, 2)};
            var index =
                indexesService.DifferentIndexesNumber2D(
                    new List<List<Coordinate>> {line1, line2, line3, line4, line5, line6}, 10, 0.6, new Coordinate(0, 0));
            Assert.AreEqual(3, index, "Неверное количество кластеров");

            // Обход завершается группой которая полностью пересекается другими группами
            index =
                indexesService.DifferentIndexesNumber2D(
                    new List<List<Coordinate>> { line5, line6, line1, line2, line3, line4 }, 10, 0.6, new Coordinate(0, 0));
            Assert.AreEqual(3, index, "Неверное количество кластеров");
        }

        /// <summary>
        /// Тест на работу с пустым массивом
        /// </summary>
        /// <remarks>
        /// Тестируется то, что алгоритм не свалится
        /// </remarks>
        [TestMethod]
        public void NullTest()
        {
            var indexesService = new IndexesNumberService();
            var index =
                indexesService.DifferentIndexesNumber2D(
                    new List<List<Coordinate>> (), 10, 0.6, new Coordinate(0, 0));
            Assert.AreEqual(0, index, "Неверное количество кластеров");
        }

        /// <summary>
        /// Тест на работу с одним элементом
        /// </summary>
        /// <remarks>
        /// Тестируется то, что алгоритм работает там где сравнивать не с чем
        /// </remarks>
        [TestMethod]
        public void UnoTest()
        {
            var indexesService = new IndexesNumberService();
            var line1 = new List<Coordinate> { new Coordinate(15, 2), new Coordinate(45, 2) };
            var index =
                indexesService.DifferentIndexesNumber2D(
                    new List<List<Coordinate>> { line1 }, 10, 0.6, new Coordinate(0, 0));
            Assert.AreEqual(1, index, "Неверное количество кластеров");
        }
    }
}