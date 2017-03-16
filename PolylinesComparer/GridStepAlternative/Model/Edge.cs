using System.Collections.Generic;
using PolylinesComparer.Model;

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
        /// <summary>
        /// Координаты сегмента
        /// </summary>
        public List<Coordinate> Coordinates;
    }
}