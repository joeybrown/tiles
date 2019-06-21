namespace Tiles.Infrastructure.Grid
{
  public interface ICoordinate
  {
    int X { get; set; }
    int Y { get; set; }
  }

  public class Coordinate : ICoordinate
  {
    public Coordinate(int x, int y)
    {
      X = x;
      Y = y;
    }

    public int X { get; set; }
    public int Y { get; set; }
  }

  public class ColoredCoordinate : ICoordinate
  {
    public ColoredCoordinate(int x, int y)
    {
      X = x;
      Y = y;
    }

    public int X { get; set; }
    public int Y { get; set; }
  }

  public class EmptyCoordinate : ICoordinate
  {
    public EmptyCoordinate(int x, int y)
    {
      X = x;
      Y = y;
    }

    public int X { get; set; }
    public int Y { get; set; }
  }
}