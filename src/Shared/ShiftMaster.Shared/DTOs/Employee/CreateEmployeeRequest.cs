using System.ComponentModel.DataAnnotations;

namespace ShiftMaster.Shared.DTOs.Employee;

/// <summary>
/// Request DTO for creating a new employee.
/// </summary>
public record CreateEmployeeRequest
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; init; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; init; }

    [Required]
    public string ContractType { get; init; } = "CDI";

    public DateTime HireDate { get; init; } = DateTime.UtcNow.Date;

    public string[] Skills { get; init; } = [];
    public string[] Availability { get; init; } = [];
    public bool PreavisFlag { get; init; }
    public bool SaturdayRotationRule { get; init; }
    public Guid? PoleId { get; init; }
    public Guid? CelluleId { get; init; }
    public Guid? DepartementId { get; init; }
}
