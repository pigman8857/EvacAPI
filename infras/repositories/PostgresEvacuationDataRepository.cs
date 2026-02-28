using evacPlanMoni.apps.interfaces;
using evacPlanMoni.entities;
using evacPlanMoni.infras.dbcontexts;
using Microsoft.EntityFrameworkCore;

namespace evacPlanMoni.infras.repositories
{
  public class PostgresEvacuationDataRepository : IEvacuationDataRepository
  {
    private readonly EvacuationDbContext _evacContext;

    // Inject the DbContext via Dependency Injection
    public PostgresEvacuationDataRepository(EvacuationDbContext context)
    {
      _evacContext = context;
    }

    public async Task AddZone(EvacuationZone zone)
    {
      _evacContext.EvacuationZones.Add(zone);
      await _evacContext.SaveChangesAsync(); // Commits the insert to PostgreSQL
    }

    public async Task<IEnumerable<EvacuationZone>> GetAllZones()
    {
      // AsNoTracking() is a performance boost for read-only operations
      return await _evacContext.EvacuationZones.AsNoTracking().ToListAsync();
    }

    public async Task AddVehicle(Vehicle vehicle)
    {
      _evacContext.Vehicles.Add(vehicle);
      await _evacContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Vehicle>> GetAllVehicles()
    {
      return _evacContext.Vehicles.AsNoTracking().ToList();
    }

    public async Task<Vehicle?> GetVehicle(string vehicleId)
    {
      return await _evacContext.Vehicles.FirstOrDefaultAsync(v => v.VehicleId == vehicleId);
    }

    public async Task UpdateVehicle(Vehicle vehicle)
    {
      // EF Core tracks the entity, so we just call Update and SaveChanges
      _evacContext.Vehicles.Update(vehicle);
      await _evacContext.SaveChangesAsync();
    }

    public async Task ClearData()
    {

      // ExecuteDelete performs a single, highly efficient database-level 
      // DELETE operation without loading entities into memory, 
      // whereas RemoveRange loads all entities into memory 
      // first and then issues a separate DELETE statement for each entity.

      // It is a superior method for most bulk deletion scenarios 
      // where you can define the entities to be deleted using a LINQ query 
      // (e.g., a Where clause). It is significantly faster and uses less memory 
      // because it performs the operation directly on the database server.


      // Clears all records from both tables. 
      // EF Core 7.0
      await _evacContext.EvacuationZones.ExecuteDeleteAsync();
      await _evacContext.Vehicles.ExecuteDeleteAsync();

      // All .Net Versions have RemoveRangeAsync
      // await _context.EvacuationZones.RemoveRangeAsync(_context.EvacuationZones);
      // await _context.Vehicles.RemoveRangeAsync(_context.Vehicles);
      // await _context.SaveChangesAsync();
    }
  }
}