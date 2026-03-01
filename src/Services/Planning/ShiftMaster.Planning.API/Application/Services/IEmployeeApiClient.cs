namespace ShiftMaster.Planning.API.Application.Services;

/// <summary>
/// Client to fetch employees from Employee service.
/// </summary>
public interface IEmployeeApiClient
{
    Task<IReadOnlyList<EmployeeInfo>> GetEmployeesAsync(Guid? celluleId, CancellationToken ct = default);
}
