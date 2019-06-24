using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Tiles.Console.FunctionalTests
{
  public class MainTests
  {
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
      Func<Task> run = () => Program.Main(args);
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
      Func<Task> run = () => Program.Main(args);
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
      Func<Task> run = () => Program.Main(args);
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
      Func<Task> run = () => Program.Main(args);
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
      Func<Task> run = () => Program.Main(args);
      run.Should().NotThrow();
    }
  }
}
