using Microsoft.EntityFrameworkCore;
using PlanningEntity = ShiftMaster.Planning.API.Domain.Entities.Planning;
using ShiftMaster.Planning.API.Domain.Entities;

namespace ShiftMaster.Planning.API.Infrastructure.Data;

public class PlanningDbContext : DbContext
{
    public PlanningDbContext(DbContextOptions<PlanningDbContext> options) : base(options) { }

    public DbSet<Shift> Shifts => Set<Shift>();
    public DbSet<PlanningEntity> Plannings => Set<PlanningEntity>();
    public DbSet<PlanningEntry> PlanningEntries => Set<PlanningEntry>();
    public DbSet<EquityScore> EquityScores => Set<EquityScore>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlanningEntry>(e =>
        {
            e.HasIndex(x => new { x.PlanningId, x.EmployeeId, x.Date }).IsUnique();
        });
    }
}
