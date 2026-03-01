using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShiftMaster.Auth.API.Domain.Entities;

namespace ShiftMaster.Auth.API.Infrastructure.Data;

/// <summary>
/// Auth service database context with Identity.
/// </summary>
public class AuthDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
        });
    }
}
