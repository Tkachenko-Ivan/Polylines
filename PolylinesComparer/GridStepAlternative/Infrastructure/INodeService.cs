using System.Collections.Generic;
using GridStepAlternative.Model;

namespace GridStepAlternative.Infrastructure
{
    /// <summary>
    /// Контракт для получения вершин графа
    /// </summary>
    /// <remarks>
    /// Вершиной графа может быть, например остановка
    /// </remarks>
    public interface INodeService
    {
        List<Node> GetNodesByEntity(int entityId);

        List<Node> GetNodesByChain(int chainId);
    }
}