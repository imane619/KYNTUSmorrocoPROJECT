namespace ShiftMaster.Shared.DTOs.Dashboard;

/// <summary>
/// Alert for dashboard display.
/// </summary>
public record AlertDto
{
    public Guid Id { get; init; }
    public string Type { get; init; } = string.Empty; // CriticalPauseOverlap, NewRecruitSaturday, EquityImbalance
    public string Message { get; init; } = string.Empty;
    public string Severity { get; init; } = "Warning"; // Info, Warning, Critical
    public DateTime CreatedAt { get; init; }
}
