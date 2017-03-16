using System.Collections.Generic;

namespace GridStepAlternative.Model
{
    /// <summary>
    /// Граф
    /// </summary>
    public class Map
    {
        public Entity Entity { get; set; }

        /// <summary>
        /// Списко рёбер графа соединяющих вершины
        /// </summary>
        public List<Edge>[,] Edges { get; set; }
    }
}