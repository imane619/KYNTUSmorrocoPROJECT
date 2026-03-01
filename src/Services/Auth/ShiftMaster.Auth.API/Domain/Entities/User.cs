using Microsoft.AspNetCore.Identity;

namespace ShiftMaster.Auth.API.Domain.Entities;

/// <summary>
/// Application user entity for authentication.
/// </summary>
public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Guid? EmployeeId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
