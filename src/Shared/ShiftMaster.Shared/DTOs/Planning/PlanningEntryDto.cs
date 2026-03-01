namespace ShiftMaster.Shared.DTOs.Planning;

/// <summary>
/// Single planning entry for an employee on a specific date.
/// </summary>
public record PlanningEntryDto
{
    public Guid Id { get; init; }
    public Guid EmployeeId { get; init; }
    public string EmployeeName { get; init; } = string.Empty;
    public Guid ShiftId { get; init; }
    public string ShiftCode { get; init; } = string.Empty;
    public DateTime Date { get; init; }
    public string Status { get; init; } = "Working"; // Working, PaidLeave, Sick, Maternity, Preavis
    public string StatusColor => Status switch
    {
        "PaidLeave" => "blue",
        "Sick" => "red",
        "Maternity" => "purple",
        "Preavis" => "orange",
        _ => "green"
    };
}
