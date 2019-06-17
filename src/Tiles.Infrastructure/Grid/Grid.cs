using System.Collections.Generic;
using System.Linq;

namespace Tiles.Infrastructure.Grid
{
  public class Grid
  {

    private GridElement[] Elements { get; set; } = new GridElement[0];

    public Grid(IEnumerable<GridElement> elements)
    {
      Elements = elements.ToArray();
    }

    public Grid()
    {
    }

    public IEnumerable<GridElement> GetElements()
    {
      return Elements;
    }
  }
}