using System.Drawing;
using System.Threading.Tasks;

namespace Tiles.Infrastructure.Cut
{
  public interface ICutService
  {
    /// <summary>
    /// Tile and layout should be exactly the same size
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    Task<Image> Cut(Image tile);
  }

  public abstract class CutService
  {
    public static ICutService GetCutService(Image layout)
    {
      return new ScoreCutService(layout);
    }
  }

  public class ScoreCutService : ICutService
  {
    private readonly Image _layout;

    public ScoreCutService(Image layout)
    {
      _layout = layout;
    }

    public async Task<Image> Cut(Image tile)
    {
      await Task.Delay(2000);
      var cutTile = new Bitmap(tile.Height, tile.Width);
      return cutTile;
    }
  }

  public class JigCutService : ICutService
  {
    private readonly Image _layout;

    public JigCutService(Image layout)
    {
      _layout = layout;
    }

    public async Task<Image> Cut(Image tile)
    {
      await Task.Delay(4000);
      var cutTile = new Bitmap(tile.Height, tile.Width);
      return cutTile;
    }
  }

  public class LaserCutService : ICutService
  {
    private readonly Image _layout;

    public LaserCutService(Image layout)
    {
      _layout = layout;
    }

    public async Task<Image> Cut(Image tile)
    {
      await Task.Delay(8000);
      var cutTile = new Bitmap(tile.Height, tile.Width);
      return cutTile;
    }
  }
}
