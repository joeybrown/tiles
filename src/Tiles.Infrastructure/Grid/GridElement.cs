namespace Tiles.Infrastructure.Grid
{
    public class GridElement
    {
        public GridElement(int coordX, int coordY, byte[] value)
        {
            CoordX = coordX;
            CoordY = coordY;
            Value = value;
        }
        public int CoordX { get; set; }
        public int CoordY { get; set; }
        public byte[] Value { get; set; }
    }
}