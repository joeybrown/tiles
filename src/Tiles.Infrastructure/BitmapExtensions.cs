using System.Drawing;

namespace Tiles.Infrastructure
{
  public delegate T ApplyToAccumulator<T>(T accumulator, int x, int y);

  public static class BitmapExtensions
  {
    public static bool IsWhite(this Color color) => color.Name == WhiteValue;

    private const string WhiteValue = "ffffffff";

    public static T ForEachPixel<T>(this Bitmap bitmap, T accumulator, ApplyToAccumulator<T> ifWhite, ApplyToAccumulator<T> ifColored)
    {
      for(var x = 0; x < bitmap.Width; x++)
      {
        for(var y = 0; y < bitmap.Height; y++)
        {
          var color = bitmap.GetPixel(x, y);
          accumulator = color.IsWhite()
            ? ifWhite(accumulator, x, y) 
            : ifColored(accumulator, x, y);
        }
      }

      return accumulator;
    }

    public static bool HasAnyColor(this Bitmap bitmap)
    {
      bool IfWhite(bool accumulator, int x, int y) => accumulator;
      bool IfColored(bool accumulator, int x, int y) => true;
      const bool init = false;
      var anyColored = ForEachPixel(bitmap, init, IfWhite, IfColored);
      return anyColored;
    }

    public class RectangleBounds
    {
      public int? minX { get; set; }
      public int? minY { get; set; }
      public int? maxX { get; set; }
      public int? maxY { get; set; }
    }

    public static Bitmap Crop(this Bitmap bitmap)
    {
      RectangleBounds IfWhite(RectangleBounds accumulator, int x, int y) => accumulator;

      RectangleBounds IfColored(RectangleBounds accumulator, int x, int y)
      {
        if (accumulator.minX == null || x < accumulator.minX) { 
          accumulator.minX = x;
        }

        if (accumulator.maxX == null || accumulator.maxX < x)
        {
          accumulator.maxX = x;
        }

        if (accumulator.minY == null || y < accumulator.minY)
        {
          accumulator.minY = y;
        }

        if (accumulator.maxY == null || accumulator.maxY < y)
        {
          accumulator.maxY = y;
        }

        return accumulator;
      };

      var init = new RectangleBounds();
      var bounds = ForEachPixel(bitmap, init, IfWhite, IfColored);

      var rectX = bounds.minX.Value;
      var rectY = bounds.minY.Value;
      var rectWidth = bounds.maxX.Value + 1 - bounds.minX.Value;
      var rectHeight = bounds.maxY.Value + 1 - bounds.minY.Value;
      var rectangle = new Rectangle(rectX, rectY, rectWidth, rectHeight);
      return bitmap.Clone(rectangle, bitmap.PixelFormat);
    }
  }
}
