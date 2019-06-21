using System.Drawing;
using System.Threading.Tasks;

namespace Tiles.Infrastructure.Cut
{
  public interface ICutService
  {
    Task<Image> CutRequest(Image layout);
  }

  public interface ICutServiceFactory
  {
    ICutService Build(Image layout);
  }

  public class CutServiceFactory : ICutServiceFactory
  {
    private readonly IJigCutService _jigCutService;
    private readonly ILaserCutService _laserCutService;
    private readonly IRandomFactory _randomFactory;
    private readonly IScoreCutService _scoreCutService;

    public CutServiceFactory(
      IRandomFactory randomFactory,
      IScoreCutService scoreCutService,
      IJigCutService jigCutService,
      ILaserCutService laserCutService)
    {
      _randomFactory = randomFactory;
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
      var index = _randomFactory.GetRandomNumber(CutServices.Length);
      return CutServices[index];
    }
  }

  public interface IScoreCutService : ICutService
  {
  }

  public class ScoreCutService : IScoreCutService
  {
    private readonly Image _tileDesign;

    public ScoreCutService(Image tileDesign)
    {
      _tileDesign = tileDesign;
    }



    public async Task<Image> CutRequest(Image tile)
    {
      await Task.Delay(2000);
      return new Bitmap(tile);
    }
  }

  public interface IJigCutService : ICutService
  {
  }

  public class JigCutCutService : IJigCutService
  {
    public async Task<Image> CutRequest(Image tile)
    {
      await Task.Delay(4000);
      return new Bitmap(tile);
    }
  }

  public interface ILaserCutService : ICutService
  {
  }

  public class LaserCutService : ILaserCutService
  {
    public async Task<Image> CutRequest(Image tile)
    {
      await Task.Delay(8000);
      return new Bitmap(tile);
    }
  }
}