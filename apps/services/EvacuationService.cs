using evacPlanMoni.apps.helpers;
using evacPlanMoni.apps.interfaces;
using evacPlanMoni.entities;
using evacPlanMoni.presentation.Services;
using StackExchange.Redis;
using System.Text.Json;

namespace evacPlanMoni.apps.Services
{
  public class EvacuationService : IEvacuationService
  {
    private readonly IEvacuationStatusRepository _statusRepository;
    private readonly ILogger<EvacuationService> _logger;

    private readonly IEvacuationDataRepository _dataRepository;

    // Lock object for concurrency handling during planning
    private static readonly object _planningLock = new object();

    public EvacuationService(IEvacuationDataRepository dataRepository, IEvacuationStatusRepository statusRepository, IConnectionMultiplexer redis, ILogger<EvacuationService> logger)
    {
      _dataRepository = dataRepository;
      _statusRepository = statusRepository;
      _logger = logger;
    }

    public void AddZone(EvacuationZone zone)
    {
      _dataRepository.AddZone(zone);

      var status = new EvacuationStatus
      {
        ZoneId = zone.ZoneId,
        RemainingPeople = zone.TotalPeople,
        TotalEvacuated = 0
      };

      _statusRepository.SaveStatus(status);
      _logger.LogInformation($"Zone {zone.ZoneId} added.");
    }

    public void AddVehicle(Vehicle vehicle)
    {
      _dataRepository.AddVehicle(vehicle);
      _logger.LogInformation($"Vehicle {vehicle.VehicleId} added.");
    }

    public List<EvacuationPlan> GeneratePlan()
    {
      lock (_planningLock)
      {
        var plans = new List<EvacuationPlan>();

        var allZones = _dataRepository.GetAllZones();
        var currentStatuses = _statusRepository.GetAllStatuses(allZones.Select(z => z.ZoneId));

        var prioritizedZones = allZones.OrderByDescending(z => z.UrgencyLevel).ToList();

        foreach (var zone in prioritizedZones)
        {
          var status = currentStatuses.FirstOrDefault(s => s.ZoneId == zone.ZoneId);
          if (status == null || status.RemainingPeople <= 0) continue;

          var availableVehicles = _dataRepository.GetAllVehicles().Where(v => v.IsAvailable).ToList();
          if (!availableVehicles.Any())
          {
            _logger.LogWarning("No available vehicles to handle remaining zones.");
            break;
          }

          var bestVehicle = availableVehicles
              .OrderByDescending(v => v.Capacity >= status.RemainingPeople ? 1 : 0)
              .ThenByDescending(v => v.Capacity)
              .ThenBy(v => GeoHelper.CalculateHaversineDistance(zone.Latitude, zone.Longitude, v.Latitude, v.Longitude))
              .First();

          var distance = GeoHelper.CalculateHaversineDistance(zone.Latitude, zone.Longitude, bestVehicle.Latitude, bestVehicle.Longitude);
          var eta = distance / bestVehicle.Speed;

          var peopleToTake = Math.Min(status.RemainingPeople, bestVehicle.Capacity);

          plans.Add(new EvacuationPlan
          {
            ZoneId = zone.ZoneId,
            VehicleId = bestVehicle.VehicleId,
            ETAHours = Math.Round(eta, 2),
            PeopleToEvacuate = peopleToTake
          });

          // Update vehicle availability via the repository
          bestVehicle.IsAvailable = false;
          _dataRepository.UpdateVehicle(bestVehicle);

          _logger.LogInformation($"Assigned {bestVehicle.VehicleId} to {zone.ZoneId}. ETA: {Math.Round(eta, 2)}h.");
        }
        return plans;
      }
    }

    public void UpdateEvacuation(string zoneId, int evacuatedCount, string vehicleId)
    {
      var status = _statusRepository.GetStatus(zoneId);
      if (status != null)
      {
        status.TotalEvacuated += evacuatedCount;
        status.RemainingPeople -= evacuatedCount;
        if (status.RemainingPeople < 0) status.RemainingPeople = 0;
        status.LastVehicleUsed = vehicleId;

        _statusRepository.SaveStatus(status);

        // Free up the vehicle via the repository
        var vehicle = _dataRepository.GetVehicle(vehicleId);
        if (vehicle != null)
        {
          vehicle.IsAvailable = true;
          _dataRepository.UpdateVehicle(vehicle);
        }

        _logger.LogInformation($"Updated Zone {zoneId}. Remaining: {status.RemainingPeople}");
      }
    }

    public void ClearData()
    {
      _dataRepository.ClearData();
      _statusRepository.ClearAllDatabase();
    }

    public List<EvacuationStatus> GetAllStatuses()
    {
      // 1. Get the zones from the new Data Repository instead of the static list
      var allZones = _dataRepository.GetAllZones();

      // 2. Extract just the IDs
      var zoneIds = allZones.Select(z => z.ZoneId);

      // 3. Fetch the statuses from the Status Repository (which handles the Redis logic)
      return _statusRepository.GetAllStatuses(zoneIds);
    }

  }
}