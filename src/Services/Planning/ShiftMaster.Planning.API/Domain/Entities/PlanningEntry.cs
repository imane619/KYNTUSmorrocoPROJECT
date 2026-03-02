namespace ShiftMaster.Planning.API.Domain.Entities;

/// <summary>
/// Single planning entry for an employee on a date.
/// Status: Working, PaidLeave, Sick, Maternity, Preavis
/// When PreavisFlag applies: EndTime = Shift.EndTime - PreavisReduction (default -1h).
/// </summary>
public class PlanningEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public Guid ShiftId { get; set; }
    public string ShiftCode { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    /// <summary>Effective start time (null = use Shift.StartTime).</summary>
    public TimeSpan? StartTime { get; set; }
    /// <summary>Effective end time (null = use Shift.EndTime). When Preavis: EndTime - 1h.</summary>
    public TimeSpan? EndTime { get; set; }
    public string Status { get; set; } = "Working";
    public Guid PlanningId { get; set; }
    public bool IsSimulation { get; set; } = true;
    public bool HasPreavis { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
