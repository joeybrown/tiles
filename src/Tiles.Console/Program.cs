using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tiles.Infrastructure;
using Tiles.Infrastructure.Cut;
using Tiles.Infrastructure.Grid;
using Tiles.Infrastructure.Lay;


namespace Tiles.Console
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
      var scoreCutServiceSettings = (IScoreCutServiceSettings) new CutServiceSettings
      {
        CutTimeMilisecondDelay = 1000,
        FailureRatePerCut = 0,
        MaxFailures = 0
      };
      var jigCutServiceSettings = (IJigCutServiceSettings) new CutServiceSettings
      {
        CutTimeMilisecondDelay = 2000,
        FailureRatePerCut = 0,
        MaxFailures = 0
      };
      var laserCutServiceSettings = (ILaserCutServiceSettings) new CutServiceSettings
      {
        CutTimeMilisecondDelay = 4000,
        FailureRatePerCut = 0,
        MaxFailures = 0
      };

      services
        .AddLogging(configure => configure.AddConsole())
        .Configure<LoggerFilterOptions>(options => { options.MinLevel = LogLevel.Debug; })
        .AddSingleton<IGridService, GridService>()
        .AddSingleton<ICutServiceFactory, CutServiceFactory>()
        .AddSingleton<IFailureService, FailureService>()
        .AddSingleton(x => scoreCutServiceSettings)
        .AddSingleton(x => jigCutServiceSettings)
        .AddSingleton(x => laserCutServiceSettings)
        .AddSingleton<IScoreCutService, ScoreCutService>()
        .AddSingleton<IJigCutService, JigCutService>()
        .AddSingleton<ITileLayService, TileLayService>()
        .AddSingleton<ILaserCutService, LaserCutService>();
      return services;
    }
  }

  public class Program
  {
    public virtual ServiceProvider BuildServiceProvider()
    {
      var serviceProvider = new ServiceCollection()
        .AddServices()
        .BuildServiceProvider();
      return serviceProvider;
    }

    public static async Task Main(string[] args)
    {
      var serviceProvider = new Program().BuildServiceProvider();

      var layoutPath = args[0];
      var layout = new Bitmap(Image.FromFile(layoutPath)).Crop();

      var tilePath = args[1];
      var tile = new Bitmap(Image.FromFile(tilePath));

      var logger = serviceProvider.GetService<ILogger<Program>>();

      logger.LogInformation($"[{DateTime.Now.ToString("HH:mm:ss.fffffff")}] Creating Grid...");
      var gridService = serviceProvider.GetService<IGridService>();
      var layoutGrid = gridService.Slice(layout, tile.Height);
      logger.LogInformation($"[{DateTime.Now.ToString("HH:mm:ss.fffffff")}] Created Grid.");

      logger.LogInformation($"[{DateTime.Now.ToString("HH:mm:ss.fffffff")}] Cutting Tiles...");
      var serviceFactory = serviceProvider.GetService<ICutServiceFactory>();
      var cutTiles = layoutGrid.GetElements().Select(x =>
      {
        var service = serviceFactory.Build(x.Value);
        return service.CutTile(new Bitmap(x.Value), new GridElement(x.Coordinate, new Bitmap(tile)));
      }).ToArray();

      await Task.WhenAll(cutTiles);
      logger.LogInformation($"[{DateTime.Now.ToString("HH:mm:ss.fffffff")}] Cut Tiles.");

      var tileGrid = new Grid(cutTiles.Select(x => x.Result).ToArray(), tile.Width, layoutGrid.Width, layoutGrid.Height);

      logger.LogInformation($"[{DateTime.Now.ToString("HH:mm:ss.fffffff")}] Laying Tiles...");
      var tileLayService = serviceProvider.GetService<ITileLayService>();
      var completed = tileLayService.LayTile(tileGrid);
      logger.LogInformation($"[{DateTime.Now.ToString("HH:mm:ss.fffffff")}] Laid Tiles.");

      var dest = args[2];
      completed.Save(dest, ImageFormat.Bmp);
    }
  }
}
