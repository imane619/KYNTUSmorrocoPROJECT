using Microsoft.EntityFrameworkCore;
using EmployeeEntity = ShiftMaster.Employee.API.Domain.Entities.Employee;
using PoleEntity = ShiftMaster.Employee.API.Domain.Entities.Pole;
using CelluleEntity = ShiftMaster.Employee.API.Domain.Entities.Cellule;

namespace ShiftMaster.Employee.API.Infrastructure.Data;

public class EmployeeDbContext : DbContext
{
    public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : base(options) { }

    public DbSet<EmployeeEntity> Employees => Set<EmployeeEntity>();
    public DbSet<PoleEntity> Poles => Set<PoleEntity>();
    public DbSet<CelluleEntity> Cellules => Set<CelluleEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmployeeEntity>(e =>
        {
            e.HasKey(x => x.Id);
            

            e.Property(x => x.Skills).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));

            e.Property(x => x.Availability).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));

            e.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<PoleEntity>();
        modelBuilder.Entity<CelluleEntity>();
    }
}