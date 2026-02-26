namespace evacPlanMoni.presentations.dtos;

public class AddEvacuationZone
{
  public string ZoneId { get; set; }
  public double Latitude { get; set; }
  public double Longitude { get; set; }
  public int TotalPeople { get; set; }
  public int UrgencyLevel { get; set; } // 1 (Low) to 5 (High)
}