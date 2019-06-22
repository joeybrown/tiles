using System;
using System.Drawing;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Tiles.Infrastructure.Cut;
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
        .Setup(x => x.IsFailure(It.IsAny<int>()))
        .Returns(true);

      var scoreCutService = new ScoreCutService(cutSettingsMock.Object, failureServiceMock.Object);
      var layout = new Bitmap(100, 100);
      var tile = new Bitmap(100, 100);
      Func<Task> action = async () => await scoreCutService.CutTile(layout, tile);

      action.Should().NotThrow("failure rate per cut is 0");
    }
  }
}
