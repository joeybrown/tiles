namespace Tiles.Infrastructure.Cut
{
  public interface ICutServiceSettings
  {
    int CutTimeMilisecondDelay { get; set; }
    decimal FailureRatePerCut { get; set; }
    int MaxFailures { get; set; }
  }
}
