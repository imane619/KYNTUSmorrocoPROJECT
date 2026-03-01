namespace ShiftMaster.Employee.API.Domain.Entities;

/// <summary>
/// Organizational pole (top-level unit).
/// </summary>
public class Pole
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}
