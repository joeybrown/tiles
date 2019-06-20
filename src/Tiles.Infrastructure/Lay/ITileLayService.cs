using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Infrastructure.Grid;

namespace Tiles.Infrastructure.Lay
{
  public interface ITileLayService
  {
    Task<Image> LayTile(Grid.Grid grid);
  }

  public class TileLayService : ITileLayService
  {
    public static void CopyRegionIntoImage(Bitmap srcBitmap, Rectangle srcRegion, ref Bitmap destBitmap, Rectangle destRegion)
    {
      using (var grD = Graphics.FromImage(destBitmap))
      {
        grD.DrawImage(srcBitmap, destRegion, srcRegion, GraphicsUnit.Pixel);
      }
    }

    /// <inheritdoc />
    public async Task<Image> LayTile(Grid.Grid grid)
    {
      var elements = grid.GetElements().ToArray();
      if (!elements.Any())
        throw new Exception("No tiles to lay. Cannot return empty image");
      var tileWidth = new Bitmap(elements.First().Value).Width;
      var width = elements.Select(x => x.Coordinate.X).Max() * tileWidth;
      var height = elements.Select(x => x.Coordinate.Y).Max() * tileWidth;
      var image = new Bitmap(width, height);
      var result = await LayTile(elements, tileWidth, image);
      return result;
    }

    public async Task<Image> LayTile(IEnumerable<GridElement> gridElements, int tileWidth, Image collector)
    {
      if (!gridElements.Any())
        return collector;
      await Task.Delay(500);

    };
  }
}
