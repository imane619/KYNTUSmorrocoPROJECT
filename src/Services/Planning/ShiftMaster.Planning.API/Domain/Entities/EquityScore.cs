namespace ShiftMaster.Planning.API.Domain.Entities;

/// <summary>
/// Equity score per employee for fair rotation.
/// </summary>
public class EquityScore
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EmployeeId { get; set; }
    public double Score { get; set; }
    public int ShiftsAssignedCount { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
