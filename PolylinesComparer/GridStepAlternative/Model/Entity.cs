using System.Collections.Generic;

namespace GridStepAlternative.Model
{
    public class Entity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Координата цента
        /// </summary>
        /// <remarks>
        /// Необягательно выбирать географический центр, 
        ///   важно лишь то, что эта точка будет считаться началом координат 
        ///   при сравнении линий принадлежащих сущности
        /// </remarks>
        public Coordinate Center { get; set; }

        public List<Edge>[,] Map { get; set; }
}
}