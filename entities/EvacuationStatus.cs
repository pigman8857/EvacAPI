namespace evacPlanMoni.entities;

public class EvacuationStatus
{
  public string ZoneId { get; set; }
  public int TotalEvacuated { get; set; }
  public int RemainingPeople { get; set; }
  public string LastVehicleUsed { get; set; }
}