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
        CutTimeMilisecondDelay = 2000,
        FailureRatePerCut = 0,
        MaxFailures = 0
      };
      var jigCutServiceSettings = new CutServiceSettings
      {
        CutTimeMilisecondDelay = 4000,
        FailureRatePerCut = 0,
        MaxFailures = 0
      };
      var laserCutServiceSettings = new CutServiceSettings
      {
        CutTimeMilisecondDelay = 8000,
        FailureRatePerCut = 0,
        MaxFailures = 0
      };


      var failureServiceProvider = new ServiceCollection()
        .AddSingleton<IFailureService, FailureService>()
        .BuildServiceProvider();

      var failureService = failureServiceProvider.GetService<IFailureService>();

      var serviceProvider = new ServiceCollection()
        .AddLogging()
        .AddSingleton<IGridService, GridService>()
        .AddSingleton<ICutServiceFactory, CutServiceFactory>()
        .AddSingleton(x=>
          (IScoreCutService) new ScoreCutService(scoreCutServiceSettings, failureService))
        .AddSingleton(x =>
          (IJigCutService)new JigCutService(jigCutServiceSettings, failureService))
        .AddSingleton(x =>
          (ILaserCutService)new LaserCutService(laserCutServiceSettings, failureService)).AddSingleton<ITileLayService, TileLayService>()
        .BuildServiceProvider();

      // serviceProvider
        // .GetService<ILoggingBuilder>().AddConsole();

      //var logger = serviceProvider.GetService<ILoggerFactory>()
      //  .CreateLogger<Program>();
      //logger.LogDebug("Starting application");

      const string layoutPath = @"./Layouts/00.bmp";
      var layout = new Bitmap(Image.FromFile(layoutPath)).Crop();

      const string tilePath = @"./Tiles/wood.bmp";
      var tile = Image.FromFile(tilePath);

      var gridService = serviceProvider.GetService<IGridService>();
      var layoutGrid = gridService.Slice(layout, tile.Height);

      //var cutServiceFactory = serviceProvider.GetService<ICutServiceFactory>();
      var tileGridElements = layoutGrid.GetElements().Select(async x =>
      {
        var serviceFactory = serviceProvider.GetService<ICutServiceFactory>();
        var service = serviceFactory.Build(x.Value);
        var cutTile = await service.CutTile(x.Value, tile);
        return new GridElement(x.Coordinate, cutTile);
      }).ToArray();

      await Task.WhenAll(tileGridElements);
      var tileGrid = new Grid(tileGridElements.Select(x => x.Result).ToArray(), layoutGrid.Width, layoutGrid.Height);

      var tileLayService = serviceProvider.GetService<ITileLayService>();
      var completed = tileLayService.LayTile(tileGrid);

      completed.Save("result.bmp", ImageFormat.Bmp);
      //logger.LogDebug("Completed application");
    }
  }
}
