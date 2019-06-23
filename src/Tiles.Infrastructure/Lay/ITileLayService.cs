using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using Tiles.Infrastructure.Grid;

namespace Tiles.Infrastructure.Lay
{
  public interface ITileLayService
  {
    Bitmap LayTile(Grid.Grid grid);
  }

  public class TileLayService : ITileLayService
  {
    public static Bitmap CopyRegionIntoImage(Bitmap srcBitmap, Rectangle srcRegion, Bitmap destBitmap, Rectangle destRegion)
    {
      using (var grD = Graphics.FromImage(destBitmap))
      {
        grD.DrawImage(srcBitmap, destRegion, srcRegion, GraphicsUnit.Pixel);
      }

      return new Bitmap(destBitmap);
    }

    public Bitmap LayTile(Grid.Grid grid)
    {
      var collector = new Bitmap(grid.Width, grid.Height);


      Bitmap ApplyToAccumulator(Bitmap accumulator, int x, int y)
      {
        var invisible = Color.FromArgb(0, 0, 0, 0);
        accumulator.SetPixel(x, y, invisible);
        return accumulator;
      }

      collector.ForEachPixel(collector, ApplyToAccumulator, ApplyToAccumulator);
      
      return LayTile(grid.GetElements().ToArray(), collector);
    }

    public Bitmap LayTile(GridElement[] gridElements, Bitmap collector)
    {
      if (!gridElements.Any())
        return collector;

      var element = gridElements.First();

      var srcRectangle = new Rectangle(0, 0, element.Value.Width, element.Value.Height);
      var destRectangle = new Rectangle(element.Coordinate.X, element.Coordinate.Y, element.Value.Width, element.Value.Height);
      var newCollector = CopyRegionIntoImage(new Bitmap(element.Value), srcRectangle, collector, destRectangle);
      return LayTile(gridElements.Skip(1).ToArray(), newCollector);
    }
  }
}
