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
    public DbSet<TotalQuotaTracking> TotalQuotaTracking { get; set; }
    public DbSet<PeriodicalQuotaTracking> PeriodicalQuotaTracking { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
