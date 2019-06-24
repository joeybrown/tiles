using System;
using System.Drawing;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tiles.Infrastructure.Grid;

namespace Tiles.Infrastructure.Cut
{
  public interface ICutService
  {
    /// <summary>
    ///   Cuts a tile according to the layout pattern.
    /// </summary>
    Task<GridElement> CutTile(Image layout, GridElement gridElement);
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

    private static bool ShouldPadX(Image layout, GridElement tile)
    {
      var adjustTileWidth = layout.Width < tile.Value.Width;
      if (!adjustTileWidth)
        return false;
      return tile.Coordinate.X == 0;
    }

    private static bool ShouldPadY(Image layout, GridElement tile)
    {
      var adjustTileHeight = layout.Height < tile.Value.Height;
      if (!adjustTileHeight)
        return false;
      return tile.Coordinate.Y == 0;
    }


    public virtual async Task<GridElement> CutTile(Image layout, GridElement tile)
    {

      var padX = ShouldPadX(layout, tile);
      var padY = ShouldPadY(layout, tile);
      if (_settings.FailureRatePerCut == 0)
      {
        await Task.Delay(_settings.CutTimeMilisecondDelay);
        var cutTile = CopyTile(layout, tile.Value, padX, padY);
        var gridElement = new GridElement(tile.Coordinate, cutTile);
        return gridElement;
      }

      var cutsPerFailure = (int) (1 / _settings.FailureRatePerCut);
      var result = await CutTile(layout, tile, cutsPerFailure, 0, padX, padY);
      return result;
    }

    private async Task<GridElement> CutTile(Image layout, GridElement tile, int cutsPerFailure, int attempts,
      bool padX, bool padY)
    {
      if (attempts >= _settings.MaxFailures)
      {
        throw new Exception(
          $"{attempts} attempts at cutting tile failed. This exceeds threshold of {_settings.MaxFailures}.");
      }

      await Task.Delay(_settings.CutTimeMilisecondDelay);
      if (_failureService.IsFailure(_settings.FailureRatePerCut))
      {
        tile = await CutTile(layout, tile, cutsPerFailure, attempts + 1, padX, padY);
        return tile;
      }

      var cutTile = CopyTile(layout, tile.Value, padX, padY);
      return new GridElement(tile.Coordinate, cutTile);
    }


    private static Image CopyTile(Image layout, Image tile, bool padX, bool padY)
    {
      var layoutBitmap = new Bitmap(layout);
      var tileBitmap = new Bitmap(tile);

      Bitmap IfWhite(Bitmap accumulator, int x, int y)
        => accumulator;

      Bitmap IfColored(Bitmap accumulator, int x, int y)
      {
        var xOffset = padX ? tile.Width - accumulator.Width : 0;
        var yOffset = padY ? tile.Height - accumulator.Height : 0;

        var tileX = x + xOffset;
        var tileY = y + yOffset;
        var pixel = tileBitmap.GetPixel(tileX, tileY);
        accumulator.SetPixel(x, y, pixel);
        return accumulator;
      }

      return layoutBitmap.ForEachPixel(layoutBitmap, IfWhite, IfColored);
    }
  }

  public interface IScoreCutService : ICutService
    {
    }

    public class ScoreCutService : CutService, IScoreCutService
    {
      private readonly ILogger<ScoreCutService> _logger;

      public ScoreCutService(IScoreCutServiceSettings settings, IFailureService failureService, ILogger<ScoreCutService> logger) :
        base(settings, failureService)
      {
        _logger = logger;
      }

      public override async Task<GridElement> CutTile(Image layout, GridElement tile)
      {
        _logger.LogDebug($"Cutting tile with the {nameof(ScoreCutService)}");
         var result = await base.CutTile(layout, tile);
        _logger.LogDebug($"Finished cutting tile with the {nameof(ScoreCutService)}");
        return result;
      }
  }


  public interface IJigCutService : ICutService
  {
  }

  public class JigCutService : CutService, IJigCutService
  {
    private readonly ILogger<JigCutService> _logger;

    public JigCutService(IJigCutServiceSettings settings, IFailureService failureService, ILogger<JigCutService> logger) :
      base(settings, failureService)
    {
      _logger = logger;
    }

    public override async Task<GridElement> CutTile(Image layout, GridElement tile)
    {
      _logger.LogDebug($"Cutting tile with the {nameof(JigCutService)}");
      var result = await base.CutTile(layout, tile);
      _logger.LogDebug($"Finished cutting tile with the {nameof(JigCutService)}");
      return result;
    }
  }

  public interface ILaserCutService : ICutService
  {
  }

  public class LaserCutService : CutService, ILaserCutService
  {
    private readonly ILogger<LaserCutService> _logger;

    public LaserCutService(ILaserCutServiceSettings settings, IFailureService failureService, ILogger<LaserCutService> logger) :
      base(settings, failureService)
    {
      _logger = logger;
    }

    public override async Task<GridElement> CutTile(Image layout, GridElement tile)
    {
      _logger.LogDebug($"Cutting tile with the {nameof(LaserCutService)}");
      var result = await base.CutTile(layout, tile);
      _logger.LogDebug($"Finished cutting tile with the {nameof(LaserCutService)}");
      return result;
    }
  }
}
