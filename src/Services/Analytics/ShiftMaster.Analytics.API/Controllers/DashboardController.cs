using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftMaster.Shared.DTOs.Dashboard;

namespace ShiftMaster.Analytics.API.Controllers;

[ApiController]
[Route("api/analytics")]
[Authorize]
public class DashboardController : ControllerBase
{
    /// <summary>
    /// Dashboard KPIs for Manager view.
    /// </summary>
    [HttpGet("dashboard/kpis")]
    [ProducesResponseType(typeof(DashboardKpiDto), StatusCodes.Status200OK)]
    public ActionResult<DashboardKpiDto> GetKpis()
    {
        // In production, aggregate from Employee, Planning, Absence services
        return Ok(new DashboardKpiDto
        {
            CoverageRate = 92,
            ActiveEmployees = 34,
            OnBreak = 4,
            Absent = 3,
            EquityScore = 87
        });
    }

    /// <summary>
    /// Heatmap: rows=Cellules, columns=Hours. Values: 0=red, 1=orange, 2=green.
    /// </summary>
    [HttpGet("dashboard/heatmap")]
    [ProducesResponseType(typeof(HeatmapDto), StatusCodes.Status200OK)]
    public ActionResult<HeatmapDto> GetHeatmap()
    {
        return Ok(new HeatmapDto
        {
            RowLabels = ["Cellule A", "Cellule B", "Cellule C"],
            ColumnLabels = ["8h", "10h", "12h", "14h", "16h", "18h"],
            Values =
            [
                [2, 2, 1, 1, 2, 2],
                [2, 2, 1, 1, 2, 2],
                [2, 1, 0, 0, 1, 2]
            ]
        });
    }

    /// <summary>
    /// Alerts: critical pause overlap, new recruit Saturday, equity imbalance.
    /// </summary>
    [HttpGet("dashboard/alerts")]
    [ProducesResponseType(typeof(IEnumerable<AlertDto>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<AlertDto>> GetAlerts()
    {
        return Ok(new[]
        {
            new AlertDto
            {
                Id = Guid.NewGuid(),
                Type = "CriticalPauseOverlap",
                Message = "Chevauchement de pauses critique 12h - 13h",
                Severity = "Critical",
                CreatedAt = DateTime.UtcNow.AddHours(-1)
            },
            new AlertDto
            {
                Id = Guid.NewGuid(),
                Type = "NewRecruitSaturday",
                Message = "Nouveau recruté non assigné pour samedi",
                Severity = "Warning",
                CreatedAt = DateTime.UtcNow.AddHours(-2)
            },
            new AlertDto
            {
                Id = Guid.NewGuid(),
                Type = "EquityImbalance",
                Message = "Déséquilibre ancienneté dans la Cellule B",
                Severity = "Warning",
                CreatedAt = DateTime.UtcNow.AddHours(-3)
            }
        });
    }

    /// <summary>
    /// Team performance, equity distribution, coverage evolution.
    /// </summary>
    [HttpGet("analytics/team-performance")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public ActionResult<object> GetTeamPerformance()
    {
        return Ok(new
        {
            rotationRate = 65,
            criticalPauses = 5,
            avgPauseTimeMinutes = 14,
            chartData = new[]
            {
                new { label = "Lun", value = 85 },
                new { label = "Mar", value = 92 },
                new { label = "Mer", value = 78 },
                new { label = "Jeu", value = 95 },
                new { label = "Ven", value = 88 }
            }
        });
    }
}
