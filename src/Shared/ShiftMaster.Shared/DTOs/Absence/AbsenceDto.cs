namespace ShiftMaster.Shared.DTOs.Absence;

/// <summary>
/// Absence/Leave data transfer object.
/// </summary>
public record AbsenceDto
{
    public Guid Id { get; init; }
    public Guid EmployeeId { get; init; }
    public string EmployeeName { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty; // PaidLeave, Sick, Maternity, Unpaid
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public string Status { get; init; } = string.Empty; // Pending, Approved, Rejected
    public string? Reason { get; init; }
    public DateTime CreatedAt { get; init; }
}
