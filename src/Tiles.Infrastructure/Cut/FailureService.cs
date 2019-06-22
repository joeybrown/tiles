using System;

namespace Tiles.Infrastructure.Cut
{
  public interface IFailureService
  {
    bool IsFailure(decimal failureRatePerCut);
  }

  public class FailureService : IFailureService
  {
    public bool IsFailure(decimal failureRatePerCut)
    {
      if (failureRatePerCut == 0)
      {
        return false;
      }

      var cutsPerFailure = (int) (1 / failureRatePerCut);
      var random = new Random();
      var randomNumber = random.Next(cutsPerFailure);
      return randomNumber == 1;
    }
  }
}