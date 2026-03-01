using Microsoft.EntityFrameworkCore;
using EmployeeEntity = ShiftMaster.Employee.API.Domain.Entities.Employee;
using ShiftMaster.Employee.API.Infrastructure.Data;

namespace ShiftMaster.Employee.API.Application.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly EmployeeDbContext _db;

    public EmployeeRepository(EmployeeDbContext db) => _db = db;

    public async Task<EmployeeEntity?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.Employees.FindAsync([id], ct);

    public async Task<IReadOnlyList<EmployeeEntity>> GetAllAsync(bool activeOnly = true, CancellationToken ct = default)
    {
        var query = _db.Employees.AsQueryable();
        if (activeOnly) query = query.Where(e => e.IsActive);
        return await query.OrderBy(e => e.LastName).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<EmployeeEntity>> GetByCelluleAsync(Guid celluleId, CancellationToken ct = default) =>
        await _db.Employees.Where(e => e.CelluleId == celluleId && e.IsActive).ToListAsync(ct);

    public async Task<EmployeeEntity> AddAsync(EmployeeEntity employee, CancellationToken ct = default)
    {
        _db.Employees.Add(employee);
        await _db.SaveChangesAsync(ct);
        return employee;
    }

    public async Task<EmployeeEntity> UpdateAsync(EmployeeEntity employee, CancellationToken ct = default)
    {
        employee.UpdatedAt = DateTime.UtcNow;
        _db.Employees.Update(employee);
        await _db.SaveChangesAsync(ct);
        return employee;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var emp = await _db.Employees.FindAsync([id], ct);
        if (emp != null)
        {
            emp.IsActive = false;
            emp.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }
    }
}
