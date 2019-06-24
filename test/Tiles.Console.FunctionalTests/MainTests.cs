using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Tiles.Infrastructure.Cut;
using Xunit;

namespace Tiles.Console.FunctionalTests
{
  public class MainTests
  {
    public Action<IServiceCollection> ServiceOverrides
    {
      get
      {
        var scoreCutServiceSettings = (IScoreCutServiceSettings) new CutServiceSettings
        {
          CutTimeMilisecondDelay = 0,
          FailureRatePerCut = 0,
          MaxFailures = 0
        };
        var jigCutServiceSettings = (IJigCutServiceSettings) new CutServiceSettings
        {
          CutTimeMilisecondDelay = 0,
          FailureRatePerCut = 0,
          MaxFailures = 0
        };
        var laserCutServiceSettings = (ILaserCutServiceSettings) new CutServiceSettings
        {
          CutTimeMilisecondDelay = 0,
          FailureRatePerCut = 0,
          MaxFailures = 0
        };
        return services =>
        {
          services.AddSingleton(x => scoreCutServiceSettings)
            .AddSingleton(x => jigCutServiceSettings)
            .AddSingleton(x => laserCutServiceSettings);
        };
      }
    }

    [Fact]
    public void Run_Main_Wood_Square()
    {
      var args = new[]
      {
        @"./Layouts/01.bmp",
        @"./Tiles/woodSquare.bmp",
        @"./Output/00.bmp"
      };
      Directory.CreateDirectory(@"./Output");
      Func<Task> run = () => Program.Main(args, ServiceOverrides);
      run.Should().NotThrow();
    }

    [Fact]
    public void Run_Main_Tangier()
    {
      var args = new[]
      {
        @"./Layouts/01.bmp",
        @"./Tiles/tangier.bmp",
        @"./Output/01.bmp"
      };
      Directory.CreateDirectory(@"./Output");
      Func<Task> run = () => Program.Main(args, ServiceOverrides);
      run.Should().NotThrow();
    }

    [Fact]
    public void Run_Main_GMT()
    {
      var args = new[]
      {
        @"./Layouts/01.bmp",
        @"./Tiles/02.bmp",
        @"./Output/02.bmp"
      };
      Directory.CreateDirectory(@"./Output");
      Func<Task> run = () => Program.Main(args, ServiceOverrides);
      run.Should().NotThrow();
    }

    [Fact]
    public void Run_Main_Keanu()
    {
      var args = new[]
      {
        @"./Layouts/01.bmp",
        @"./Tiles/keanu.bmp",
        @"./Output/03.bmp"
      };
      Directory.CreateDirectory(@"./Output");
      Func<Task> run = () => Program.Main(args, ServiceOverrides);
      run.Should().NotThrow();
    }

    [Fact]
    public void Run_Main_Frank()
    {
      var args = new[]
      {
        @"./Layouts/01.bmp",
        @"./Tiles/frank.bmp",
        @"./Output/04.bmp"
      };
      Directory.CreateDirectory(@"./Output");
      Func<Task> run = () => Program.Main(args, ServiceOverrides);
      run.Should().NotThrow();
    }
  }
}
