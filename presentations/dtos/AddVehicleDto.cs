namespace evacPlanMoni.presentations.dtos;

public class AddVehicle
{
  public string VehicleId { get; set; }
  public int Capacity { get; set; }
  public string Type { get; set; }
  public double Latitude { get; set; }
  public double Longitude { get; set; }
  public double Speed { get; set; } // km/h
  public bool IsAvailable { get; set; } = true;
}