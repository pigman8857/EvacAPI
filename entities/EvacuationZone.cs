namespace evacPlanMoni.entities;

public class EvacuationZone
{
  public string ZoneId { get; set; }
  public double Latitude { get; set; }
  public double Longitude { get; set; }
  public int NumberOfPeople { get; set; }
  public int UrgencyLevel { get; set; } // 1 (Low) to 5 (High)
}