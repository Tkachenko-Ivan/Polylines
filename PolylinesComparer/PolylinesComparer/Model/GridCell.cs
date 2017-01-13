namespace PolylinesComparer.Model
{
    /// <summary>
    /// Ячейка пространственного индекса
    /// </summary>
    internal class GridCell
    {
        public GridCell(int row, int column)
        {
            Row = row;
            Column = column;
            Layer = 0;
        }

        public GridCell(int row, int column, int layer)
        {
            Row = row;
            Column = column;
            Layer = layer;
        }

        public int Row { get; private set; }

        public int Column { get; private set; }

        public int Layer { get; private set; }
    }
}
