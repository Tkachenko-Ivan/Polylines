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
        /// Тестируется провильность формирвоания групп, в том числе для одиночных узлов
        /// </summary>
        /// <remarks>
        /// Тест должен показать что одиноко лежащие элемены не были потеряны, и все кластеры были учтены
        /// </remarks>
        [TestMethod]
        public void SimpleTest()
        {
            var indexesService = new IndexesNumberService();

            var line1 = new List<Coordinate> { new Coordinate(15, 2), new Coordinate(45, 2) };
            var line2 = new List<Coordinate> { new Coordinate(25, 2), new Coordinate(55, 2) };
            var line3 = new List<Coordinate> { new Coordinate(15, 2), new Coordinate(45, 2) };
            var line4 = new List<Coordinate> { new Coordinate(25, 2), new Coordinate(45, 2) };
            var line5 = new List<Coordinate> { new Coordinate(25, 25), new Coordinate(55, 25) };
            var line6 = new List<Coordinate> { new Coordinate(5, 2), new Coordinate(35, 2) };
            var line7 = new List<Coordinate> { new Coordinate(55, 25), new Coordinate(85, 25) };
            var line8 = new List<Coordinate> { new Coordinate(0, 2), new Coordinate(25, 2) };
            var line9 = new List<Coordinate> { new Coordinate(35, 2), new Coordinate(65, 2) };

            var index =
                indexesService.DifferentIndexesNumber2D(
                    new List<List<Coordinate>> { line1, line2, line3, line4, line5, line6, line7, line8, line9 }, 10, 0.6, new Coordinate(0, 0));
            Assert.AreEqual(6, index, "Неверное количество групп");
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
            Assert.AreEqual(3, index, "Неверное количество групп");

            // Обход завершается группой которая полностью пересекается другими группами
            index =
                indexesService.DifferentIndexesNumber2D(
                    new List<List<Coordinate>> { line5, line6, line1, line2, line3, line4 }, 10, 0.6, new Coordinate(0, 0));
            Assert.AreEqual(3, index, "Неверное количество групп");
        }


        /// <summary>
        /// Тестируется подсчёт групп в случае, когда 
        ///   объект с наибольшим количеством пересечений стоит на первом месте
        /// </summary>
        /// <remarks>
        /// Тест должен показать, что порядок следования объектов не имеет значения,
        ///     и что объект который попадает в несколько разных групп, не образует новую группу, 
        ///     по причине уникального набора пересечений
        /// </remarks>
        [TestMethod]
        public void IndexesIntersectionMaximalTest()
        {
            var indexesService = new IndexesNumberService();

            var line1 = new List<Coordinate> { new Coordinate(15, 2), new Coordinate(45, 2) };
            var line2 = new List<Coordinate> { new Coordinate(25, 2), new Coordinate(55, 2) };
            var line3 = new List<Coordinate> { new Coordinate(25, 2), new Coordinate(55, 2) };
            var line4 = new List<Coordinate> { new Coordinate(5, 2), new Coordinate(35, 2) };
            var index =
                indexesService.DifferentIndexesNumber2D(
                    new List<List<Coordinate>> { line1, line2, line3, line4 }, 10, 0.6, new Coordinate(0, 0));
            Assert.AreEqual(2, index, "Неверное количество групп");
        }

        /// <summary>
        /// Тест на подсчёт групп, в случае, когда каждый объект имеет уникальный набор из нескольких групп
        /// </summary>
        [TestMethod]
        public void UniqueGroupTest()
        {
            bool[,] matrix = new bool[4, 4];

            matrix[0, 0] = true;
            matrix[0, 1] = true;
            matrix[0, 3] = true;

            matrix[1, 0] = true;
            matrix[1, 1] = true;
            matrix[1, 2] = true;

            matrix[2, 1] = true;
            matrix[2, 2] = true;
            matrix[2, 3] = true;

            matrix[3, 0] = true;
            matrix[3, 2] = true;
            matrix[3, 3] = true;

            var indexesNumberService = new IndexesNumberService();
            var privateObject = new PrivateObject(indexesNumberService);
            var result = privateObject.Invoke("Distaff", matrix);

            Assert.AreEqual(4, result, "Неверное количество групп");
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
            Assert.AreEqual(0, index, "Неверное количество групп");
        }

        /// <summary>
        /// Тест на работу с одним элементом
        /// </summary>
        [TestMethod]
        public void UnoElementTest()
        {
            var indexesService = new IndexesNumberService();
            var line1 = new List<Coordinate> { new Coordinate(15, 2), new Coordinate(45, 2) };
            var index =
                indexesService.DifferentIndexesNumber2D(
                    new List<List<Coordinate>> { line1 }, 10, 0.6, new Coordinate(0, 0));
            Assert.AreEqual(1, index, "Неверное количество групп");
        }

        /// <summary>
        /// Тест обработки списка элементов, которые составляют лишь одиночные группы
        /// </summary>
        [TestMethod]
        public void SingleGroupTest()
        {
            bool[,] matrix = new bool[4, 4];

            matrix[0, 0] = true;
            matrix[1, 1] = true;
            matrix[2, 2] = true;
            matrix[3, 3] = true;

            var indexesNumberService = new IndexesNumberService();
            var privateObject = new PrivateObject(indexesNumberService);
            var result = privateObject.Invoke("Distaff", matrix);

            Assert.AreEqual(4, result, "Неверное количество групп");
        }

        /// <summary>
        /// Тест обработки списка элементов, которые образуют одну группу
        /// </summary>
        [TestMethod]
        public void UnoGroupTest()
        {
            bool[,] matrix = new bool[4, 4];

            matrix[0, 0] = true;
            matrix[0, 1] = true;
            matrix[0, 2] = true;
            matrix[0, 3] = true;

            matrix[1, 0] = true;
            matrix[1, 1] = true;
            matrix[1, 2] = true;
            matrix[1, 3] = true;

            matrix[2, 0] = true;
            matrix[2, 1] = true;
            matrix[2, 2] = true;
            matrix[2, 3] = true;

            matrix[3, 0] = true;
            matrix[3, 1] = true;
            matrix[3, 2] = true;
            matrix[3, 3] = true;

            var indexesNumberService = new IndexesNumberService();
            var privateObject = new PrivateObject(indexesNumberService);
            var result = privateObject.Invoke("Distaff", matrix);

            Assert.AreEqual(1, result, "Неверное количество групп");
        }

        [TestMethod]
        public void CloverTest()
        {
            bool[,] matrix = new bool[7, 7];

            matrix[0, 0] = true;
            matrix[0, 1] = true;
            matrix[0, 2] = true;
            matrix[0, 5] = true;

            matrix[1, 0] = true;
            matrix[1, 1] = true;
            matrix[1, 2] = true;
            matrix[1, 3] = true;
            matrix[1, 4] = true;
            matrix[1, 5] = true;
            matrix[1, 6] = true;

            matrix[2, 0] = true;
            matrix[2, 1] = true;
            matrix[2, 2] = true;

            matrix[3, 1] = true;
            matrix[3, 3] = true;
            matrix[3, 4] = true;
            matrix[3, 6] = true;

            matrix[4, 1] = true;
            matrix[4, 3] = true;
            matrix[4, 4] = true;

            matrix[5, 0] = true;
            matrix[5, 1] = true;
            matrix[5, 5] = true;

            matrix[6, 1] = true;
            matrix[6, 3] = true;
            matrix[6, 6] = true;

            var indexesNumberService = new IndexesNumberService();
            var privateObject = new PrivateObject(indexesNumberService);
            var result = privateObject.Invoke("Distaff", matrix);

            Assert.AreEqual(4, result, "Неверное количество групп");
        }
    }
}