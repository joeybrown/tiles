using System.Collections.Generic;
using System.Linq;

namespace Tiles.Infrastructure.Grid
{
  public class Grid
  {
    public int Width { get; }
    public int Height { get; }
    private GridElement[] Elements { get; } = new GridElement[0];

    public Grid(IEnumerable<GridElement> elements, int width, int height)
    {
      Width = width;
      Height = height;
      Elements = elements.ToArray();
    }

    public Grid()
    {
    }

    public IEnumerable<GridElement> GetElements() => 
      Elements.OrderBy(x=>x.Coordinate.X).ThenBy(x=>x.Coordinate.Y);
  }
}