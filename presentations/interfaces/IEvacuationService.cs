using evacPlanMoni.entities;

namespace evacPlanMoni.presentations.Services
{
  public interface IEvacuationService
  {
    /// <summary>
    /// Adds a new evacuation zone and initializes its status in Redis.
    /// </summary>
    Task AddZone(EvacuationZone zone);

    /// <summary>
    /// Registers a new vehicle into the available fleet.
    /// </summary>
    Task AddVehicle(Vehicle vehicle);

    /// <summary>
    /// Generates an optimized evacuation plan based on urgency, vehicle capacity, and distance.
    /// </summary>
    Task<List<EvacuationPlan>> GeneratePlan();

    /// <summary>
    /// Retrieves the real-time status of all evacuation zones from Redis.
    /// </summary>
    Task<List<EvacuationStatus>> GetAllStatuses();

    /// <summary>
    /// Updates the evacuation progress for a specific zone and frees up the assigned vehicle.
    /// </summary>
    Task UpdateEvacuation(string zoneId, int evacuatedCount, string vehicleId);

    /// <summary>
    /// Clears all in-memory data and flushes the Redis database.
    /// </summary>
    Task ClearData();
  }
}