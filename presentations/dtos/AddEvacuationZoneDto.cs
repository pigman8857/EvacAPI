using System.ComponentModel.DataAnnotations;

namespace evacPlanMoni.presentations.dtos;

public class AddEvacuationZoneDto
{
  [Required]
  public string ZoneId { get; set; }
  [Required]
  public double Latitude { get; set; }
  [Required]
  public double Longitude { get; set; }
  [Required]
  [MinLength(0)]
  public int TotalPeople { get; set; }
  [Range(1, 5)]
  public int UrgencyLevel { get; set; } // 1 (Low) to 5 (High)
}