namespace Tiles.Infrastructure.Cut
{
  public interface ICutServiceSettings
  {
    int CutTimeMilisecondDelay { get; set; }
    decimal FailureRatePerCut { get; set; }
    int MaxFailures { get; set; }
  }

  public class CutServiceSettings : ICutServiceSettings
  {
    public int CutTimeMilisecondDelay { get; set; }
    public decimal FailureRatePerCut { get; set; }
    public int MaxFailures { get; set; }
  }
}
