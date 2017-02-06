using System.Collections.Generic;

namespace GridStepAlternative.Model
{
    /// <summary>
    /// Ребро графа, соединяющее два его узла
    /// </summary>
    /// <remarks>
    /// Ребром графа может быть, например, участок маршрута связывающий остановки,
    ///    ! для каждого маршрута новое ребро !
    /// </remarks>
    public class Edge
    {
        /*
        /// <summary>
        /// Узел - исток
        /// </summary>
        public Node StartNode;

        /// <summary>
        /// Узел - сток
        /// </summary>
        public Node EndNode;
        */

        /// <summary>
        /// Координаты сегмента
        /// </summary>
        public List<Coordinate> Coordinates;
    }
}