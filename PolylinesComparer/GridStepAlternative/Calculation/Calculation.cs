using System.Collections.Generic;
using System.Linq;
using GridStepAlternative.Infrastructure;
using GridStepAlternative.Model;
using PolylinesComparer.Model;

namespace GridStepAlternative.Calculation
{
    public class Calculation
    {
        private readonly INodeService _nodeService;

        private readonly IСhainService _сhainService;

        private readonly IEntityService _entityService;

        private readonly IEdgeService _edgeService;

        public Calculation(INodeService nodeService, IСhainService сhainService, IEntityService entityService, IEdgeService edgeService)
        {
            _nodeService = nodeService;
            _сhainService = сhainService;
            _entityService = entityService;
            _edgeService = edgeService;
        }

        /// <summary>
        /// Расчитывает матрицы связности для списка сущностей
        /// </summary>
        public List<Map> GetMaps()
        {
            var entitys = _entityService.GetEntitys();
            return entitys.Select(entity => new Map {Entity = entity, Edges = GetEntityMap(entity.Id)}).ToList();
        }

        private List<Edge>[,] GetEntityMap(int entityId)
        {
            var nodes = _nodeService.GetNodesByEntity(entityId);
            var сhains = _сhainService.GetСhainByEntity(entityId);

            // Заполнение матрицы связности
            var result = new List<Edge>[nodes.Count, nodes.Count];
            foreach (var chain in сhains)
            {
                var chainNodes = _nodeService.GetNodesByChain(entityId, chain);

                #region  Обход последовательно связанных узлов для заполнения матрицы связности

                // Метонахождение (индекс) узла в списке всех узлов последовательности
                var nodeIndex = new List<int>();

                // Координаты ребра между узлами
                var coords = new List<List<Coordinate>>();

                for (int i = 0; i < chainNodes.Count; i++)
                {
                    // Найдём индекс узла в общем списке
                    var ij = nodes.FindIndex(r => r == chainNodes[i]);
                    if (ij == -1)
                        continue;

                    // Получим точки из которых строится участок
                    if (nodeIndex.Any())
                        coords.Add(_edgeService.GetCoord(chainNodes[i - 1], chainNodes[i], chain, entityId));

                    // Из всех узлов, найденных ранее до текущего можно добраться напрямую
                    int startNumber = 0;
                    foreach (var ii in nodeIndex)
                    {
                        // Агрегировать координаты нескольких сегментов, для получения одного
                        var newCoords = AgregatePoints(coords, startNumber);
                        startNumber++;

                        if (newCoords.Any())
                        {
                            if (result[ii, ij] == null)
                                result[ii, ij] = new List<Edge>();
                            result[ii, ij].Add(new Edge {Coordinates = newCoords});
                        }
                    }

                    if (!nodeIndex.Contains(ij))
                        nodeIndex.Add(ij);
                }

                #endregion
            }
            return result;
        }

        /// <summary>
        /// Объединяет массивы координат
        /// </summary>
        /// <param name="coords">Список массивов координат</param>
        /// <param name="startIndex">номер массива с которого начинается объединение</param>
        /// <returns>Массив координат</returns>
        private List<Coordinate> AgregatePoints(List<List<Coordinate>> coords, int startIndex)
        {
            var newCoords = new List<Coordinate>();
            for (int c = startIndex; c < coords.Count; c++)
                if (coords[c] != null)
                    newCoords.AddRange(coords[c]);
            return newCoords;
        }
    }
}