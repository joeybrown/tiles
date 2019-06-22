using System.Drawing;
using System.Linq;

namespace Tiles.Infrastructure
{
  public delegate T ApplyToAccumulator<T>(T accumulator, int x, int y);

  public static class BitmapExtensions
  {
    private const string WhiteValue = "ffffffff";

    public static T ForEachPixel<T>(this Bitmap bitmap, T accumulator, ApplyToAccumulator<T> ifWhite, ApplyToAccumulator<T> ifColored)
    {
      foreach (var x in Enumerable.Range(0, bitmap.Width))
      {
        foreach (var y in Enumerable.Range(0, bitmap.Height))
        {
          var color = bitmap.GetPixel(x, y);
          accumulator = color.Name == WhiteValue
            ? ifWhite(accumulator, x, y) 
            : ifColored(accumulator, x, y);
        }
      }

      return accumulator;
    }
  }
}
