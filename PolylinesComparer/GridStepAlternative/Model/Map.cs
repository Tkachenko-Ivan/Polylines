using System.Collections.Generic;

namespace GridStepAlternative.Model
{
    public class Map
    {
        public Entity Entity { get; set; }

        public List<Edge>[,] Edges { get; set; }
    }
}