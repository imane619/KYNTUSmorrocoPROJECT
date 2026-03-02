namespace ShiftMaster.Planning.API.Application.Services;

/// <summary>
/// Calculates dynamic Equity Score (0-100%) for an employee.
/// </summary>
public interface IEquityCalculatorService
{
    /// <summary>
    /// Compute equity score for an employee based on shift distribution, weekend frequency, and total hours vs team average.
    /// </summary>
    Task<EquityResult> CalculateAsync(Guid employeeId, Guid? celluleId, DateTime? periodStart, DateTime? periodEnd, CancellationToken ct = default);

    /// <summary>
    /// Recalculate equity scores for all employees in a cellule/period.
    /// </summary>
    Task RecalculateAllAsync(Guid? celluleId, DateTime periodStart, DateTime periodEnd, CancellationToken ct = default);
}

public record EquityResult
{
    public Guid EmployeeId { get; init; }
    public double Score { get; init; }
    public int ShiftsAssigned { get; init; }
    public int SaturdayShifts { get; init; }
    public double TotalHours { get; init; }
    public double TeamAverageHours { get; init; }
    public string Label { get; init; } = ""; // "Très bien", "Bien", etc.
}
