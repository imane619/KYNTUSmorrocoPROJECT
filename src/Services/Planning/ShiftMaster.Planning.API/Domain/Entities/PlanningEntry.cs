namespace ShiftMaster.Planning.API.Domain.Entities;

/// <summary>
/// Single planning entry for an employee on a date.
/// Status: Working, PaidLeave, Sick, Maternity, Preavis
/// </summary>
public class PlanningEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public Guid ShiftId { get; set; }
    public string ShiftCode { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Status { get; set; } = "Working";
    public Guid PlanningId { get; set; }
    public bool IsSimulation { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
