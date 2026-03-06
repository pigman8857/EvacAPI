using evacPlanMoni.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace evacPlanMoni.entities.configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
       public void Configure(EntityTypeBuilder<Vehicle> builder)
       {
              builder.ToTable("Vehicles");

              // Primary Key
              builder.HasKey(v => v.VehicleId);

              // Properties
              builder.Property(v => v.VehicleId)
                     .HasMaxLength(50)
                     .IsRequired();

              builder.Property(v => v.Type)
                     .HasMaxLength(50)
                     .IsRequired();

              builder.Property(v => v.Capacity).IsRequired();
              builder.Property(v => v.Latitude).IsRequired();
              builder.Property(v => v.Longitude).IsRequired();
              builder.Property(v => v.Speed).IsRequired();

              builder.Property(v => v.IsAvailable)
                     .IsRequired()
                     .HasDefaultValue(true);
       }
}

public class EvacuationZoneConfiguration : IEntityTypeConfiguration<EvacuationZone>
{
       public void Configure(EntityTypeBuilder<EvacuationZone> builder)
       {
              builder.ToTable("EvacuationZones");

              // Primary Key
              builder.HasKey(z => z.ZoneId);

              // Properties
              builder.Property(z => z.ZoneId)
                     .HasMaxLength(50)
                     .IsRequired();

              builder.Property(z => z.Latitude).IsRequired();
              builder.Property(z => z.Longitude).IsRequired();

              builder.Property(z => z.NumberOfPeople)
                     .IsRequired();

              builder.Property(z => z.UrgencyLevel)
                     .IsRequired()
                     .HasDefaultValue(1); // Default to low urgency

              // Indexing UrgencyLevel makes querying for high-priority zones faster
              builder.HasIndex(z => z.UrgencyLevel);
       }
}

public class EvacuationStatusConfiguration : IEntityTypeConfiguration<EvacuationStatus>
{
       public void Configure(EntityTypeBuilder<EvacuationStatus> builder)
       {
              builder.ToTable("EvacuationStatuses");

              // Assuming ZoneId is a 1-to-1 relationship with the Zone
              builder.HasKey(s => s.ZoneId);

              builder.Property(s => s.ZoneId)
                     .HasMaxLength(50)
                     .IsRequired();

              builder.Property(s => s.TotalEvacuated).IsRequired();
              builder.Property(s => s.RemainingPeople).IsRequired();

              builder.Property(s => s.LastVehicleUsed)
                     .HasMaxLength(50)
                     .IsRequired(false); // Can be null if no vehicle has been used yet
       }
}

public class EvacuationPlanConfiguration : IEntityTypeConfiguration<EvacuationPlan>
{
       public void Configure(EntityTypeBuilder<EvacuationPlan> builder)
       {
              builder.ToTable("EvacuationPlans");

              // Since EvacuationPlan doesn't have a dedicated ID property like "PlanId", 
              // we create a composite primary key using both ZoneId and VehicleId.
              builder.HasKey(p => new { p.ZoneId, p.VehicleId });

              builder.Property(p => p.ZoneId)
                     .HasMaxLength(50)
                     .IsRequired();

              builder.Property(p => p.VehicleId)
                     .HasMaxLength(50)
                     .IsRequired();

              builder.Property(p => p.ETA).IsRequired();
              builder.Property(p => p.NumberOfPeople).IsRequired();
       }
}