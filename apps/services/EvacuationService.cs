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
    private readonly IEvacuationStatusRepository _repository;
    private readonly ILogger<EvacuationService> _logger;

    // In-memory stores for base data (in a real app, this would be a DB like SQL Server or CosmosDB)
    private static readonly List<EvacuationZone> Zones = new();
    private static readonly List<Vehicle> Vehicles = new();

    // Lock object for concurrency handling during planning
    private static readonly object _planningLock = new object();

    public EvacuationService(IEvacuationStatusRepository repository, IConnectionMultiplexer redis, ILogger<EvacuationService> logger)
    {
      _repository = repository;
      _logger = logger;
    }

    public void AddZone(EvacuationZone zone)
    {
      Zones.Add(zone);
      var status = new EvacuationStatus
      {
        ZoneId = zone.ZoneId,
        RemainingPeople = zone.TotalPeople,
        TotalEvacuated = 0
      };

      // Use repository instead of direct Redis calls
      _repository.SaveStatus(status);
      _logger.LogInformation($"Zone {zone.ZoneId} added.");
    }

    public void AddVehicle(Vehicle vehicle) => Vehicles.Add(vehicle);

    public List<EvacuationPlan> GeneratePlan()
    {
      lock (_planningLock) // Concurrency: Prevent simultaneous conflicting plans
      {
        var plans = new List<EvacuationPlan>();
        var currentStatuses = GetAllStatuses();

        // 1. Urgency Priority: Sort zones by Urgency (Highest first)
        var prioritizedZones = Zones
            .OrderByDescending(z => z.UrgencyLevel)
            .ToList();

        foreach (var zone in prioritizedZones)
        {
          var status = currentStatuses.FirstOrDefault(s => s.ZoneId == zone.ZoneId);
          if (status == null || status.RemainingPeople <= 0) continue;

          // 2. Capacity & Distance Optimization
          var availableVehicles = Vehicles.Where(v => v.IsAvailable).ToList();
          if (!availableVehicles.Any())
          {
            _logger.LogWarning("No available vehicles to handle remaining zones.");
            break; // No vehicles left
          }

          // Sort vehicles: First by capacity that can best handle the crowd, then by closest distance
          var bestVehicle = availableVehicles
              .OrderByDescending(v => v.Capacity >= status.RemainingPeople ? 1 : 0) // Prefer vehicles that can take everyone
              .ThenByDescending(v => v.Capacity) // Otherwise, get the biggest vehicle
              .ThenBy(v => GeoHelper.CalculateHaversineDistance(zone.Latitude, zone.Longitude, v.Latitude, v.Longitude))
              .First();

          var distance = GeoHelper.CalculateHaversineDistance(zone.Latitude, zone.Longitude, bestVehicle.Latitude, bestVehicle.Longitude);
          var eta = distance / bestVehicle.Speed; // Time = Distance / Speed

          var peopleToTake = Math.Min(status.RemainingPeople, bestVehicle.Capacity);

          var plan = new EvacuationPlan
          {
            ZoneId = zone.ZoneId,
            VehicleId = bestVehicle.VehicleId,
            ETAHours = Math.Round(eta, 2),
            PeopleToEvacuate = peopleToTake
          };

          plans.Add(plan);
          bestVehicle.IsAvailable = false; // Mark as dispatched

          _logger.LogInformation($"Assigned {bestVehicle.VehicleId} to {zone.ZoneId}. ETA: {plan.ETAHours}h. Evacuating: {peopleToTake}");
        }
        return plans;
      }
    }

    public void UpdateEvacuation(string zoneId, int evacuatedCount, string vehicleId)
    {
      var status = _repository.GetStatus(zoneId);
      if (status != null)
      {
        status.TotalEvacuated += evacuatedCount;
        status.RemainingPeople -= evacuatedCount;
        if (status.RemainingPeople < 0) status.RemainingPeople = 0;
        status.LastVehicleUsed = vehicleId;

        _repository.SaveStatus(status);

        var vehicle = Vehicles.FirstOrDefault(v => v.VehicleId == vehicleId);
        if (vehicle != null) vehicle.IsAvailable = true;
      }
    }

    public void ClearData()
    {
      Zones.Clear();
      Vehicles.Clear();
      _repository.ClearAllDatabase();
    }

    // --- Redis Helpers ---
    public List<EvacuationStatus> GetAllStatuses()
    {
      var zoneIds = Zones.Select(z => z.ZoneId);
      return _repository.GetAllStatuses(zoneIds);
    }

  }
}