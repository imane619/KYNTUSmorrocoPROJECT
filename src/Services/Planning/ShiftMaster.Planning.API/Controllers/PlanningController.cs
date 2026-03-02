using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShiftMaster.Planning.API.Application.Services;
using ShiftMaster.Planning.API.Infrastructure.Data;
using ShiftMaster.Shared.DTOs.Planning;

namespace ShiftMaster.Planning.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PlanningController : ControllerBase
{
    private readonly IPlanningGeneratorService _generator;
    private readonly IEmployeeApiClient _employeeClient;
    private readonly IAbsenceApiClient _absenceClient;
    private readonly PlanningDbContext _db;

    public PlanningController(
        IPlanningGeneratorService generator,
        IEmployeeApiClient employeeClient,
        IAbsenceApiClient absenceClient,
        PlanningDbContext db)
    {
        _generator = generator;
        _employeeClient = employeeClient;
        _absenceClient = absenceClient;
        _db = db;
    }

    /// <summary>
    /// Generate planning for period. Supports simulation mode.
    /// </summary>
    [HttpPost("generate")]
    [ProducesResponseType(typeof(PlanningResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PlanningResponse>> Generate([FromBody] GeneratePlanningRequest request, CancellationToken ct = default)
    {
        var employees = await _employeeClient.GetEmployeesAsync(request.CelluleId, ct);
        var absences = await _absenceClient.GetAbsencesAsync(request.StartDate, request.EndDate, ct);

        var planning = await _generator.GenerateAsync(
            request.StartDate,
            request.EndDate,
            request.CelluleId,
            request.IsSimulation,
            employees,
            absences,
            ct);

        var entries = await _db.PlanningEntries
            .Where(e => e.PlanningId == planning.Id)
            .OrderBy(e => e.Date)
            .ThenBy(e => e.ShiftCode)
            .ToListAsync(ct);

        return Ok(new PlanningResponse
        {
            PlanningId = planning.Id,
            StartDate = planning.StartDate,
            EndDate = planning.EndDate,
            IsSimulation = planning.IsSimulation,
            Entries = entries.Select(e => new PlanningEntryDto
            {
                Id = e.Id,
                EmployeeId = e.EmployeeId,
                EmployeeName = e.EmployeeName,
                ShiftId = e.ShiftId,
                ShiftCode = e.ShiftCode,
                Date = e.Date,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                Status = e.Status
            }).ToList()
        });
    }

    /// <summary>
    /// Publish planning (move from simulation to production).
    /// </summary>
    [HttpPost("{id:guid}/publish")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Publish(Guid id, CancellationToken ct = default)
    {
        var planning = await _db.Plannings.FindAsync([id], ct);
        if (planning == null) return NotFound();
        planning.IsSimulation = false;
        planning.IsPublished = true;
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    /// <summary>
    /// Get planning entries for a period.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PlanningEntryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PlanningEntryDto>>> GetEntries(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] Guid? planningId,
        CancellationToken ct = default)
    {
        var query = _db.PlanningEntries.AsQueryable();
        if (planningId.HasValue) query = query.Where(e => e.PlanningId == planningId.Value);
        if (startDate.HasValue) query = query.Where(e => e.Date >= startDate.Value);
        if (endDate.HasValue) query = query.Where(e => e.Date <= endDate.Value);

        var entries = await query.OrderBy(e => e.Date).ThenBy(e => e.EmployeeName).ToListAsync(ct);
        return Ok(entries.Select(e => new PlanningEntryDto
        {
            Id = e.Id,
            EmployeeId = e.EmployeeId,
            EmployeeName = e.EmployeeName,
            ShiftId = e.ShiftId,
            ShiftCode = e.ShiftCode,
            Date = e.Date,
            StartTime = e.StartTime,
            EndTime = e.EndTime,
            Status = e.Status
        }));
    }

    /// <summary>
    /// Remove planning entries for an employee in a date range (e.g. when leave approved).
    /// </summary>
    [HttpDelete("entries")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveEntriesForEmployee(
        [FromQuery] Guid employeeId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken ct = default)
    {
        var toRemove = await _db.PlanningEntries
            .Where(e => e.EmployeeId == employeeId && e.Date >= startDate && e.Date <= endDate && (e.Status == "Working" || e.Status == "Preavis"))
            .ToListAsync(ct);
        _db.PlanningEntries.RemoveRange(toRemove);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}

public record PlanningResponse
{
    public Guid PlanningId { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public bool IsSimulation { get; init; }
    public List<PlanningEntryDto> Entries { get; init; } = [];
}
