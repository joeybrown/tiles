using System.Collections.Generic;
using System.Linq;

namespace Tiles.Infrastructure.Grid
{
  public class Grid
  {
    private int TileWidth = 0;
    private GridElement[] Elements { get; } = new GridElement[0];

    public Grid(IEnumerable<GridElement> elements, int tileWidth)
    {
      Elements = elements.ToArray();
      TileWidth = tileWidth;
    }

    public Grid()
    {
    }

    public IEnumerable<GridElement> GetElements() => 
      Elements.OrderBy(x=>x.Coordinate.X).ThenBy(x=>x.Coordinate.Y);
  }
}