using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Tiles.Infrastructure.Grid
{
  public interface IGridService
  {
    /// <summary>
    /// This method slices a given layout by the tile height.
    /// <para>If the layout is larger than the tile height, there will
    /// be a full tile in the center of the layout.</para>
    /// <para>This assumes a square tile.</para>
    /// </summary>
    Grid Slice(Image layout, int tileHeight);
  }

  public class GridService : IGridService
  {
    private static IEnumerable<ICoordinate> ToCoordinates(Bitmap layout)
    {
      List<ICoordinate> IfWhite(List<ICoordinate> coordinates, int x, int y)
      {
        coordinates.Add(new EmptyCoordinate(x, y));
        return coordinates;
      }

      List<ICoordinate> IfColored(List<ICoordinate> coordinates, int x, int y)
      {
        coordinates.Add(new ColoredCoordinate(x, y));
        return coordinates;
      }

      var accumulator = new List<ICoordinate>();


      return layout.ForEachPixel(accumulator, IfWhite, IfColored);
    }

    private static Bitmap GetTrimmedArea(Bitmap layout, IEnumerable<ICoordinate> coordinates)
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

    private static IEnumerable<int> SliceBackward(int start, int step) => 
      start <= 0 ? new List<int>() : new List<int>{start}.Concat(SliceBackward(start-step, step)).ToList();

    private static IEnumerable<int> SliceForward(int start, int step, int limit) =>
      start >= limit ? new List<int>() : new List<int> {start}.Concat(SliceForward(start + step, step, limit));

    private static int[] SliceByTileWidth(int limit, int tileWidth)
    {
      var midPoint = limit / 2;
      var lowerSet = new []{0}.Concat( SliceBackward(midPoint - (tileWidth / 2), tileWidth).ToArray());
      var upperSet = SliceForward(midPoint + (tileWidth / 2), tileWidth, limit).ToArray().Concat(new []{limit});
      return lowerSet.Concat(upperSet).ToArray();
    }

    /// <inheritdoc cref="IGridService.Slice"/>
    public Grid Slice(Image layout, int tileWidth)
    {
      using (var bitmapLayout = new Bitmap(layout))
      {
        var coordinates = ToCoordinates(bitmapLayout).ToArray();
        var hasArea = coordinates.Any(x => x is ColoredCoordinate);
        if (!hasArea)
        {
          return new Grid();
        }

        var trimmedLayout = GetTrimmedArea(bitmapLayout, coordinates);

        var xSlices = SliceByTileWidth(trimmedLayout.Width, tileWidth);
        var ySlices = SliceByTileWidth(trimmedLayout.Height, tileWidth);

        var xSets = GetSets(xSlices);
        var ySets = GetSets(ySlices);

        var grid = BuildGrid(xSets, ySets, layout);
        return grid;
      }
      
    }

    private static IEnumerable<(int start, int stop)> GetSets(IReadOnlyCollection<int> slices)
    {
      if (slices.Count <= 1)
      {
        return new (int start, int stop)[0];
      }

      (int start, int stop) newVal = (slices.First(), slices.Skip(1).First());
      return new[] {newVal}.Concat(GetSets(slices.Skip(1).ToArray()));
    }

    private static Bitmap SliceImage(int xRectStart, int xRectStop, int yRectStart, int yRectStop, Image image)
    {
      var bitmap = new Bitmap(image);
      var rectWidth = (xRectStop - xRectStart);
      var rectHeight = (yRectStop - yRectStart);
      var rectangle = new Rectangle(xRectStart, yRectStart, rectWidth, rectHeight);
      return bitmap.Clone(rectangle, bitmap.PixelFormat);
    }
    
    private static Grid BuildGrid(IEnumerable<(int start, int stop)> xSets, IEnumerable<(int start, int stop)> ySets, Image image)
    {
      var elements = xSets.SelectMany(x =>
        ySets.Select(y =>
        {
          var gridElementValue = SliceImage(x.start, x.stop, y.start, y.stop, image);
          var gridElementCoordinates = new Coordinate(x.start, y.start);
          return new GridElement(gridElementCoordinates, gridElementValue);
        }));
      var bitmap = new Bitmap(image);
      return new Grid(elements, bitmap.Width, bitmap.Height);
    }
  }
}