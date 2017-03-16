using System.Collections.Generic;
using PolylinesComparer.Model;

namespace GridStepAlternative.Infrastructure
{
    /// <summary>
    /// Контракт для получения информации о рёбрах
    /// </summary>
    /// <remarks>
    /// Ребра создаются если, например, от одной остановки до другой можно проехать без пересадок
    /// </remarks>
    public interface IEdgeService
    {
        List<Coordinate> GetCoord(int nodeStart, int nodeEnd, int chainId, int entityId);
    }
}