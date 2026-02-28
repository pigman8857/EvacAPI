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

    public async Task AddZone(EvacuationZone zone)
    {
      _context.EvacuationZones.Add(zone);
      await _context.SaveChangesAsync(); // Commits the insert to PostgreSQL
    }

    public async Task<IEnumerable<EvacuationZone>> GetAllZones()
    {
      // AsNoTracking() is a performance boost for read-only operations
      return await _context.EvacuationZones.AsNoTracking().ToListAsync();
    }

    public async Task AddVehicle(Vehicle vehicle)
    {
      _context.Vehicles.Add(vehicle);
      await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Vehicle>> GetAllVehicles()
    {
      return _context.Vehicles.AsNoTracking().ToList();
    }

    public async Task<Vehicle?> GetVehicle(string vehicleId)
    {
      return await _context.Vehicles.FirstOrDefaultAsync(v => v.VehicleId == vehicleId);
    }

    public async Task UpdateVehicle(Vehicle vehicle)
    {
      // EF Core tracks the entity, so we just call Update and SaveChanges
      _context.Vehicles.Update(vehicle);
      await _context.SaveChangesAsync();
    }

    public async Task ClearData()
    {

      // ExecuteDelete performs a single, highly efficient database-level 
      // DELETE operation without loading entities into memory, 
      // whereas RemoveRange loads all entities into memory 
      // first and then issues a separate DELETE statement for each entity 

      // Clears all records from both tables. 
      // EF Core 7.0
      await _context.EvacuationZones.ExecuteDeleteAsync();
      await _context.Vehicles.ExecuteDeleteAsync();

      // Note:EF Core 6 or older
      // await _context.EvacuationZones.RemoveRangeAsync(_context.EvacuationZones);
      // await _context.Vehicles.RemoveRangeAsync(_context.Vehicles);
      // await _context.SaveChangesAsync();
    }
  }
}