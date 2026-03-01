namespace ShiftMaster.Shared.DTOs.Planning;

/// <summary>
/// Shift definition (A, B, C, D).
/// </summary>
public record ShiftDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty; // A, B, C, D
    public TimeSpan StartTime { get; init; }
    public TimeSpan EndTime { get; init; }
}
