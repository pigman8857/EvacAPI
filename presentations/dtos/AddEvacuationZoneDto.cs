using System.ComponentModel.DataAnnotations;

namespace evacPlanMoni.presentations.dtos;

public class AddEvacuationZoneDto
{
  [Required]
  public string ZoneId { get; set; }
  [Required]
  public LocationCoordinates LocationCoordinates { get; set; }
  [Required]
  [Range(0, int.MaxValue, ErrorMessage = "TotalPeople must be 0 or greater.")]
  public int NumberOfPeople { get; set; }
  [Range(1, 5)]
  public int UrgencyLevel { get; set; } // 1 (Low) to 5 (High)
}