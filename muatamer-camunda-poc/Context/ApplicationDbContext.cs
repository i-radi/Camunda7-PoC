using Microsoft.EntityFrameworkCore;
using muatamer_camunda_poc.Models;

namespace muatamer_camunda_poc.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<MuatamerInformation> MuatamerInformations { get; set; }
    public DbSet<UmrahGroup> UmrahGroups { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
