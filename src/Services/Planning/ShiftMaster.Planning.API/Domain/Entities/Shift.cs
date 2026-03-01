namespace ShiftMaster.Planning.API.Domain.Entities;

/// <summary>
/// Shift definition (A, B, C, D).
/// </summary>
public class Shift
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
