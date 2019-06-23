using System.Drawing;

namespace Tiles.Infrastructure.Grid
{
    public class GridElement
    {
      public GridElement(ICoordinate coordinate, Image value)
        {
          Coordinate = coordinate;
          Value = value;
        }

      public ICoordinate Coordinate { get; }
      public Image Value { get;}
    }
}