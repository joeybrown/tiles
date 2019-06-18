using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace Tiles.Infrastructure.Grid
{
  public interface IGridService
  {
    Grid Slice(Image layout, int tileHeight);
  }

  public class GridService : IGridService
  {
    private readonly IGridServiceSettings _settings;

    public GridService(IGridServiceSettings settings)
    {
      _settings = settings;
    }

    private const string WhiteValue = "ffffffff";

    public static IEnumerable<ICoordinate> ToCoordinates(Bitmap layout)
    {
      foreach (var x in Enumerable.Range(0, layout.Width))
      {
        foreach (var y in Enumerable.Range(0, layout.Height))
        {
          var color = layout.GetPixel(x, y);
          if (color.Name != WhiteValue)
            yield return new ColoredCoordinate(x, y);
          yield return new EmptyCoordinate(x, y);
        }
      }
    }

    public static Bitmap GetTrimmedArea(Bitmap layout, IEnumerable<ICoordinate> coordinates)
    {
      var coloredCoordinates = coordinates.Where(x => x is ColoredCoordinate).ToArray();
      var xValues = coloredCoordinates.Select(x => x.X).ToArray();
      var yValues = coloredCoordinates.Select(x => x.Y).ToArray();
      var rectX = xValues.Min();
      var rectY = yValues.Min();
      var rectWidth = xValues.Max() + 1 - xValues.Min();
      var rectHeight = yValues.Max() + 1 - yValues.Min();
      var rectangle = new Rectangle(rectX, rectY, rectWidth, rectHeight);
      return layout.Clone(rectangle, layout.PixelFormat);
    }

    public IEnumerable<int> SliceBackward(int start, int step) => 
      start <= 0 ? new List<int>() : new List<int>{start}.Concat(SliceBackward(start-step, step)).ToList();

    public IEnumerable<int> SliceForward(int start, int step, int limit) =>
      start >= limit ? new List<int>() : new List<int> {start}.Concat(SliceForward(start + step, step, limit));

    public int[] SliceByTileWidth(int limit, int tileWidth)
    {
      var midPoint = limit / 2;
      var lowerSet = new []{0}.Concat( SliceBackward(midPoint - (tileWidth / 2), tileWidth).ToArray());
      var upperSet = SliceForward(midPoint + (tileWidth / 2), tileWidth, limit).ToArray().Concat(new []{limit});
      return lowerSet.Concat(upperSet).ToArray();
    }

    public Grid Slice(Image layout, int tileWidth)
    {
      var bitmapLayout = new Bitmap(layout);
      var coordinates = ToCoordinates(bitmapLayout).ToArray();
      var hasArea = coordinates.Any(x => x is ColoredCoordinate);
      if (!hasArea)
      {
        return new Grid();
      }

      var trimmedLayout = GetTrimmedArea(bitmapLayout, coordinates);

      var xSlices = SliceByTileWidth(trimmedLayout.Width, tileWidth);
      var ySlices = SliceByTileWidth(trimmedLayout.Height, tileWidth);

      IEnumerable<GridElement> elements;

      if (!xSlices.Any() && !ySlices.Any())
      {
        var coordinate = new Coordinate(0, 0);
        elements = new[] { new GridElement(coordinate, layout) };
        return new Grid(elements);
      }

      var blah = new Coordinate(0, 0);
      elements = new[] { new GridElement(blah, layout) };
      return new Grid(elements);
    }
  }
}