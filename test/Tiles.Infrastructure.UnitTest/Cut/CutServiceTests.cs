using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Tiles.Infrastructure.Cut;
using Tiles.Infrastructure.Grid;
using Xunit;

namespace Tiles.Infrastructure.UnitTest.Cut
{
  public class CutServiceTests
  {
    [Fact]
    public void Score_Cut_With_Failure_Rate_Zero_Should_Not_Fail()
    {
      var cutSettingsMock = new Mock<ICutServiceSettings>();
      cutSettingsMock.SetupGet(x => x.CutTimeMilisecondDelay).Returns(0);
      cutSettingsMock.SetupGet(x => x.FailureRatePerCut).Returns(0);
      cutSettingsMock.SetupGet(x => x.MaxFailures).Returns(0);

      var failureServiceMock = new Mock<IFailureService>();
      failureServiceMock
        .Setup(x => x.IsFailure(It.IsAny<decimal>()))
        .Returns(true);

      var scoreCutService = new ScoreCutService(cutSettingsMock.Object, failureServiceMock.Object);
      var layout = new Bitmap(100, 100);
      var tile = new Bitmap(100, 100);
      var gridElement = new GridElement(new Coordinate(0, 0), tile);

      Func<Task> action = async () => await scoreCutService.CutTile(layout, gridElement);

      action.Should().NotThrow("failure rate per cut is 0");
    }

    [Fact]
    public void Score_Cut_With_Failure_Rate_Should_Not_Fail()
    {
      var cutSettingsMock = new Mock<ICutServiceSettings>();
      cutSettingsMock.SetupGet(x => x.CutTimeMilisecondDelay).Returns(0);
      cutSettingsMock.SetupGet(x => x.FailureRatePerCut).Returns(.5m);
      cutSettingsMock.SetupGet(x => x.MaxFailures).Returns(2);

      var failureServiceMock = new Mock<IFailureService>();
      failureServiceMock.SetupSequence(x => x.IsFailure(It.IsAny<decimal>()))
        .Returns(true)
        .Returns(false);

      var scoreCutService = new ScoreCutService(cutSettingsMock.Object, failureServiceMock.Object);
      var layout = new Bitmap(100, 100);
      var tile = new Bitmap(100, 100);
      var gridElement = new GridElement(new Coordinate(0, 0), tile);

      Func<Task> action = async () => await scoreCutService.CutTile(layout, gridElement);

      action.Should().NotThrow("the first pass thru will not be successful but the second one will be successful.");
    }

    [Fact]
    public void Score_Cut_With_Too_High_Failure_Rate_Should_Fail()
    {
      var cutSettingsMock = new Mock<ICutServiceSettings>();
      cutSettingsMock.SetupGet(x => x.CutTimeMilisecondDelay).Returns(0);
      cutSettingsMock.SetupGet(x => x.FailureRatePerCut).Returns(1);
      cutSettingsMock.SetupGet(x => x.MaxFailures).Returns(1);

      var failureServiceMock = new Mock<IFailureService>();
      failureServiceMock.Setup(x => x.IsFailure(It.IsAny<decimal>())).Returns(true);

      var scoreCutService = new ScoreCutService(cutSettingsMock.Object, failureServiceMock.Object);
      var layout = new Bitmap(100, 100);
      var tile = new Bitmap(100, 100);
      var gridElement = new GridElement(new Coordinate(0,0), tile);

      Func<Task> action = async () => await scoreCutService.CutTile(layout, gridElement);

      action.Should().Throw<Exception>("failure rate per cut is 1");
    }

    [Theory]
    [MemberData(nameof(CutServicesTestCases))]
    public async Task Cut_Service_Should_Cut_Full_Tile(CutServiceTestCase testCase)
    {
      var layout = Image.FromFile(testCase.ImagePath);
      var tile = Image.FromFile(testCase.ImagePath);
      var gridElement = new GridElement(new Coordinate(0,0), tile);
      var result = await testCase.Service.CutTile(layout, gridElement);

      var bitmap = new Bitmap(result.Value);
      var pixelsAreEqual = new List<bool>();

      var layoutBitmap = new Bitmap(layout);

      List<bool> Func(List<bool> accumulator, int x, int y)
      {
        var resultPixel = bitmap.GetPixel(x, y);
        var layoutPixel = layoutBitmap.GetPixel(x, y);
        accumulator.Add(resultPixel.Name == layoutPixel.Name);
        return accumulator;
      }

      pixelsAreEqual = bitmap.ForEachPixel(pixelsAreEqual, Func, Func);

      pixelsAreEqual.Count.Should().Be(10000, "there are 10000 pixels");
      pixelsAreEqual.Any(x => !x).Should().BeFalse("all pixels should be equivalent");
    }

    [Theory]
    [MemberData(nameof(CutServices))]
    public async Task Cut_Service_Should_Cut_Half_Tile(ICutService cutService)
    {
      var layout = Image.FromFile(@"./Layouts/07.bmp");
      var tile = Image.FromFile(@"./Tiles/01.bmp");
      var gridElement = new GridElement(new Coordinate(0, 0), tile);
      var result = await cutService.CutTile(layout, gridElement);

      var bitmap = new Bitmap(result.Value);
      var pixelsAreEqual = new List<bool>();

      var expected = new Bitmap(Image.FromFile(@"./Tiles/04.bmp"));

      List<bool> Func(List<bool> accumulator, int x, int y)
      {
        var resultPixel = bitmap.GetPixel(x, y);
        var expectedPixel = expected.GetPixel(x, y);
        accumulator.Add(resultPixel.Name == expectedPixel.Name);
        return accumulator;
      }

      pixelsAreEqual = bitmap.ForEachPixel(pixelsAreEqual, Func, Func);

      pixelsAreEqual.Count.Should().Be(5000, "there are 5000 pixels");
      pixelsAreEqual.Any(x => !x).Should().BeFalse("all pixels should be equivalent");
    }

    public static IEnumerable<object[]> CutServices
    {
      get
      {
        var cutSettingsMock = new Mock<ICutServiceSettings>();
        cutSettingsMock.SetupGet(x => x.CutTimeMilisecondDelay).Returns(0);
        cutSettingsMock.SetupGet(x => x.FailureRatePerCut).Returns(0);

        var failureServiceMock = new Mock<IFailureService>();
        failureServiceMock.Setup(x => x.IsFailure(It.IsAny<decimal>())).Returns(true);
        return new[]
        {
          new[] {new ScoreCutService(cutSettingsMock.Object, failureServiceMock.Object)},
          new[] {new JigCutService(cutSettingsMock.Object, failureServiceMock.Object)},
          new object[] {new LaserCutService(cutSettingsMock.Object, failureServiceMock.Object)}
        };
      }
    }

    public static IEnumerable<object[]> CutServicesTestCases
    {
      get
      {
        var imagePaths = new[]
        {
          @"./Tiles/00.bmp",
          @"./Tiles/01.bmp"
        };

        return imagePaths.SelectMany(x =>
          CutServices.Select(y => new[]
          {
            new CutServiceTestCase
            {
              ImagePath = x,
              Service = (ICutService) y.First()
            }
          }).ToArray());
      }
    }

    public class CutServiceTestCase
    {
      public string ImagePath { get; set; }
      public ICutService Service { get; set; }
    }
  }
}
