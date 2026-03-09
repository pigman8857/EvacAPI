using System.ComponentModel.DataAnnotations;

namespace evacPlanMoni.presentations.dtos;

public class UpdateEvacuationStatusDto
{
  [Required]
  public string ZoneId { get; set; }
  [Required]
  public int EvacuatedCount { get; set; }
  [Required]
  public string VehicleId { get; set; }
}

