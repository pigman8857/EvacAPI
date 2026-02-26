using evacPlanMoni.apps.Services;
using evacPlanMoni.entities;
using Microsoft.AspNetCore.Mvc;

namespace evacPlanMoni.presentations.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class EvacuationsController : ControllerBase
  {
    private readonly EvacuationService _service;

    public EvacuationsController(EvacuationService service)
    {
      _service = service;
    }

    [HttpPost("/api/evacuation-zones")]
    public IActionResult AddZone([FromBody] EvacuationZone zone)
    {
      _service.AddZone(zone);
      return Ok(new { Message = "Zone added successfully" });
    }

    [HttpPost("/api/vehicles")]
    public IActionResult AddVehicle([FromBody] Vehicle vehicle)
    {
      _service.AddVehicle(vehicle);
      return Ok(new { Message = "Vehicle added successfully" });
    }

    [HttpPost("plan")]
    public IActionResult GeneratePlan()
    {
      var plan = _service.GeneratePlan();
      if (!plan.Any())
        return BadRequest(new { Message = "Cannot generate plan. Ensure zones have people and vehicles are available." });

      return Ok(plan);
    }

    [HttpGet("status")]
    public IActionResult GetStatus()
    {
      return Ok(_service.GetAllStatuses());
    }

    [HttpPut("update")]
    public IActionResult UpdateStatus(string zoneId, int evacuatedCount, string vehicleId)
    {
      _service.UpdateEvacuation(zoneId, evacuatedCount, vehicleId);
      return Ok(new { Message = "Status updated successfully" });
    }

    [HttpDelete("clear")]
    public IActionResult Clear()
    {
      _service.ClearData();
      return Ok(new { Message = "Data reset successfully" });
    }
  }
}