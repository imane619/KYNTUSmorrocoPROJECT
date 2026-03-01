namespace ShiftMaster.Planning.API.Application.Services;

/// <summary>
/// Client to fetch absences from Absence service.
/// </summary>
public interface IAbsenceApiClient
{
    Task<IReadOnlyList<AbsenceInfo>> GetAbsencesAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default);
}
