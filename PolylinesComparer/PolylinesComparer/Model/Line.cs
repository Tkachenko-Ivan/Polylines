using System.Collections.Generic;
using System.Linq;

namespace PolylinesComparer.Model
{
    public class Line
    {
        public List<Coordinate> Coordinates { get; set; }

        public bool HasH { get; set; }

        public int Count
        {
            get
            {
                return Coordinates.Count;
            }
        }
    }
}
