using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
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

      const string layoutPath = @"./Layouts/01.bmp";
      var layout = new Bitmap(Image.FromFile(layoutPath)).Crop();

      // const string tilePath = @"./Tiles/tangier.bmp";
      const string tilePath = @"./Tiles/woodSquare.bmp";
      var tile = Image.FromFile(tilePath);

      var gridService = serviceProvider.GetService<IGridService>();
      var layoutGrid = gridService.Slice(layout, tile.Height);

      var serviceFactory = serviceProvider.GetService<ICutServiceFactory>();
      var cutTiles = layoutGrid.GetElements().Select(x =>
      {
        var service = serviceFactory.Build(x.Value);
        return service.CutTile(x.Value, new GridElement(x.Coordinate, tile));
      }).ToArray();

      await Task.WhenAll(cutTiles);
      var tileGrid = new Grid(cutTiles.Select(x => x.Result).ToArray(), tile.Width, layoutGrid.Width, layoutGrid.Height);

      var tileLayService = serviceProvider.GetService<ITileLayService>();
      var completed = tileLayService.LayTile(tileGrid);

      completed.Save("result.bmp", ImageFormat.Bmp);
      //logger.LogDebug("Completed application");
    }
  }
}
