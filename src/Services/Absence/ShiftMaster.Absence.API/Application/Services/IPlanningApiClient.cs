namespace ShiftMaster.Absence.API.Application.Services;

/// <summary>
/// Client to notify Planning API when a leave is approved (remove shifts, recalc equity).
/// </summary>
public interface IPlanningApiClient
{
    Task OnLeaveApprovedAsync(Guid employeeId, DateTime startDate, DateTime endDate, CancellationToken ct = default);
}
