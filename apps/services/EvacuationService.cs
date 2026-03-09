using evacPlanMoni.apps.helpers;
using evacPlanMoni.apps.interfaces;
using evacPlanMoni.entities;
using evacPlanMoni.presentations.dtos;
using evacPlanMoni.presentations.Services;
using StackExchange.Redis;

namespace evacPlanMoni.apps.Services
{
  public class EvacuationService : IEvacuationService
  {

    private readonly IEvacuationStatusRepository _statusRepository;
    private readonly IEvacuationDataRepository _dataRepository;
    private readonly ILogger<EvacuationService> _logger;

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
        RemainingPeople = zone.NumberOfPeople,
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

        //Sorting base on UrgencyLevel from DB
        var prioritizedZones = allZones.OrderByDescending(z => z.UrgencyLevel).ToList();

        //Current status of all zone.
        var currentStatuses = await _statusRepository.GetAllStatusesAsync(allZones.Select(z => z.ZoneId));

        //All available vehicles
        var availableVehicles = (await _dataRepository.GetAllVehiclesAsync()).Where(v => v.IsAvailable).ToList();
        if (!availableVehicles.Any())
        {
          _logger.LogWarning("No available vehicles to handle remaining zones.");
          return plans;
        }

        //Process each zones
        foreach (var zone in prioritizedZones)
        {
          //Get one in db and in status list. Let's see how it is going first.
          var zoneStatus = currentStatuses.FirstOrDefault(s => s.ZoneId == zone.ZoneId);

          //If that zone does not exist or there is people left in the zone. Just move on to the next zone
          if (zoneStatus == null || zoneStatus.RemainingPeople <= 0) continue;

          //hard part, the optimization


          //  var bestVehicle = availableVehicles
          // .OrderByDescending(v => v.Capacity >= zoneStatus.RemainingPeople ? 1 : 0)
          // .ThenByDescending(v => v.Capacity)
          // .ThenBy(v => GeoHelper.CalculateHaversineDistance(zone.Latitude, zone.Longitude, v.Latitude, v.Longitude))
          // .First();

          // Logic Check: 
          // If the zone has 50 people, prioritize finding a single vehicle with a capacity $\ge$ 50. 
          // If none exist, find the largest available vehicle to take the biggest chunk of people at once. 

          var bestVehicleLeastRemaining = availableVehicles
                    .Where(v => v.IsAvailable)
                    //// Prioritize vehicles that can fit everyone (True comes before False in descending order)  
                    .OrderBy(v =>
                      zoneStatus.RemainingPeople - v.Capacity
                    ).First();

          //Distance is in km
          var distance = GeoHelper.CalculateHaversineDistance(
            zone.Latitude,
            zone.Longitude,
            bestVehicleLeastRemaining.Latitude,
            bestVehicleLeastRemaining.Longitude);

          var eta = distance / bestVehicleLeastRemaining.Speed; //Speed = km/h  => eta is Hours (e.g., 0.25 hours)

          var peopleToTake = Math.Min(zoneStatus.RemainingPeople, bestVehicleLeastRemaining.Capacity);

          plans.Add(new EvacuationPlan
          {
            ZoneId = zone.ZoneId,
            VehicleId = bestVehicleLeastRemaining.VehicleId,
            ETA = ETAHelper.GetFormattedEta(Math.Round(eta, 2)),
            NumberOfPeople = peopleToTake
          });

          // // Update vehicle availability via the repository
          bestVehicleLeastRemaining.IsAvailable = false;
          await _dataRepository.UpdateVehicleAsync(bestVehicleLeastRemaining);

          _logger.LogInformation($"Assigned {bestVehicleLeastRemaining.VehicleId} to {zone.ZoneId}. ETA: {Math.Round(eta, 2)}h.");
        }
        return plans;
      }
      finally
      {
        // CRITICAL: Always release the semaphore in a finally block so API won't get permanently locked. 
        _semaphore.Release();
      }
    }

    public async Task UpdateEvacuation(UpdateEvacuationStatusDto updateEvacStatusDto)//(string zoneId, int evacuatedCount, string vehicleId)
    {
      var status = await _statusRepository.GetStatusAsync(updateEvacStatusDto.ZoneId);

      // We can check if status is null then we logged and then leave by returning or throwing 
      // but we will let it be.

      if (status != null)
      {
        status.TotalEvacuated += updateEvacStatusDto.EvacuatedCount;
        status.RemainingPeople -= updateEvacStatusDto.EvacuatedCount;
        if (status.RemainingPeople < 0) status.RemainingPeople = 0;
        status.LastVehicleUsed = updateEvacStatusDto.VehicleId;

        await _statusRepository.SaveStatusAsync(status);

        // Free up the vehicle via the repository
        var vehicle = await _dataRepository.GetVehicle(updateEvacStatusDto.VehicleId);
        if (vehicle != null)
        {
          vehicle.IsAvailable = true;
          await _dataRepository.UpdateVehicleAsync(vehicle);
        }

        _logger.LogInformation($"Updated Zone {updateEvacStatusDto.ZoneId}. Remaining: {status.RemainingPeople}");
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