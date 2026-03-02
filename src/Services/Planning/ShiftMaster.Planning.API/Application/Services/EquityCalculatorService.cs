using Microsoft.EntityFrameworkCore;
using ShiftMaster.Planning.API.Domain.Entities;
using ShiftMaster.Planning.API.Infrastructure.Data;

namespace ShiftMaster.Planning.API.Application.Services;

/// <summary>
/// Dynamic Equity Score (0-100%) based on:
/// - Shift distribution fairness
/// - Weekend (Saturday) rotation balance
/// - Total hours vs team average
/// </summary>
public class EquityCalculatorService : IEquityCalculatorService
{
    private readonly PlanningDbContext _db;

    public EquityCalculatorService(PlanningDbContext db) => _db = db;

    public async Task<EquityResult> CalculateAsync(Guid employeeId, Guid? celluleId, DateTime? periodStart, DateTime? periodEnd, CancellationToken ct = default)
    {
        var (start, end) = ResolvePeriod(periodStart, periodEnd);

        var entries = await _db.PlanningEntries
            .Where(e => e.Date >= start && e.Date <= end && (e.Status == "Working" || e.Status == "Preavis"))
            .ToListAsync(ct);

        var employeeEntries = entries.Where(e => e.EmployeeId == employeeId).ToList();
        if (employeeEntries.Count == 0)
            return new EquityResult { EmployeeId = employeeId, Score = 50, Label = "Non évalué" };

        var shiftMap = await _db.Shifts.ToDictionaryAsync(s => s.Id, ct);
        var myHours = ComputeHours(employeeEntries, shiftMap);
        var mySaturdays = employeeEntries.Count(e => e.Date.DayOfWeek == DayOfWeek.Saturday);

        var allEmployeeIds = entries.Select(e => e.EmployeeId).Distinct().ToList();
        var teamHours = allEmployeeIds
            .Select(id => entries.Where(e => e.EmployeeId == id))
            .Select(empEntries => ComputeHours(empEntries.ToList(), shiftMap))
            .ToList();
        var teamAvgHours = teamHours.Count > 0 ? teamHours.Average() : myHours;

        var avgShifts = (double)entries.Count / Math.Max(1, allEmployeeIds.Count);
        var shiftFairness = avgShifts > 0
            ? Math.Max(0, 100 - Math.Abs(employeeEntries.Count - avgShifts) * 6)
            : 50;

        var saturdayAvg = allEmployeeIds.Count > 0
            ? (double)entries.Count(e => e.Date.DayOfWeek == DayOfWeek.Saturday) / allEmployeeIds.Count
            : 0;
        var weekendFairness = saturdayAvg > 0
            ? Math.Max(0, 100 - Math.Abs(mySaturdays - saturdayAvg) * 12)
            : 50;

        var hoursFairness = teamAvgHours > 0
            ? Math.Max(0, 100 - Math.Abs(myHours - teamAvgHours) / teamAvgHours * 30)
            : 50;

        var score = (shiftFairness * 0.4 + weekendFairness * 0.35 + hoursFairness * 0.25);
        score = Math.Min(100, Math.Max(0, score));

        var label = score >= 85 ? "Très bien" : score >= 70 ? "Bien" : score >= 50 ? "Moyen" : "À améliorer";

        return new EquityResult
        {
            EmployeeId = employeeId,
            Score = Math.Round(score, 1),
            ShiftsAssigned = employeeEntries.Count,
            SaturdayShifts = mySaturdays,
            TotalHours = Math.Round(myHours, 1),
            TeamAverageHours = Math.Round(teamAvgHours, 1),
            Label = label
        };
    }

    public async Task RecalculateAllAsync(Guid? celluleId, DateTime periodStart, DateTime periodEnd, CancellationToken ct = default)
    {
        var employeeIds = await _db.PlanningEntries
            .Where(e => e.Date >= periodStart && e.Date <= periodEnd && (e.Status == "Working" || e.Status == "Preavis"))
            .Select(e => e.EmployeeId)
            .Distinct()
            .ToListAsync(ct);

        foreach (var empId in employeeIds)
        {
            var result = await CalculateAsync(empId, celluleId, periodStart, periodEnd, ct);
            var existing = await _db.EquityScores.FirstOrDefaultAsync(e => e.EmployeeId == empId, ct);
            if (existing != null)
            {
                existing.Score = result.Score;
                existing.ShiftsAssignedCount = result.ShiftsAssigned;
                existing.UpdatedAt = DateTime.UtcNow;
                _db.EquityScores.Update(existing);
            }
            else
            {
                _db.EquityScores.Add(new EquityScore
                {
                    EmployeeId = empId,
                    Score = result.Score,
                    ShiftsAssignedCount = result.ShiftsAssigned,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

        await _db.SaveChangesAsync(ct);
    }

    private static (DateTime Start, DateTime End) ResolvePeriod(DateTime? start, DateTime? end)
    {
        var now = DateTime.UtcNow.Date;
        var s = start ?? new DateTime(now.Year, now.Month, 1);
        var e = end ?? s.AddMonths(1).AddDays(-1);
        return (s, e);
    }

    private static double ComputeHours(List<PlanningEntry> entries, IReadOnlyDictionary<Guid, Shift> shiftMap)
    {
        var total = 0.0;
        foreach (var entry in entries)
        {
            var start = entry.StartTime ?? shiftMap.GetValueOrDefault(entry.ShiftId)?.StartTime ?? TimeSpan.Zero;
            var end = entry.EndTime ?? shiftMap.GetValueOrDefault(entry.ShiftId)?.EndTime ?? TimeSpan.Zero;
            total += (end - start).TotalHours;
        }
        return total;
    }
}
