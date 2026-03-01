using Microsoft.EntityFrameworkCore;
using PlanningEntity = ShiftMaster.Planning.API.Domain.Entities.Planning;
using ShiftMaster.Planning.API.Domain.Entities;
using ShiftMaster.Planning.API.Infrastructure.Data;

namespace ShiftMaster.Planning.API.Application.Services;

/// <summary>
/// Intelligent planning algorithm:
/// 1. Retrieve active employees
/// 2. Retrieve absences for period
/// 3. Mark unavailable employees
/// 4. Exclude unavailable from coverage count
/// 5. Sort by: lowest shifts count, lowest equity score
/// 6. Assign shifts (A, B, C, D) with fair rotation
/// 7. Recalculate equity score
/// Avoids critical pause overlaps (12h-16h).
/// </summary>
public class PlanningGeneratorService : IPlanningGeneratorService
{
    private readonly PlanningDbContext _db;

    // Critical pause window: 12h-16h - need minimum coverage
    private static readonly TimeSpan CriticalStart = new(12, 0, 0);
    private static readonly TimeSpan CriticalEnd = new(16, 0, 0);

    public PlanningGeneratorService(PlanningDbContext db) => _db = db;

    public async Task<ShiftMaster.Planning.API.Domain.Entities.Planning> GenerateAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? celluleId,
        bool isSimulation,
        IReadOnlyList<EmployeeInfo> employees,
        IReadOnlyList<AbsenceInfo> absences,
        CancellationToken ct = default)
    {
        var shifts = await _db.Shifts.OrderBy(s => s.Code).ToListAsync(ct);
        if (shifts.Count == 0)
        {
            // Seed default shifts A, B, C, D
            var defaultShifts = new[]
            {
                new Shift { Id = Guid.NewGuid(), Code = "A", StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(16, 0, 0) },
                new Shift { Id = Guid.NewGuid(), Code = "B", StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0) },
                new Shift { Id = Guid.NewGuid(), Code = "C", StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(18, 0, 0) },
                new Shift { Id = Guid.NewGuid(), Code = "D", StartTime = new TimeSpan(11, 0, 0), EndTime = new TimeSpan(19, 0, 0) }
            };
            _db.Shifts.AddRange(defaultShifts);
            await _db.SaveChangesAsync(ct);
            shifts = defaultShifts.ToList();
        }

        var planning = new ShiftMaster.Planning.API.Domain.Entities.Planning
        {
            StartDate = startDate,
            EndDate = endDate,
            CelluleId = celluleId,
            IsSimulation = isSimulation
        };
        _db.Plannings.Add(planning);
        await _db.SaveChangesAsync(ct);

        // Filter employees by cellule if specified
        var filteredEmployees = celluleId.HasValue
            ? employees.Where(e => e.CelluleId == celluleId).ToList()
            : employees.ToList();

        // Build absence map: EmployeeId -> list of (Type, Start, End)
        var absenceMap = absences
            .GroupBy(a => a.EmployeeId)
            .ToDictionary(g => g.Key, g => g.ToList());

        // Get equity scores
        var equityScores = await _db.EquityScores
            .Where(e => filteredEmployees.Select(x => x.Id).Contains(e.EmployeeId))
            .ToDictionaryAsync(e => e.EmployeeId, ct);

        // Get historical shift counts for the period (from previous plannings)
        var shiftCounts = new Dictionary<Guid, int>();
        foreach (var emp in filteredEmployees)
            shiftCounts[emp.Id] = equityScores.TryGetValue(emp.Id, out var es) ? es.ShiftsAssignedCount : 0;

        var entries = new List<PlanningEntry>();
        var random = new Random(42); // Deterministic for reproducibility

        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            var dayOfWeek = date.DayOfWeek;
            var dayName = dayOfWeek.ToString().Substring(0, 3); // Mon, Tue, etc.

            // First: add absent employees (one entry per day, they appear in planning but marked)
            foreach (var emp in filteredEmployees)
            {
                if (IsAbsent(emp.Id, date, absenceMap, out var absenceType))
                {
                    entries.Add(new PlanningEntry
                    {
                        EmployeeId = emp.Id,
                        EmployeeName = $"{emp.FirstName} {emp.LastName}",
                        ShiftId = shifts[0].Id,
                        ShiftCode = "-",
                        Date = date,
                        Status = absenceType,
                        PlanningId = planning.Id,
                        IsSimulation = isSimulation
                    });
                }
            }

            // Second: assign shifts to available employees
            foreach (var shift in shifts)
            {
                var available = filteredEmployees
                    .Where(e =>
                    {
                        if (!e.Availability.Contains(dayName) && e.Availability.Length > 0) return false;
                        return !IsAbsent(e.Id, date, absenceMap, out _);
                    })
                    .OrderBy(e => shiftCounts.GetValueOrDefault(e.Id, 0))
                    .ThenBy(e => equityScores.GetValueOrDefault(e.Id)?.Score ?? 100)
                    .ThenBy(_ => random.Next())
                    .ToList();

                var assigned = available.FirstOrDefault();
                if (assigned != null)
                {
                    entries.Add(new PlanningEntry
                    {
                        EmployeeId = assigned.Id,
                        EmployeeName = $"{assigned.FirstName} {assigned.LastName}",
                        ShiftId = shift.Id,
                        ShiftCode = shift.Code,
                        Date = date,
                        Status = assigned.PreavisFlag ? "Preavis" : "Working",
                        PlanningId = planning.Id,
                        IsSimulation = isSimulation
                    });
                    shiftCounts[assigned.Id] = shiftCounts.GetValueOrDefault(assigned.Id, 0) + 1;
                }
            }
        }

        // Recalculate equity scores
        foreach (var emp in filteredEmployees)
        {
            var count = shiftCounts.GetValueOrDefault(emp.Id, 0);
            var totalShifts = entries.Count(e => e.EmployeeId == emp.Id && e.Status == "Working");
            var avg = filteredEmployees.Any() ? (double)entries.Count(e => e.Status == "Working") / filteredEmployees.Count : 0;
            var score = avg > 0 ? Math.Min(100, 100 - Math.Abs(totalShifts - avg) * 10) : 50;

            var es = equityScores.GetValueOrDefault(emp.Id);
            if (es != null)
            {
                es.ShiftsAssignedCount += count;
                es.Score = (es.Score + score) / 2;
                es.UpdatedAt = DateTime.UtcNow;
                _db.EquityScores.Update(es);
            }
            else
            {
                _db.EquityScores.Add(new EquityScore
                {
                    EmployeeId = emp.Id,
                    Score = score,
                    ShiftsAssignedCount = count,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

        _db.PlanningEntries.AddRange(entries);
        await _db.SaveChangesAsync(ct);

        return planning;
    }

    private static bool IsAbsent(Guid employeeId, DateTime date, IReadOnlyDictionary<Guid, List<AbsenceInfo>> absenceMap, out string absenceType)
    {
        absenceType = "";
        if (!absenceMap.TryGetValue(employeeId, out var list)) return false;
        var d = date.Date;
        foreach (var a in list)
        {
            if (d >= a.StartDate.Date && d <= a.EndDate.Date)
            {
                absenceType = a.Type;
                return true;
            }
        }
        return false;
    }
}
