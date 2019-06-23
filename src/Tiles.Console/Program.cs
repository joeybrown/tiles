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
  class Program
  {
    static async Task Main(string[] args)
    {
      var scoreCutServiceSettings = new CutServiceSettings
      {
        CutTimeMilisecondDelay = 4000,
        FailureRatePerCut = 0,
        MaxFailures = 0
      };
      var jigCutServiceSettings = new CutServiceSettings
      {
        CutTimeMilisecondDelay = 8000,
        FailureRatePerCut = 0,
        MaxFailures = 0
      };
      var laserCutServiceSettings = new CutServiceSettings
      {
        CutTimeMilisecondDelay = 16000,
        FailureRatePerCut = 0,
        MaxFailures = 0
      };


      var failureServiceProvider = new ServiceCollection()
        .AddSingleton<IFailureService, FailureService>()
        .BuildServiceProvider();

      var failureService = failureServiceProvider.GetService<IFailureService>();

      var serviceProvider = new ServiceCollection()
        .AddLogging(configure => configure.AddConsole())
        .Configure<LoggerFilterOptions>(options =>
        {
          options.MinLevel = LogLevel.Debug;
        })
        .AddSingleton<IGridService, GridService>()
        .AddSingleton<ICutServiceFactory, CutServiceFactory>()
        .AddSingleton(x=>
          (IScoreCutService) new ScoreCutService(scoreCutServiceSettings, failureService))
        .AddSingleton(x =>
          (IJigCutService)new JigCutService(jigCutServiceSettings, failureService))
        .AddSingleton(x =>
          (ILaserCutService)new LaserCutService(laserCutServiceSettings, failureService)).AddSingleton<ITileLayService, TileLayService>()
        .BuildServiceProvider();

      var logger = serviceProvider.GetService<ILogger<Program>>();

      logger.LogInformation($"[{DateTime.Now.ToString("HH:mm:ss.fffffff")}] Starting Service.");

      logger.LogInformation($"[{DateTime.Now.ToString("HH:mm:ss.fffffff")}] Cropping Image...");

      const string layoutPath = @"./Layouts/01.bmp";
      var layout = new Bitmap(Image.FromFile(layoutPath)).Crop();

      logger.LogInformation($"[{DateTime.Now.ToString("HH:mm:ss.fffffff")}] Cropped Image.");

      // const string tilePath = @"./Tiles/tangier.bmp";
      const string tilePath = @"./Tiles/woodSquare.bmp";
      var tile = Image.FromFile(tilePath);

      logger.LogInformation($"[{DateTime.Now.ToString("HH:mm:ss.fffffff")}] Creating Grid...");
      var gridService = serviceProvider.GetService<IGridService>();
      var layoutGrid = gridService.Slice(layout, tile.Height);
      logger.LogInformation($"[{DateTime.Now.ToString("HH:mm:ss.fffffff")}] Created Grid.");

      logger.LogInformation($"[{DateTime.Now.ToString("HH:mm:ss.fffffff")}] Cutting Tiles...");
      var serviceFactory = serviceProvider.GetService<ICutServiceFactory>();
      var cutTiles = layoutGrid.GetElements().Select(x =>
      {
        var service = serviceFactory.Build(x.Value);
        return service.CutTile(x.Value, new GridElement(x.Coordinate, tile));
      }).ToArray();

      await Task.WhenAll(cutTiles);
      logger.LogInformation($"[{DateTime.Now.ToString("HH:mm:ss.fffffff")}] Cut Tiles.");

      var tileGrid = new Grid(cutTiles.Select(x => x.Result).ToArray(), tile.Width, layoutGrid.Width, layoutGrid.Height);

      logger.LogInformation($"[{DateTime.Now.ToString("HH:mm:ss.fffffff")}] Laying Tiles...");
      var tileLayService = serviceProvider.GetService<ITileLayService>();
      var completed = tileLayService.LayTile(tileGrid);
      logger.LogInformation($"[{DateTime.Now.ToString("HH:mm:ss.fffffff")}] Laid Tiles.");

      completed.Save("result.bmp", ImageFormat.Bmp);
    }
  }
}
