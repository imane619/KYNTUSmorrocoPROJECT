using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShiftMaster.Reporting.API.Controllers;

[ApiController]
[Route("api/reporting")]
[Authorize]
public class ReportsController : ControllerBase
{
    /// <summary>
    /// Generate PDF report.
    /// </summary>
    [HttpGet("pdf")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    public ActionResult GetPdf([FromQuery] string type = "monthly")
    {
        // Placeholder - in production use QuestPDF, iTextSharp, or similar
        var content = System.Text.Encoding.UTF8.GetBytes($"PDF Report - {type}");
        return File(content, "application/pdf", $"report-{type}.pdf");
    }

    /// <summary>
    /// Generate Excel report.
    /// </summary>
    [HttpGet("excel")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    public ActionResult GetExcel([FromQuery] string type = "planning")
    {
        // Placeholder - in production use ClosedXML or EPPlus
        var content = System.Text.Encoding.UTF8.GetBytes($"Excel Report - {type}");
        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"report-{type}.xlsx");
    }

    /// <summary>
    /// HR monthly report.
    /// </summary>
    [HttpGet("hr-monthly")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public ActionResult<object> GetHrMonthly([FromQuery] int year, [FromQuery] int month)
    {
        return Ok(new
        {
            year,
            month,
            totalEmployees = 34,
            absences = 12,
            newHires = 2,
            turnover = 0.05
        });
    }

    /// <summary>
    /// Audit logs.
    /// </summary>
    [HttpGet("audit-logs")]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<object>> GetAuditLogs([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        return Ok(new[]
        {
            new { timestamp = DateTime.UtcNow.AddHours(-1), user = "admin@shiftmaster.com", action = "Planning published", entity = "Planning" },
            new { timestamp = DateTime.UtcNow.AddHours(-2), user = "manager@shiftmaster.com", action = "Leave approved", entity = "Absence" }
        });
    }
}
