using EmployeeEntity = ShiftMaster.Employee.API.Domain.Entities.Employee;

namespace ShiftMaster.Employee.API.Application.Repositories;

public interface IEmployeeRepository
{
    Task<EmployeeEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<EmployeeEntity>> GetAllAsync(bool activeOnly = true, CancellationToken ct = default);
    Task<IReadOnlyList<EmployeeEntity>> GetByCelluleAsync(Guid celluleId, CancellationToken ct = default);
    Task<EmployeeEntity> AddAsync(EmployeeEntity employee, CancellationToken ct = default);
    Task<EmployeeEntity> UpdateAsync(EmployeeEntity employee, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
