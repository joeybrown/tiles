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
    private const int TileWidth = 100;

    [Fact]
    public void Slice_Layout_With_No_Area()
    {
      const string inputPath = @"./Layouts/00.bmp";
      var layout = Image.FromFile(inputPath);

      var sut = new GridService();
      var result = sut.Slice(layout, TileWidth);
      var elements = result.GetElements();

      elements.Should().HaveCount(0);
    }

    [Fact]
    public void Slice_Layout_With_Area_Smaller_Than_Tile()
    {
      const string inputPath = @"./Layouts/01.bmp";
      var layout = Image.FromFile(inputPath);

      var sut = new GridService();
      var result = sut.Slice(layout, TileWidth);
      var elements = result.GetElements().ToArray();

      elements.Should().HaveCount(1);
      elements.First().Coordinate.X.Should().Be(0);
      elements.First().Coordinate.Y.Should().Be(0);
      new Bitmap(elements.First().Value).Height.Should().Be(42);
      new Bitmap(elements.First().Value).Width.Should().Be(46);
    }

    [Fact]
    public void Slice_Layout_With_Area_And_Layout_Same_Size_As_Tile()
    {
      const string inputPath = @"./Layouts/02.bmp";
      var layout = Image.FromFile(inputPath);

      var sut = new GridService();
      var result = sut.Slice(layout, TileWidth);
      var elements = result.GetElements().ToArray();

      elements.Should().HaveCount(1);
      elements.First().Coordinate.X.Should().Be(0);
      elements.First().Coordinate.Y.Should().Be(0);
      new Bitmap(elements.First().Value).Height.Should().Be(100);
      new Bitmap(elements.First().Value).Width.Should().Be(100);
    }

    [Fact]
    public void Slice_Layout_With_Area_Same_Size_As_Tile_And_Layout_Larger_Than_Tile()
    {
      const string inputPath = @"./Layouts/03.bmp";
      var layout = Image.FromFile(inputPath);

      var sut = new GridService();
      var result = sut.Slice(layout, TileWidth);
      var elements = result.GetElements().ToArray();

      elements.Should().HaveCount(1);
      elements.First().Coordinate.X.Should().Be(0);
      elements.First().Coordinate.Y.Should().Be(0);
      new Bitmap(elements.First().Value).Height.Should().Be(100);
      new Bitmap(elements.First().Value).Width.Should().Be(100);
    }

    [Fact]
    public void Slice_Layout_With_Area_Same_Size_As_Two_Tiles_Horizontal()
    {
      const string inputPath = @"./Layouts/04.bmp";
      var layout = Image.FromFile(inputPath);

      var sut = new GridService();
      var result = sut.Slice(layout, TileWidth);
      var elements = result.GetElements().ToArray();
      elements.Should().HaveCount(3);

      var firstElement = elements[0];
      var secondElement = elements[1];
      var thirdElement = elements[2];

      firstElement.Coordinate.X.Should().Be(0);
      firstElement.Coordinate.Y.Should().Be(0);

      secondElement.Coordinate.X.Should().Be(50);
      secondElement.Coordinate.Y.Should().Be(0);

      thirdElement.Coordinate.X.Should().Be(150);
      thirdElement.Coordinate.Y.Should().Be(0);

      new Bitmap(firstElement.Value).Height.Should().Be(100);
      new Bitmap(firstElement.Value).Width.Should().Be(50);

      new Bitmap(secondElement.Value).Height.Should().Be(100);
      new Bitmap(secondElement.Value).Width.Should().Be(100);

      new Bitmap(thirdElement.Value).Height.Should().Be(100);
      new Bitmap(thirdElement.Value).Width.Should().Be(50);
    }

    [Fact]
    public void Slice_Layout_With_Area_Same_Size_As_Two_Tiles_Vertical()
    {
      const string inputPath = @"./Layouts/05.bmp";
      var layout = Image.FromFile(inputPath);

      var sut = new GridService();
      var result = sut.Slice(layout, TileWidth);
      var elements = result.GetElements().ToArray();
      elements.Should().HaveCount(3);

      var firstElement = elements[0];
      var secondElement = elements[1];
      var thirdElement = elements[2];

      firstElement.Coordinate.X.Should().Be(0);
      firstElement.Coordinate.Y.Should().Be(0);

      secondElement.Coordinate.X.Should().Be(0);
      secondElement.Coordinate.Y.Should().Be(50);

      thirdElement.Coordinate.X.Should().Be(0);
      thirdElement.Coordinate.Y.Should().Be(150);

      new Bitmap(firstElement.Value).Height.Should().Be(50);
      new Bitmap(firstElement.Value).Width.Should().Be(100);

      new Bitmap(secondElement.Value).Height.Should().Be(100);
      new Bitmap(secondElement.Value).Width.Should().Be(100);

      new Bitmap(thirdElement.Value).Height.Should().Be(50);
      new Bitmap(thirdElement.Value).Width.Should().Be(100);
    }

    [Fact]
    public void Slice_Layout_With_Area_Shaped_Like_A_T()
    {
      const string inputPath = @"./Layouts/06.bmp";
      var layout = Image.FromFile(inputPath);

      var sut = new GridService();
      var result = sut.Slice(layout, TileWidth);
      var elements = result.GetElements().ToArray();
      elements.Should().HaveCount(9);
    }
  }
}