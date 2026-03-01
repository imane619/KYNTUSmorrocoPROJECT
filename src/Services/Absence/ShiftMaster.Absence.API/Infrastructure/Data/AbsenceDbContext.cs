using Microsoft.EntityFrameworkCore;
using AbsenceEntity = ShiftMaster.Absence.API.Domain.Entities.Absence;

namespace ShiftMaster.Absence.API.Infrastructure.Data;

public class AbsenceDbContext : DbContext
{
    public AbsenceDbContext(DbContextOptions<AbsenceDbContext> options) : base(options) { }

    public DbSet<AbsenceEntity> Absences => Set<AbsenceEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AbsenceEntity>(e => e.HasKey(x => x.Id));
    }
}
