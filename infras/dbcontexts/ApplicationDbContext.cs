using evacPlanMoni.entities;
using Microsoft.EntityFrameworkCore;

namespace evacPlanMoni.infras.dbcontexts
{
  public class ApplicationDbContext : DbContext
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<EvacuationZone> EvacuationZones { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
  }
}