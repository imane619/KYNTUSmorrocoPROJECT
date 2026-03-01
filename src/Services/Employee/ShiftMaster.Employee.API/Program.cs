using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShiftMaster.Employee.API.Application.Repositories;
using ShiftMaster.Employee.API.Domain.Entities;
using ShiftMaster.Employee.API.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EmployeeDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var jwtKey = builder.Configuration["Jwt:Key"] ?? "ShiftMaster-SuperSecret-Key-Min32Chars!!";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "ShiftMaster",
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "ShiftMaster",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ShiftMaster Employee API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { In = ParameterLocation.Header, Name = "Authorization", Type = SecuritySchemeType.ApiKey });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() } });
});

var app = builder.Build();

// Remplace tout ton bloc "using (var scope...)" par celui-ci :
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();
    
    // 1. On utilise MigrateAsync au lieu de EnsureCreatedAsync pour forcer la création des tables
    await db.Database.MigrateAsync();

    // 2. Seed sample data if empty
    await db.Database.MigrateAsync();
    if (!await db.Employees.AnyAsync())
    {
        var cellA = new Cellule { Id = Guid.NewGuid(), Name = "Cellule A", Code = "A" };
        var cellB = new Cellule { Id = Guid.NewGuid(), Name = "Cellule B", Code = "B" };
        var cellC = new Cellule { Id = Guid.NewGuid(), Name = "Cellule C", Code = "C" };
        
        db.Cellules.AddRange(cellA, cellB, cellC);

        for (var i = 1; i <= 40; i++)
        {
            db.Employees.Add(new Employee
            {
                FirstName = $"Employé{i}",
                LastName = $"Test",
                Email = $"emp{i}@shiftmaster.com",
                ContractType = i % 3 == 0 ? "CDD" : "CDI",
                HireDate = DateTime.SpecifyKind(DateTime.UtcNow.AddMonths(-i * 2), DateTimeKind.Utc),
                Skills = ["Appels", "Support"], 
                Availability = ["Lun", "Mar", "Mer", "Jeu", "Ven"],
                PreavisFlag = i % 5 == 0,
                SaturdayRotationRule = i % 4 == 0,
                CelluleId = i % 3 == 0 ? cellA.Id : i % 3 == 1 ? cellB.Id : cellC.Id,
                IsActive = true
            });
        }
        await db.SaveChangesAsync();
    }
}

// AJOUTE LE CORS ICI (sinon Angular ne pourra pas lire les données)
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "Employee" }));

app.Run();
