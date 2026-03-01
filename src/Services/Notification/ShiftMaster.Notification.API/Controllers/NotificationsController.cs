using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShiftMaster.Notification.API.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    /// <summary>
    /// Get notifications for current user.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<object>> GetMyNotifications()
    {
        return Ok(new[]
        {
            new { id = Guid.NewGuid(), type = "PlanningValidated", message = "Planning validé et publié", read = false, createdAt = DateTime.UtcNow.AddHours(-1) },
            new { id = Guid.NewGuid(), type = "LeaveApproved", message = "Votre demande de congé a été approuvée", read = false, createdAt = DateTime.UtcNow.AddHours(-2) }
        });
    }

    /// <summary>
    /// Mark notification as read.
    /// </summary>
    [HttpPut("{id:guid}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult MarkAsRead(Guid id) => NoContent();
}
