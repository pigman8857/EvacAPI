using System.ComponentModel.DataAnnotations;

namespace evacPlanMoni.presentations.dtos;

public class LocationCoordinates
{
  [Required]
  public double latitude { get; set; }
  [Required]
  public double longitude { get; set; }
}