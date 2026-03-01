using System.Net.Http.Json;

namespace ShiftMaster.Planning.API.Application.Services;

/// <summary>
/// HTTP client to fetch absences from Absence microservice.
/// </summary>
public class AbsenceApiClient : IAbsenceApiClient
{
    private readonly HttpClient _http;

    public AbsenceApiClient(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<AbsenceInfo>> GetAbsencesAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default)
    {
        var url = $"api/absences?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
        var list = await _http.GetFromJsonAsync<List<AbsenceDto>>(url, ct) ?? new List<AbsenceDto>();
        return list
            .Where(a => a.Status == "Approved")
            .Select(d => new AbsenceInfo
            {
                EmployeeId = d.EmployeeId,
                Type = d.Type,
                StartDate = d.StartDate,
                EndDate = d.EndDate
            }).ToList();
    }

    private record AbsenceDto(Guid EmployeeId, string Type, DateTime StartDate, DateTime EndDate, string Status);
}
