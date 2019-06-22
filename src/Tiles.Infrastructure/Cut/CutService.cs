using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Tiles.Infrastructure.Cut
{
  public interface ICutService
  {
    /// <summary>
    ///   Cuts a tile according to the layout pattern.
    /// </summary>
    Task<Image> CutTile(Image layout, Image tile);
  }

  public interface ICutServiceFactory
  {
    /// <summary>
    ///   Pass in a layout to determine which cut service to use.
    /// </summary>
    ICutService Build(Image layout);
  }

  public class CutServiceFactory : ICutServiceFactory
  {
    private readonly IJigCutService _jigCutService;
    private readonly ILaserCutService _laserCutService;
    private readonly IScoreCutService _scoreCutService;

    public CutServiceFactory(
      IScoreCutService scoreCutService,
      IJigCutService jigCutService,
      ILaserCutService laserCutService)
    {
      _scoreCutService = scoreCutService;
      _jigCutService = jigCutService;
      _laserCutService = laserCutService;
    }

    private ICutService[] CutServices => new[]
    {
      _scoreCutService,
      _scoreCutService,
      _scoreCutService,
      _scoreCutService,
      _jigCutService,
      _jigCutService,
      (ICutService) _laserCutService
    };

    public ICutService Build(Image layout)
    {
      var random = new Random();
      var index = random.Next(CutServices.Length);
      return CutServices[index];
    }
  }

  public abstract class CutService : ICutService
  {
    private readonly IFailureService _failureService;
    private readonly ICutServiceSettings _settings;

    protected CutService(ICutServiceSettings settings, IFailureService failureService)
    {
      _settings = settings;
      _failureService = failureService;
    }

    public Task<Image> CutTile(Image layout, Image tile)
    {
      if (_settings.FailureRatePerCut == 0)
      {
        return Task.FromResult(CopyTile(layout, tile));
      }

      var cutsPerFailure = (int) (1 / _settings.FailureRatePerCut);
      return CutTile(layout, tile, cutsPerFailure, 0);
    }

    private static Image CopyTile(Image layout, Image tile)
    {
      var layoutBitmap = new Bitmap(layout);
      var tileBitmap = new Bitmap(tile);

      Bitmap IfWhite(Bitmap accumulator, int x, int y)
        => accumulator;

      Bitmap IfColored(Bitmap accumulator, int x, int y)
      {
        var pixel = tileBitmap.GetPixel(x, y);
        accumulator.SetPixel(x, y, pixel);
        return accumulator;
      }

      return layoutBitmap.ForEachPixel(layoutBitmap, IfWhite, IfColored);
    }

    private async Task<Image> CutTile(Image layout, Image tile, int cutPerFailure, int attempts)
    {
      if (attempts >= _settings.MaxFailures)
      {
        throw new Exception(
          $"{attempts} attempts at cutting tile failed. This exceeds threshold of {_settings.MaxFailures}.");
      }

      await Task.Delay(_settings.CutTimeMilisecondDelay);
      if (_failureService.IsFailure(_settings.FailureRatePerCut))
      {
        tile = await CutTile(layout, tile, cutPerFailure, attempts + 1);
        return tile;
      }

      return CopyTile(layout, tile);
    }
  }

  public interface IScoreCutService : ICutService
  {
  }

  public class ScoreCutService : CutService, IScoreCutService
  {
    public ScoreCutService(ICutServiceSettings settings, IFailureService failureService) :
      base(settings, failureService)
    {
    }
  }

  public interface IJigCutService : ICutService
  {
  }

  public class JigCutCutService : CutService, IJigCutService
  {
    public JigCutCutService(ICutServiceSettings settings, IFailureService failureService) :
      base(settings, failureService)
    {
    }
  }

  public interface ILaserCutService : ICutService
  {
  }

  public class LaserCutService : CutService, ILaserCutService
  {
    public LaserCutService(ICutServiceSettings settings, IFailureService failureService) :
      base(settings, failureService)
    {
    }
  }
}