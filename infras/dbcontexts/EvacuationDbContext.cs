using evacPlanMoni.entities;
using Microsoft.EntityFrameworkCore;

namespace evacPlanMoni.infras.dbcontexts
{
  public class EvacuationDbContext : DbContext
  {
    public EvacuationDbContext(DbContextOptions<EvacuationDbContext> options)
        : base(options) { }

    public DbSet<EvacuationZone> EvacuationZones { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // Automatically applies all configurations in the same assembly
      modelBuilder.ApplyConfigurationsFromAssembly(typeof(EvacuationDbContext).Assembly);
    }
  }
}