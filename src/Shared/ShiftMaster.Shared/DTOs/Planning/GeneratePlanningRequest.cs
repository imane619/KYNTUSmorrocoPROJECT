namespace ShiftMaster.Shared.DTOs.Planning;

/// <summary>
/// Request to generate a new planning.
/// </summary>
public record GeneratePlanningRequest
{
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public Guid? CelluleId { get; init; }
    public bool IsSimulation { get; init; } = true;
}
