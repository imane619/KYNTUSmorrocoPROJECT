namespace ShiftMaster.Employee.API.Domain.Entities;

/// <summary>
/// Employee entity for workforce planning.
/// </summary>
public class Employee
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string ContractType { get; set; } = "CDI";
    /// <summary>Employment type: CDI, CDD, Interim, etc.</summary>
    public string EmploymentType { get; set; } = "CDI";
    public DateTime HireDate { get; set; }
    public string[] Skills { get; set; } = [];
    public string[] Availability { get; set; } = [];
    /// <summary>When true, applies -1h reduction to shifts (Préavis).</summary>
    public bool PreavisFlag { get; set; }
    /// <summary>Hours to subtract when PreavisFlag is true (default 1).</summary>
    public int PreavisReduction { get; set; } = 1;
    public bool SaturdayRotationRule { get; set; }
    public Guid? PoleId { get; set; }
    public Guid? CelluleId { get; set; }
    public Guid? DepartementId { get; set; }
    public bool IsActive { get; set; } = true;
    /// <summary>Computed equity score (0-100%).</summary>
    public double EquityScore { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
