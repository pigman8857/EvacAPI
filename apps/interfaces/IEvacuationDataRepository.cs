using evacPlanMoni.entities;

namespace evacPlanMoni.apps.interfaces
{
  public interface IEvacuationDataRepository
  {
    // Zone Methods
    void AddZone(EvacuationZone zone);
    IEnumerable<EvacuationZone> GetAllZones();

    // Vehicle Methods
    void AddVehicle(Vehicle vehicle);
    IEnumerable<Vehicle> GetAllVehicles();
    Vehicle GetVehicle(string vehicleId);
    void UpdateVehicle(Vehicle vehicle);

    // Utility
    void ClearData();
  }
}