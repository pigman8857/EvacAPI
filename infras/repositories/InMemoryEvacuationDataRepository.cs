using evacPlanMoni.apps.interfaces;
using evacPlanMoni.entities;


namespace evacPlanMoni.infras.repositories
{
  public class InMemoryEvacuationDataRepository : IEvacuationDataRepository
  {
    // Moved from the Service to the Repository
    // In-memory stores for base data (in a real app, this would be a DB like SQL Server or CosmosDB)
    private static readonly List<EvacuationZone> Zones = new();
    private static readonly List<Vehicle> Vehicles = new();

    public void AddZone(EvacuationZone zone) => Zones.Add(zone);

    public IEnumerable<EvacuationZone> GetAllZones() => Zones;

    public void AddVehicle(Vehicle vehicle) => Vehicles.Add(vehicle);

    public IEnumerable<Vehicle> GetAllVehicles() => Vehicles;

    public Vehicle GetVehicle(string vehicleId) => Vehicles.FirstOrDefault(v => v.VehicleId == vehicleId);

    public void UpdateVehicle(Vehicle vehicle)
    {
      // In a real database (like EF Core), we would do an Update/Save operation here.
      // Since this is in-memory and we are passing by reference, the object is already updated.
      // But having this method ensures our Service doesn't rely on in-memory side effects!
      var existing = GetVehicle(vehicle.VehicleId);
      if (existing != null)
      {
        existing.IsAvailable = vehicle.IsAvailable;
      }
    }

    public void ClearData()
    {
      Zones.Clear();
      Vehicles.Clear();
    }
  }
}