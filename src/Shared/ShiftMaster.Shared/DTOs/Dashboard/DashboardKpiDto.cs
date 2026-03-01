namespace ShiftMaster.Shared.DTOs.Dashboard;

/// <summary>
/// Dashboard KPI widgets for Manager view.
/// </summary>
public record DashboardKpiDto
{
    public double CoverageRate { get; init; }
    public int ActiveEmployees { get; init; }
    public int OnBreak { get; init; }
    public int Absent { get; init; }
    public double EquityScore { get; init; }
}
