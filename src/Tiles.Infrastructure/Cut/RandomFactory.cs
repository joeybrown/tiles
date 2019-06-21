using System;
using System.Collections.Generic;
using System.Text;

namespace Tiles.Infrastructure.Cut
{
  public interface IRandomFactory
  {
    int GetRandomNumber(int max);
  }

  public class RandomFactory: IRandomFactory
  {
    public int GetRandomNumber(int max)
    {

      var random = new Random();
      return random.Next(max);
    }
  }
}
