using Microsoft.EntityFrameworkCore;
using PlanningEntity = ShiftMaster.Planning.API.Domain.Entities.Planning;
using ShiftMaster.Planning.API.Domain.Entities;
using ShiftMaster.Planning.API.Infrastructure.Data;

namespace ShiftMaster.Planning.API.Application.Services;

/// <summary>
/// Intelligent planning algorithm with fairness and Preavis logic:
/// 1. Exclude employees with Approved Leaves/Sick/Maternity
/// 2. Rotation A, B, C, D with fair distribution
/// 3. Weekend fairness: balance Saturday shifts between employees
/// 4. Preavis: apply -PreavisReduction (default -1h) to shift EndTime when HasPreavis
/// 5. Sort by: lowest total shifts, lowest equity score, lowest Saturday count (for Saturday)
/// </summary>
public class PlanningGeneratorService : IPlanningGeneratorService
{
    private readonly PlanningDbContext _db;

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
            var defaultShifts = CreateDefaultShifts();
            _db.Shifts.AddRange(defaultShifts);
            await _db.SaveChangesAsync(ct);
            shifts = [.. defaultShifts];
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

        var filteredEmployees = celluleId.HasValue
            ? employees.Where(e => e.CelluleId == celluleId).ToList()
            : [.. employees];

        var absenceMap = absences
            .GroupBy(a => a.EmployeeId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var equityScores = await _db.EquityScores
            .Where(e => filteredEmployees.Select(x => x.Id).Contains(e.EmployeeId))
            .ToDictionaryAsync(e => e.EmployeeId, ct);

        var shiftCounts = new Dictionary<Guid, int>();
        var saturdayCounts = new Dictionary<Guid, int>();
        foreach (var emp in filteredEmployees)
        {
            shiftCounts[emp.Id] = equityScores.TryGetValue(emp.Id, out var es) ? es.ShiftsAssignedCount : 0;
            saturdayCounts[emp.Id] = 0;
        }

        var entries = new List<PlanningEntry>();
        var random = new Random(42);

        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            var dayOfWeek = date.DayOfWeek;
            var dayName = dayOfWeek.ToString()[..3];
            var isSaturday = dayOfWeek == DayOfWeek.Saturday;

            // 1. Add absent employees (Approved PaidLeave, Sick, Maternity only)
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

            // 2. Assign shifts to available employees
            foreach (var shift in shifts)
            {
                var available = filteredEmployees
                    .Where(e =>
                    {
                        if (e.Availability.Length > 0 && !e.Availability.Contains(dayName)) return false;
                        return !IsAbsent(e.Id, date, absenceMap, out _);
                    })
                    .OrderBy(e => shiftCounts.GetValueOrDefault(e.Id, 0))
                    .ThenBy(e => equityScores.GetValueOrDefault(e.Id)?.Score ?? 100)
                    .ThenBy(e => isSaturday && e.SaturdayRotationRule ? saturdayCounts.GetValueOrDefault(e.Id, 0) : 0)
                    .ThenBy(_ => random.Next())
                    .ToList();

                var assigned = available.FirstOrDefault();
                if (assigned != null)
                {
                    var hasPreavis = assigned.PreavisFlag;
                    var reduction = hasPreavis ? Math.Min(assigned.PreavisReduction, 3) : 0;
                    var effectiveStart = shift.StartTime;
                    var effectiveEnd = reduction > 0
                        ? shift.EndTime.Subtract(TimeSpan.FromHours(reduction))
                        : shift.EndTime;

                    entries.Add(new PlanningEntry
                    {
                        EmployeeId = assigned.Id,
                        EmployeeName = $"{assigned.FirstName} {assigned.LastName}",
                        ShiftId = shift.Id,
                        ShiftCode = shift.Code,
                        Date = date,
                        StartTime = effectiveStart,
                        EndTime = effectiveEnd,
                        Status = hasPreavis ? "Preavis" : "Working",
                        PlanningId = planning.Id,
                        IsSimulation = isSimulation,
                        HasPreavis = hasPreavis
                    });

                    shiftCounts[assigned.Id] = shiftCounts.GetValueOrDefault(assigned.Id, 0) + 1;
                    if (isSaturday)
                        saturdayCounts[assigned.Id] = saturdayCounts.GetValueOrDefault(assigned.Id, 0) + 1;
                }
            }
        }

        // 3. Recalculate equity scores
        foreach (var emp in filteredEmployees)
        {
            var totalShifts = entries.Count(e => e.EmployeeId == emp.Id && (e.Status == "Working" || e.Status == "Preavis"));
            var avg = filteredEmployees.Count > 0
                ? (double)entries.Count(e => e.Status == "Working" || e.Status == "Preavis") / filteredEmployees.Count
                : 0;
            var score = avg > 0 ? Math.Min(100, Math.Max(0, 100 - Math.Abs(totalShifts - avg) * 8)) : 50;

            var es = equityScores.GetValueOrDefault(emp.Id);
            if (es != null)
            {
                es.ShiftsAssignedCount += shiftCounts.GetValueOrDefault(emp.Id, 0);
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
                    ShiftsAssignedCount = shiftCounts.GetValueOrDefault(emp.Id, 0),
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

        _db.PlanningEntries.AddRange(entries);
        await _db.SaveChangesAsync(ct);

        return planning;
    }

    private static List<Shift> CreateDefaultShifts() =>
    [
        new Shift { Id = Guid.NewGuid(), Code = "A", StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(16, 0, 0) },
        new Shift { Id = Guid.NewGuid(), Code = "B", StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0) },
        new Shift { Id = Guid.NewGuid(), Code = "C", StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(18, 0, 0) },
        new Shift { Id = Guid.NewGuid(), Code = "D", StartTime = new TimeSpan(11, 0, 0), EndTime = new TimeSpan(19, 0, 0) }
    ];

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
