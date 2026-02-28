using evacPlanMoni.entities;

namespace evacPlanMoni.apps.interfaces
{
  public interface IEvacuationDataRepository
  {
    // Zone Methods
    Task AddZone(EvacuationZone zone);
    Task<IEnumerable<EvacuationZone>> GetAllZones();

    // Vehicle Methods
    Task AddVehicle(Vehicle vehicle);
    Task<IEnumerable<Vehicle>> GetAllVehicles();
    Task<Vehicle?> GetVehicle(string vehicleId);
    Task UpdateVehicle(Vehicle vehicle);

    // Utility
    Task ClearData();
  }
}