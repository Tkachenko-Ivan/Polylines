using System.Collections.Generic;
using GridStepAlternative.Model;

namespace GridStepAlternative.Infrastructure
{
    /// <summary>
    /// Контракт для получения связной последовательности рёбер графа
    /// </summary>
    /// <remarks>
    /// Последоватлеьностью может быть, например, автобусный маршрут последовательно соединяющий свои остановки
    /// </remarks>
    public interface IСhainService
    {
        List<Сhain> GetСhainByEntity(int entityId);
    }
}