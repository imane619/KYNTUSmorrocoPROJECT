namespace ShiftMaster.Absence.API.Application.Services;

/// <summary>
/// Notifies Planning API: remove employee shifts for approved leave dates, then recalculate equity.
/// </summary>
public class PlanningApiClient : IPlanningApiClient
{
    private readonly HttpClient _http;

    public PlanningApiClient(HttpClient http) => _http = http;

    public async Task OnLeaveApprovedAsync(Guid employeeId, DateTime startDate, DateTime endDate, CancellationToken ct = default)
    {
        var start = startDate.ToString("yyyy-MM-dd");
        var end = endDate.ToString("yyyy-MM-dd");
        await _http.DeleteAsync($"api/planning/entries?employeeId={employeeId}&startDate={start}&endDate={end}", ct);

        var monthStart = new DateTime(startDate.Year, startDate.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);
        await _http.PostAsync($"api/equity/recalculate?start={monthStart:yyyy-MM-dd}&end={monthEnd:yyyy-MM-dd}", null, ct);
    }
}
