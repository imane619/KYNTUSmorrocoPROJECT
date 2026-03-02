using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftMaster.Planning.API.Application.Services;

namespace ShiftMaster.Planning.API.Controllers;

[ApiController]
[Route("api/equity")]
[Authorize]
public class EquityController : ControllerBase
{
    private readonly IEquityCalculatorService _calculator;

    public EquityController(IEquityCalculatorService calculator) => _calculator = calculator;

    /// <summary>
    /// Get equity score for an employee.
    /// </summary>
    [HttpGet("{employeeId:guid}")]
    [ProducesResponseType(typeof(EquityResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<EquityResult>> GetEquity(
        Guid employeeId,
        [FromQuery] Guid? celluleId,
        [FromQuery] DateTime? start,
        [FromQuery] DateTime? end,
        CancellationToken ct = default)
    {
        var result = await _calculator.CalculateAsync(employeeId, celluleId, start, end, ct);
        return Ok(result);
    }

    /// <summary>
    /// Recalculate equity scores for all employees in a period.
    /// </summary>
    [HttpPost("recalculate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Recalculate(
        [FromQuery] Guid? celluleId,
        [FromQuery] DateTime start,
        [FromQuery] DateTime end,
        CancellationToken ct = default)
    {
        await _calculator.RecalculateAllAsync(celluleId, start, end, ct);
        return NoContent();
    }
}
