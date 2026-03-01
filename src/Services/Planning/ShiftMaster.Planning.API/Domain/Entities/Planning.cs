namespace ShiftMaster.Planning.API.Domain.Entities;

/// <summary>
/// Planning period (batch of entries).
/// </summary>
public class Planning
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? CelluleId { get; set; }
    public bool IsSimulation { get; set; } = true;
    public bool IsPublished { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
