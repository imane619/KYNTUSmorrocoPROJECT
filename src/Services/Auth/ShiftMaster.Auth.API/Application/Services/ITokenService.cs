using ShiftMaster.Auth.API.Domain.Entities;
using ShiftMaster.Shared.DTOs.Auth;

namespace ShiftMaster.Auth.API.Application.Services;

/// <summary>
/// JWT token generation and validation service.
/// </summary>
public interface ITokenService
{
    Task<LoginResponse> GenerateTokenAsync(User user, CancellationToken ct = default);
    string? ValidateToken(string token);
}
