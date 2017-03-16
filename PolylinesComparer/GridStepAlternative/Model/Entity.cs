using PolylinesComparer.Model;

namespace GridStepAlternative.Model
{
    public class Entity
    {
        public int Id;

        public string Name;

        /// <summary>
        /// Координата центра
        /// </summary>
        /// <remarks>
        /// Необязательно выбирать геометрический центр, 
        ///   важно лишь то, что эта точка будет считаться началом координат 
        ///   при сравнении линий принадлежащих сущности
        /// </remarks>
        public Coordinate Center;
    }
}