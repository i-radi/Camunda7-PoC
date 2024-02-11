using Microsoft.EntityFrameworkCore;
using muatamer_camunda_poc.Models;

namespace muatamer_camunda_poc.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<UmrahGroup> UmrahGroups { get; set; }
    public DbSet<MuatamerInformation> MuatamerInformations { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Nationality> Nationalities { get; set; }
    public DbSet<ExternalAgent> ExternalAgents { get; set; }
    public DbSet<UmrahOperator> UmrahOperators { get; set; }
    public DbSet<ExternalAgentUmrahOperator> ExternalAgentUmrahOperators { get; set; }
    public DbSet<StandaloneQuotaTracking> StandaloneQuotaTracking { get; set; }
    public DbSet<IntersectionQuotaTracking> IntersectionQuotaTracking { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<StandaloneQuotaTracking>(entity =>
        {
            entity.HasCheckConstraint("CK_StandaloneQuotaTracking_Reserved", "Reserved <= Total - Used");
            entity.HasCheckConstraint("CK_StandaloneQuotaTracking_Used", "Used <= Total");
        });
        modelBuilder.Entity<IntersectionQuotaTracking>(entity =>
        {
            entity.HasCheckConstraint("CK_IntersectionQuotaTracking_Reserved", "Reserved <= Total - Used");
            entity.HasCheckConstraint("CK_IntersectionQuotaTracking_Used", "Used <= Total");
        });
    }

    public override int SaveChanges()
    {
        ValidateQuotaTrackings();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ValidateQuotaTrackings();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ValidateQuotaTrackings()
    {
        foreach (var entry in ChangeTracker.Entries<StandaloneQuotaTracking>())
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                var entity = entry.Entity;
                if (entity.Total - entity.Used - entity.Reserved < 0)
                {
                    throw new InvalidOperationException("Reserved quota cannot exceed the remaining quota.");
                }
            }
        }

        foreach (var entry in ChangeTracker.Entries<IntersectionQuotaTracking>())
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                var entity = entry.Entity;
                if (entity.Total - entity.Used - entity.Reserved < 0)
                {
                    throw new InvalidOperationException("Reserved quota cannot exceed the remaining quota.");
                }
            }
        }
    }

    public override void Dispose()
    {
        base.Dispose();
    }
}
