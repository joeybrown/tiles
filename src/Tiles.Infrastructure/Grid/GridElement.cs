using System.Drawing;

namespace Tiles.Infrastructure.Grid
{
    public class GridElement
    {
      public GridElement(ICoordinate coordinate, Bitmap value)
        {
          Coordinate = coordinate;
          Value = value;
        }

      public ICoordinate Coordinate { get; }
      public Bitmap Value { get;}
    }
}