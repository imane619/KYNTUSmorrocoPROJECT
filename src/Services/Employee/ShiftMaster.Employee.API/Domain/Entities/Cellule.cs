namespace ShiftMaster.Employee.API.Domain.Entities;

/// <summary>
/// Cellule (sub-unit under Pole).
/// </summary>
public class Cellule
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Guid? PoleId { get; set; }
}
