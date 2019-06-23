using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using FluentAssertions;
using Tiles.Infrastructure.Grid;
using Tiles.Infrastructure.Lay;
using Xunit;

namespace Tiles.Infrastructure.UnitTest.Lay
{
  public class TileLayServiceTests
  {
    [Fact]
    public void Should_Lay_Single_Tile_Successfully()
    {
      var tileService = new TileLayService();
      var tile = new Bitmap(Image.FromFile(@"./Tiles/01.bmp"));
      var gridElements = new[]
      {
        new GridElement(new Coordinate(0, 0), tile),
      };
      var grid = new Infrastructure.Grid.Grid(gridElements, 100, 100);
      var result = tileService.LayTile(grid);

      var bitmap = new Bitmap(result);
      var pixelsAreEqual = new List<bool>();

      var expected = new Bitmap(Image.FromFile(@"./Tiles/01.bmp"));

      List<bool> Func(List<bool> accumulator, int x, int y)
      {
        var resultPixel = bitmap.GetPixel(x, y);
        var expectedPixel = expected.GetPixel(x, y);
        accumulator.Add(resultPixel.Name == expectedPixel.Name);
        return accumulator;
      }

      pixelsAreEqual = bitmap.ForEachPixel(pixelsAreEqual, Func, Func);

      pixelsAreEqual.Count.Should().Be(10000, "there are 10000 pixels");
      pixelsAreEqual.Any(x => !x).Should().BeFalse("all pixels should be equivalent");
    }

    [Fact]
    public void Should_Lay_Three_Tiles_Successfully()
    {
      var tileService = new TileLayService();
      var fullTile = new Bitmap(Image.FromFile(@"./Tiles/01.bmp"));
      var halfTile = new Bitmap(Image.FromFile(@"./Tiles/03.bmp"));
      var gridElements = new[]
      {
        new GridElement(new Coordinate(0, 0), halfTile),
        new GridElement(new Coordinate(50, 0), fullTile),
        new GridElement(new Coordinate(150, 0), halfTile),
      };
      var grid = new Infrastructure.Grid.Grid(gridElements, 200, 100);
      var result = tileService.LayTile(grid);
      
      result.Save("result.bmp", ImageFormat.Bmp);

      var bitmap = new Bitmap(result);
      var pixelsAreEqual = new List<bool>();

      var expected = new Bitmap(Image.FromFile(@"./Completed/00.bmp"));

      List<bool> Func(List<bool> accumulator, int x, int y)
      {
        var resultPixel = bitmap.GetPixel(x, y);
        var expectedPixel = expected.GetPixel(x, y);
        accumulator.Add(resultPixel.Name == expectedPixel.Name);
        return accumulator;
      }

      pixelsAreEqual = bitmap.ForEachPixel(pixelsAreEqual, Func, Func);

      pixelsAreEqual.Count.Should().Be(20000, "there are 20000 pixels");
      pixelsAreEqual.Any(x => !x).Should().BeFalse("all pixels should be equivalent");
    }
  }
}
