using evacPlanMoni.apps.interfaces;
using evacPlanMoni.entities;
using evacPlanMoni.infras.dbcontexts;
using Microsoft.EntityFrameworkCore;

namespace evacPlanMoni.infras.repositories
{
  public class PostgresEvacuationDataRepository : IEvacuationDataRepository
  {
    private readonly EvacuationDbContext _context;

    // Inject the DbContext via Dependency Injection
    public PostgresEvacuationDataRepository(EvacuationDbContext context)
    {
      _context = context;
    }

    public void AddZone(EvacuationZone zone)
    {
      _context.EvacuationZones.Add(zone);
      _context.SaveChanges(); // Commits the insert to PostgreSQL
    }

    public IEnumerable<EvacuationZone> GetAllZones()
    {
      // AsNoTracking() is a performance boost for read-only operations
      return _context.EvacuationZones.AsNoTracking().ToList();
    }

    public void AddVehicle(Vehicle vehicle)
    {
      _context.Vehicles.Add(vehicle);
      _context.SaveChanges();
    }

    public IEnumerable<Vehicle> GetAllVehicles()
    {
      return _context.Vehicles.AsNoTracking().ToList();
    }

    public Vehicle GetVehicle(string vehicleId)
    {
      return _context.Vehicles.FirstOrDefault(v => v.VehicleId == vehicleId);
    }

    public void UpdateVehicle(Vehicle vehicle)
    {
      // EF Core tracks the entity, so we just call Update and SaveChanges
      _context.Vehicles.Update(vehicle);
      _context.SaveChanges();
    }

    public void ClearData()
    {
      // Clears all records from both tables. 
      // If you are using EF Core 7.0 or newer, ExecuteDelete() is the most efficient way to do bulk deletes.
      _context.EvacuationZones.ExecuteDelete();
      _context.Vehicles.ExecuteDelete();

      // Note: If you are on EF Core 6 or older, use this instead:
      // _context.EvacuationZones.RemoveRange(_context.EvacuationZones);
      // _context.Vehicles.RemoveRange(_context.Vehicles);
      // _context.SaveChanges();
    }
  }
}