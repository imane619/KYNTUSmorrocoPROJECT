namespace ShiftMaster.Shared.DTOs.Dashboard;

/// <summary>
/// Heatmap data for presence visualization.
/// Rows: Cellules, Columns: Hours (8h-18h).
/// </summary>
public record HeatmapDto
{
    public string[] RowLabels { get; init; } = []; // Cellule A, B, C...
    public string[] ColumnLabels { get; init; } = []; // 8h, 10h, 12h...
    public int[][] Values { get; init; } = []; // 0=red, 1=orange, 2=green
}
