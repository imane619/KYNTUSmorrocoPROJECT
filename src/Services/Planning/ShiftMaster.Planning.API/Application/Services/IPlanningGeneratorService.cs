namespace ShiftMaster.Planning.API.Application.Services;

/// <summary>
/// Intelligent planning generation with fair rotation and equity.
/// </summary>
public interface IPlanningGeneratorService
{
    Task<ShiftMaster.Planning.API.Domain.Entities.Planning> GenerateAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? celluleId,
        bool isSimulation,
        IReadOnlyList<EmployeeInfo> employees,
        IReadOnlyList<AbsenceInfo> absences,
        CancellationToken ct = default);
}

public record EmployeeInfo
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public Guid? CelluleId { get; init; }
    public bool PreavisFlag { get; init; }
    public bool SaturdayRotationRule { get; init; }
    public string[] Availability { get; init; } = [];
}

public record AbsenceInfo
{
    public Guid EmployeeId { get; init; }
    public string Type { get; init; } = string.Empty; // PaidLeave, Sick, Maternity, Preavis
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
}
