using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tiles.Infrastructure.Grid;

namespace Tiles.Infrastructure
{
  public delegate T ApplyToAccumulator<T>(T accumulator, int x, int y);

  public static class BitmapExtensions
  {
    public static bool IsWhite(this Color color) => color.Name == WhiteValue;

    private const string WhiteValue = "ffffffff";

    public static T ForEachPixel<T>(this Bitmap bitmap, T accumulator, ApplyToAccumulator<T> ifWhite, ApplyToAccumulator<T> ifColored)
    {
      foreach (var x in Enumerable.Range(0, bitmap.Width))
      {
        foreach (var y in Enumerable.Range(0, bitmap.Height))
        {
          var color = bitmap.GetPixel(x, y);
          accumulator = color.IsWhite()
            ? ifWhite(accumulator, x, y) 
            : ifColored(accumulator, x, y);
        }
      }

      return accumulator;
    }

    public static IEnumerable<ICoordinate> ToCoordinates(this Bitmap layout)
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

    public static bool HasAnyColor(this Bitmap bitmap)
    {
      var coordinates = bitmap.ToCoordinates().ToArray();
      return coordinates.Any(x => x is ColoredCoordinate);
    }

    public static Bitmap Crop(this Bitmap bitmap)
    {
      var coloredCoordinates = bitmap.ToCoordinates().Where(x => x is ColoredCoordinate).ToArray();
      var xValues = coloredCoordinates.Select(x => x.X).Distinct().ToArray();
      var yValues = coloredCoordinates.Select(x => x.Y).Distinct().ToArray();
      var rectX = xValues.Min();
      var rectY = yValues.Min();
      var rectWidth = xValues.Max() + 1 - xValues.Min();
      var rectHeight = yValues.Max() + 1 - yValues.Min();
      var rectangle = new Rectangle(rectX, rectY, rectWidth, rectHeight);
      return bitmap.Clone(rectangle, bitmap.PixelFormat);
    }
  }
}
