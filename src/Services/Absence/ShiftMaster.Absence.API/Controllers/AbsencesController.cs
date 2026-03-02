using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AbsenceEntity = ShiftMaster.Absence.API.Domain.Entities.Absence;
using ShiftMaster.Absence.API.Application.Services;
using ShiftMaster.Absence.API.Infrastructure.Data;
using ShiftMaster.Shared.DTOs.Absence;

namespace ShiftMaster.Absence.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AbsencesController : ControllerBase
{
    private readonly AbsenceDbContext _db;
    private readonly IPlanningApiClient _planningClient;

    public AbsencesController(AbsenceDbContext db, IPlanningApiClient planningClient)
    {
        _db = db;
        _planningClient = planningClient;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AbsenceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AbsenceDto>>> GetAll(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] Guid? employeeId,
        [FromQuery] string? status,
        CancellationToken ct = default)
    {
        var query = _db.Absences.AsQueryable();
        if (startDate.HasValue) query = query.Where(a => a.EndDate >= startDate.Value);
        if (endDate.HasValue) query = query.Where(a => a.StartDate <= endDate.Value);
        if (employeeId.HasValue) query = query.Where(a => a.EmployeeId == employeeId.Value);
        if (!string.IsNullOrEmpty(status)) query = query.Where(a => a.Status == status);

        var list = await query.OrderBy(a => a.StartDate).ToListAsync(ct);
        return Ok(list.Select(Map));
    }

    [HttpPost]
    [ProducesResponseType(typeof(AbsenceDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<AbsenceDto>> Create([FromBody] CreateAbsenceRequest request, CancellationToken ct = default)
    {
        var absence = new AbsenceEntity
        {
            EmployeeId = request.EmployeeId,
            EmployeeName = request.EmployeeName,
            Type = request.Type,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = "Pending",
            Reason = request.Reason,
            JustificationPath = request.JustificationPath,
            Comment = request.Comment
        };
        _db.Absences.Add(absence);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = absence.Id }, Map(absence));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AbsenceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AbsenceDto>> GetById(Guid id, CancellationToken ct = default)
    {
        var a = await _db.Absences.FindAsync([id], ct);
        if (a == null) return NotFound();
        return Ok(Map(a));
    }

    [HttpPut("{id:guid}/approve")]
    [ProducesResponseType(typeof(AbsenceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AbsenceDto>> Approve(Guid id, CancellationToken ct = default)
    {
        var a = await _db.Absences.FindAsync([id], ct);
        if (a == null) return NotFound();
        a.Status = "Approved";
        a.ResolvedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        try
        {
            await _planningClient.OnLeaveApprovedAsync(a.EmployeeId, a.StartDate, a.EndDate, ct);
        }
        catch
        {
        }

        return Ok(Map(a));
    }

    [HttpPut("{id:guid}/reject")]
    [ProducesResponseType(typeof(AbsenceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AbsenceDto>> Reject(Guid id, CancellationToken ct = default)
    {
        var a = await _db.Absences.FindAsync([id], ct);
        if (a == null) return NotFound();
        a.Status = "Rejected";
        await _db.SaveChangesAsync(ct);
        return Ok(Map(a));
    }

    private static AbsenceDto Map(AbsenceEntity a) => new()
    {
        Id = a.Id,
        EmployeeId = a.EmployeeId,
        EmployeeName = a.EmployeeName,
        Type = a.Type,
        StartDate = a.StartDate,
        EndDate = a.EndDate,
        Status = a.Status,
        Reason = a.Reason,
        JustificationPath = a.JustificationPath,
        Comment = a.Comment,
        CreatedAt = a.CreatedAt
    };
}

public record CreateAbsenceRequest
{
    public Guid EmployeeId { get; init; }
    public string EmployeeName { get; init; } = string.Empty;
    public string Type { get; init; } = "PaidLeave";
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public string? Reason { get; init; }
    public string? JustificationPath { get; init; }
    public string? Comment { get; init; }
}
