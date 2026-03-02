using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmployeeEntity = ShiftMaster.Employee.API.Domain.Entities.Employee;
using ShiftMaster.Employee.API.Application.Repositories;
using ShiftMaster.Shared.DTOs.Employee;

namespace ShiftMaster.Employee.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository _repo;

    public EmployeesController(IEmployeeRepository repo) => _repo = repo;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EmployeeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll([FromQuery] bool activeOnly = true, CancellationToken ct = default)
    {
        var list = await _repo.GetAllAsync(activeOnly, ct);
        return Ok(list.Select(Map));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmployeeDto>> GetById(Guid id, CancellationToken ct = default)
    {
        var emp = await _repo.GetByIdAsync(id, ct);
        if (emp == null) return NotFound();
        return Ok(Map(emp));
    }

    [HttpPost]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EmployeeDto>> Create([FromBody] CreateEmployeeRequest request, CancellationToken ct = default)
    {
        var emp = new EmployeeEntity
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            ContractType = request.ContractType,
            HireDate = request.HireDate,
            Skills = request.Skills,
            Availability = request.Availability,
            PreavisFlag = request.PreavisFlag,
            PreavisReduction = request.PreavisReduction,
            EmploymentType = request.EmploymentType,
            SaturdayRotationRule = request.SaturdayRotationRule,
            PoleId = request.PoleId,
            CelluleId = request.CelluleId,
            DepartementId = request.DepartementId
        };
        emp = await _repo.AddAsync(emp, ct);
        return CreatedAtAction(nameof(GetById), new { id = emp.Id }, Map(emp));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmployeeDto>> Update(Guid id, [FromBody] CreateEmployeeRequest request, CancellationToken ct = default)
    {
        var emp = await _repo.GetByIdAsync(id, ct);
        if (emp == null) return NotFound();
        emp.FirstName = request.FirstName;
        emp.LastName = request.LastName;
        emp.Email = request.Email;
        emp.Phone = request.Phone;
        emp.ContractType = request.ContractType;
        emp.HireDate = request.HireDate;
        emp.Skills = request.Skills;
        emp.Availability = request.Availability;
        emp.PreavisFlag = request.PreavisFlag;
        emp.PreavisReduction = request.PreavisReduction;
        emp.EmploymentType = request.EmploymentType;
        emp.SaturdayRotationRule = request.SaturdayRotationRule;
        emp.PoleId = request.PoleId;
        emp.CelluleId = request.CelluleId;
        emp.DepartementId = request.DepartementId;
        emp = await _repo.UpdateAsync(emp, ct);
        return Ok(Map(emp));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        await _repo.DeleteAsync(id, ct);
        return NoContent();
    }

    private static EmployeeDto Map(EmployeeEntity e) => new()
    {
        Id = e.Id,
        FirstName = e.FirstName,
        LastName = e.LastName,
        Email = e.Email,
        Phone = e.Phone,
        ContractType = e.ContractType,
        EmploymentType = e.EmploymentType,
        HireDate = e.HireDate,
        SeniorityMonths = (int)(DateTime.UtcNow - e.HireDate).TotalDays / 30,
        Skills = e.Skills,
        Availability = e.Availability,
        PreavisFlag = e.PreavisFlag,
        PreavisReduction = e.PreavisReduction,
        SaturdayRotationRule = e.SaturdayRotationRule,
        EquityScore = e.EquityScore,
        PoleId = e.PoleId,
        CelluleId = e.CelluleId,
        DepartementId = e.DepartementId,
        IsActive = e.IsActive
    };
}
