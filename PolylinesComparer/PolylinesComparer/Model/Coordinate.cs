namespace PolylinesComparer.Model
{
    public class Coordinate
    {
        public Coordinate(double lon, double lat)
        {
            Lon = lon;
            Lat = lat;
            H = 0;
        }

        public Coordinate(double lon, double lat, double h)
        {
            Lon = lon;
            Lat = lat;
            H = h;
        }

        /// <summary>
        /// X - долгота
        /// </summary>
        public double Lon { get; private set; }

        /// <summary>
        /// Y - широта
        /// </summary>
        public double Lat { get; private set; }

        /// <summary>
        /// Высота над уровнем моря
        /// </summary>
        public double H { get; private set; }
    }
}
