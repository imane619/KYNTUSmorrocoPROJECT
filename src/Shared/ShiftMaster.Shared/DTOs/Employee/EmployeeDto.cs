namespace ShiftMaster.Shared.DTOs.Employee;

/// <summary>
/// Employee data transfer object.
/// </summary>
public record EmployeeDto
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string ContractType { get; init; } = string.Empty;
    public string EmploymentType { get; init; } = "CDI";
    public DateTime HireDate { get; init; }
    public int SeniorityMonths { get; init; }
    public string[] Skills { get; init; } = [];
    public string[] Availability { get; init; } = [];
    public bool PreavisFlag { get; init; }
    public int PreavisReduction { get; init; } = 1;
    public bool SaturdayRotationRule { get; init; }
    public Guid? PoleId { get; init; }
    public Guid? CelluleId { get; init; }
    public Guid? DepartementId { get; init; }
    public bool IsActive { get; init; } = true;
    public double EquityScore { get; init; }
}
