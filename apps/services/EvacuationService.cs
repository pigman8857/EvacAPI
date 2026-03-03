using evacPlanMoni.apps.helpers;
using evacPlanMoni.apps.interfaces;
using evacPlanMoni.entities;
using evacPlanMoni.presentations.Services;
using StackExchange.Redis;

namespace evacPlanMoni.apps.Services
{
  public class EvacuationService : IEvacuationService
  {
    // I got `CS1996 - Cannot await in the body of a lock statement` when i tried changed all method to async
    // and changed methods in IEvacuationStatusRepository and IEvacuationDataRepository to async
    // Refer from these resources: 

    // https://medium.com/@tyschenk20/mixing-traditional-locks-with-async-code-in-c-27431f857e01
    // https://www.rocksolidknowledge.com/articles/locking-asyncawait

    // I have to remove lock() {} block and using SemaphoreSlim instead.
    //  The (1, 1) means only 1 thread can access the resource at a time, exactly like a standard lock.
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    // Lock object for concurrency handling during planning
    //private static readonly object _planningLock = new object();
    private readonly IEvacuationStatusRepository _statusRepository;
    private readonly ILogger<EvacuationService> _logger;
    private readonly IEvacuationDataRepository _dataRepository;

    public EvacuationService(IEvacuationDataRepository dataRepository, IEvacuationStatusRepository statusRepository, IConnectionMultiplexer redis, ILogger<EvacuationService> logger)
    {
      _dataRepository = dataRepository;
      _statusRepository = statusRepository;
      _logger = logger;
    }

    public async Task AddZone(EvacuationZone zone)
    {
      await _dataRepository.AddZoneAsync(zone);

      var status = new EvacuationStatus
      {
        ZoneId = zone.ZoneId,
        RemainingPeople = zone.TotalPeople,
        TotalEvacuated = 0
      };

      await _statusRepository.SaveStatusAsync(status);
      _logger.LogInformation($"Zone {zone.ZoneId} added.");
    }

    public async Task AddVehicle(Vehicle vehicle)
    {
      await _dataRepository.AddVehicleAsync(vehicle);
      _logger.LogInformation($"Vehicle {vehicle.VehicleId} added.");
    }

    public async Task<List<EvacuationPlan>> GeneratePlan()
    {
      await _semaphore.WaitAsync();
      // lock (_planningLock)
      try
      {
        var plans = new List<EvacuationPlan>();

        var allZones = await _dataRepository.GetAllZonesAsync();
        var currentStatuses = await _statusRepository.GetAllStatusesAsync(allZones.Select(z => z.ZoneId));

        var prioritizedZones = allZones.OrderByDescending(z => z.UrgencyLevel).ToList();

        foreach (var zone in prioritizedZones)
        {
          var status = currentStatuses.FirstOrDefault(s => s.ZoneId == zone.ZoneId);
          if (status == null || status.RemainingPeople <= 0) continue;

          var availableVehicles = (await _dataRepository.GetAllVehiclesAsync()).Where(v => v.IsAvailable).ToList();
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
          await _dataRepository.UpdateVehicleAsync(bestVehicle);

          _logger.LogInformation($"Assigned {bestVehicle.VehicleId} to {zone.ZoneId}. ETA: {Math.Round(eta, 2)}h.");
        }
        return plans;
      }
      finally
      {
        // CRITICAL: Always release the semaphore in a finally block so API won't get permanently locked. 
        _semaphore.Release();
      }
    }

    public async Task UpdateEvacuation(string zoneId, int evacuatedCount, string vehicleId)
    {
      var status = await _statusRepository.GetStatusAsync(zoneId);

      // We can check if status is null then we logged and then leave by returning or throwing 
      // but we will let it be.

      if (status != null)
      {
        status.TotalEvacuated += evacuatedCount;
        status.RemainingPeople -= evacuatedCount;
        if (status.RemainingPeople < 0) status.RemainingPeople = 0;
        status.LastVehicleUsed = vehicleId;

        await _statusRepository.SaveStatusAsync(status);

        // Free up the vehicle via the repository
        var vehicle = await _dataRepository.GetVehicle(vehicleId);
        if (vehicle != null)
        {
          vehicle.IsAvailable = true;
          await _dataRepository.UpdateVehicleAsync(vehicle);
        }

        _logger.LogInformation($"Updated Zone {zoneId}. Remaining: {status.RemainingPeople}");
      }
    }

    public async Task ClearData()
    {
      await _dataRepository.ClearDataAsync();
      await _statusRepository.ClearAllDatabaseAsync();
    }

    public async Task<List<EvacuationStatus>> GetAllStatuses()
    {
      // Get the zones from the new Data Repository instead of the static list
      var allZones = await _dataRepository.GetAllZonesAsync();

      // Extract just the IDs
      var zoneIds = allZones.Select(z => z.ZoneId);

      // Fetch the statuses from the Status Repository (which handles the Redis logic)
      return await _statusRepository.GetAllStatusesAsync(zoneIds);
    }

  }
}