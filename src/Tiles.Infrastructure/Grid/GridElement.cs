using System.Collections.Generic;
using System.Linq;

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

    public class Grid {

        private GridElement[] Elements {get;set;}

        public Grid(IEnumerable<GridElement> elements) {
            Elements = elements.ToArray();
        }

        public IEnumerable<GridElement> GetElements() {
            return Elements;
        }
    }
}