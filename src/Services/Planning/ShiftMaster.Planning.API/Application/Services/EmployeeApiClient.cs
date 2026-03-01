using System.Net.Http.Json;
using ShiftMaster.Shared.DTOs.Employee;

namespace ShiftMaster.Planning.API.Application.Services;

/// <summary>
/// HTTP client to fetch employees from Employee microservice.
/// </summary>
public class EmployeeApiClient : IEmployeeApiClient
{
    private readonly HttpClient _http;

    public EmployeeApiClient(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<EmployeeInfo>> GetEmployeesAsync(Guid? celluleId, CancellationToken ct = default)
    {
        var url = "api/employees?activeOnly=true";
        var list = await _http.GetFromJsonAsync<List<EmployeeDto>>(url, ct) ?? new List<EmployeeDto>();
        return list.Select(d => new EmployeeInfo
        {
            Id = d.Id,
            FirstName = d.FirstName,
            LastName = d.LastName,
            CelluleId = d.CelluleId,
            PreavisFlag = d.PreavisFlag,
            SaturdayRotationRule = d.SaturdayRotationRule,
            Availability = d.Availability
        }).ToList();
    }
}
