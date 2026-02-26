using System.ComponentModel.DataAnnotations;

namespace evacPlanMoni.presentations.dtos;

public class AddVehicleDto
{
  [Required]
  public string VehicleId { get; set; }
  [Required]
  public int Capacity { get; set; }
  [Required]
  [RegularExpression("^(Van|Bus|Boat)$", ErrorMessage = "Vehicle Type must be Van, Bus, or Boat.")]
  public string Type { get; set; }
  [Required]
  public double Latitude { get; set; }
  [Required]
  public double Longitude { get; set; }
  [Required]
  public double Speed { get; set; } // km/h
}