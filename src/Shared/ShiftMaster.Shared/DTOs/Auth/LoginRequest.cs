using System.ComponentModel.DataAnnotations;

namespace ShiftMaster.Shared.DTOs.Auth;

/// <summary>
/// Request DTO for user login.
/// </summary>
public record LoginRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6)]
    public string Password { get; init; } = string.Empty;
}
