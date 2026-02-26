using AutoMapper;
using evacPlanMoni.entities;
using evacPlanMoni.presentations.dtos;
using evacPlanMoni.presentations.Services;
using Microsoft.AspNetCore.Mvc;

namespace evacPlanMoni.presentations.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class EvacuationsController : ControllerBase
  {
    private readonly IEvacuationService _service;
    private readonly IMapper _mapper;

    public EvacuationsController(IMapper mapper, IEvacuationService service)
    {
      _mapper = mapper;
      _service = service;
    }

    [HttpPost("/api/evacuation-zones")]
    public IActionResult AddZone([FromBody] AddEvacuationZoneDto zone)
    {
      _service.AddZone(_mapper.Map<EvacuationZone>(zone));
      return Ok(new { Message = "Zone added successfully" });
    }

    [HttpPost("/api/vehicles")]
    public IActionResult AddVehicle([FromBody] AddVehicleDto vehicle)
    {
      _service.AddVehicle(_mapper.Map<Vehicle>(vehicle));
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