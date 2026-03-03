using evacPlanMoni.entities;

namespace evacPlanMoni.apps.interfaces
{
  public interface IEvacuationDataRepository
  {
    // Zone Methods
    Task AddZoneAsync(EvacuationZone zone);
    Task<IEnumerable<EvacuationZone>> GetAllZonesAsync();

    // Vehicle Methods
    Task AddVehicleAsync(Vehicle vehicle);
    Task<IEnumerable<Vehicle>> GetAllVehiclesAsync();
    Task<Vehicle?> GetVehicle(string vehicleId);
    Task UpdateVehicleAsync(Vehicle vehicle);

    // Utility
    Task ClearDataAsync();
  }
}