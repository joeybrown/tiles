using System;
using System.Collections.Generic;
using System.Text;

namespace Tiles.Infrastructure.Grid
{
  public interface ICoordinate
  {
    int X { get; set; }
    int Y { get; set; }
  }

  public class Coordinate : ICoordinate
  {
    public int X { get; set; }
    public int Y { get; set; }

    public Coordinate(int x, int y)
    {
      X = x;
      Y = y;
    }
  }

  public class ColoredCoordinate : ICoordinate
  {
    public int X { get; set; }
    public int Y { get; set; }

    public ColoredCoordinate(int x, int y)
    {
      X = x;
      Y = y;
    }
  }

  public class EmptyCoordinate : ICoordinate
  {
    public int X { get; set; }
    public int Y { get; set; }

    public EmptyCoordinate(int x, int y)
    {
      X = x;
      Y = y;
    }
  }
}
