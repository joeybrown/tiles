using System.Drawing;
using System.Linq;
using FluentAssertions;
using Moq;
using Tiles.Infrastructure.Grid;
using Xunit;

namespace Tiles.Infrastructure.UnitTest.Grid
{
  public class GridServiceTests
  {
    [Fact]
    public void Slice_Layout_With_No_Area()
    {
      const string inputPath = @"./Layouts/00.bmp";
      var layout = Image.FromFile(inputPath);

      var settings = new Mock<IGridServiceSettings>();

      var sut = new GridService(settings.Object);
      var result = sut.Slice(layout);
      var elements = result.GetElements();

      elements.Should().HaveCount(0);
    }

    [Fact]
    public void Slice_Layout_With_Area_Smaller_Than_Tile()
    {
      const string inputPath = @"./Layouts/01.bmp";
      var layout = Image.FromFile(inputPath);
      var settings = new Mock<IGridServiceSettings>();

      var sut = new GridService(settings.Object);
      var result = sut.Slice(layout);
      var elements = result.GetElements().ToArray();

      elements.Should().HaveCount(1);
      elements.First().Coordinate.X.Should().Be(0);
      elements.First().Coordinate.Y.Should().Be(0);
      elements.First().Value.Should().BeEquivalentTo(layout);
    }

    [Fact]
    public void Slice_Layout_With_Area_And_Layout_Same_Size_As_Tile()
    {
      const string inputPath = @"./Layouts/02.bmp";
      var layout = Image.FromFile(inputPath);
      var settings = new Mock<IGridServiceSettings>();

      var sut = new GridService(settings.Object);
      var result = sut.Slice(layout);
      var elements = result.GetElements();

      elements.Should().HaveCount(1);
      elements.First().Coordinate.X.Should().Be(0);
      elements.First().Coordinate.Y.Should().Be(0);
      elements.First().Value.Should().BeEquivalentTo(layout);
    }

    [Fact]
    public void Slice_Layout_With_Area_Same_Size_As_Tile_And_Layout_Larger_Than_Tile()
    {
      const string inputPath = @"./Layouts/03.bmp";
      var layout = Image.FromFile(inputPath);
      var settings = new Mock<IGridServiceSettings>();

      var sut = new GridService(settings.Object);
      var result = sut.Slice(layout);
      var elements = result.GetElements();

      elements.Should().HaveCount(1);
      elements.First().Coordinate.X.Should().Be(0);
      elements.First().Coordinate.Y.Should().Be(0);
      elements.First().Value.Should().BeEquivalentTo(layout);
    }
  }
}