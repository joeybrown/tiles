using System.Drawing;
using System.Linq;
using Tiles.Infrastructure.Grid;

namespace Tiles.Infrastructure.Lay
{
  public interface ITileLayService
  {
    Bitmap LayTile(GridElement[] elements);

  }

  public class TileLayService : ITileLayService
  {
    public static Bitmap CopyRegionIntoImage(Bitmap srcBitmap, Rectangle srcRegion, Bitmap destBitmap, Rectangle destRegion)
    {
      using (var grD = Graphics.FromImage(destBitmap))
      {
        grD.DrawImage(srcBitmap, destRegion, srcRegion, GraphicsUnit.Pixel);
      }

      return destBitmap;
    }

    public Bitmap LayTile(GridElement[] gridElements)
    {
      var width = gridElements.Max(x=>x.Coordinate.X);
      var height = gridElements.Max(x=>x.Coordinate.Y);
      var collector = new Bitmap(width, height);
      return LayTile(gridElements, collector);
    }

    public Bitmap LayTile(GridElement[] gridElements, Bitmap collector)
    {
      if (!gridElements.Any())
        return collector;

      var element = gridElements.First();

      var srcRectangle = new Rectangle(0, 0, element.Value.Width, element.Value.Height);

      var destRectWidth = element.Coordinate.X + element.Value.Width;
      var destRectHeight = element.Coordinate.Y + element.Value.Height;
      var destRectangle = new Rectangle(element.Coordinate.X, element.Coordinate.Y, destRectWidth, destRectHeight);

      var newCollector = CopyRegionIntoImage(element.Value, srcRectangle, collector, destRectangle);
      return LayTile(gridElements.Skip(1).ToArray(), newCollector);
    }
  }
}
