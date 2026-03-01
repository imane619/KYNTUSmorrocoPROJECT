namespace ShiftMaster.Absence.API.Domain.Entities;

/// <summary>
/// Absence/Leave entity.
/// Type: PaidLeave, Sick, Maternity, Unpaid
/// Status: Pending, Approved, Rejected
/// </summary>
public class Absence
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string Type { get; set; } = "PaidLeave";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "Pending";
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
