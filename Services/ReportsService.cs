using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Data;
using TaskManagementSystem.Dtos.Reports;
using TaskManagementSystem.Interfaces.Services;
using TaskStatusEnum = TaskManagementSystem.Enums.TaskStatus;

namespace TaskManagementSystem.Services
{
    public class ReportsService : IReportsService
    {
        private readonly AppDbContext _context;

        public ReportsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ReportsDto> GetReportsDataAsync()
        {
            var now = DateTime.UtcNow;
            var weekAgo = now.AddDays(-7);
            var sixMonthsAgo = now.AddMonths(-6);

            // ── Stat cards ────────────────────────────────────────────
            var totalCompleted = await _context.Tasks
                .CountAsync(t => t.Status == TaskStatusEnum.Completed);

            var completedThisWeek = await _context.Tasks
                .CountAsync(t => t.Status == TaskStatusEnum.Completed
                              && t.CreatedAt >= weekAgo);

            // Approximation with current model: completion age uses CreatedAt only.
            var completedTaskAges = await _context.Tasks
                .Where(t => t.Status == TaskStatusEnum.Completed)
                .Select(t => EF.Functions.DateDiffDay(t.CreatedAt, now))
                .ToListAsync();

            var avgDays = completedTaskAges.Any()
                ? Math.Round(completedTaskAges.Average(), 1)
                : 0;

            // Most active user = most tasks assigned
            var mostActive = await _context.Users
                .Select(u => new
                {
                    u.FullName,
                    Count = _context.Tasks.Count(t => t.AssignedToUserId == u.Id)
                })
                .OrderByDescending(x => x.Count)
                .FirstOrDefaultAsync();

            // Overdue rate
            var totalTasks = await _context.Tasks.CountAsync();
            var overdueCount = await _context.Tasks
                .CountAsync(t => t.DueDate < now
                              && t.Status != TaskStatusEnum.Completed
                              && t.Status != TaskStatusEnum.Cancelled);

            var overdueRate = totalTasks > 0
                ? Math.Round((double)overdueCount / totalTasks * 100, 1)
                : 0;

            // ── Monthly completions (last 6 months) ───────────────────
            var monthly = new List<MonthlyCompletionDto>();
            for (int i = 5; i >= 0; i--)
            {
                var month = now.AddMonths(-i);
                var monthStart = new DateTime(month.Year, month.Month, 1);
                var monthEnd = monthStart.AddMonths(1);

                var count = await _context.Tasks.CountAsync(t =>
                    t.Status == TaskStatusEnum.Completed &&
                    t.CreatedAt >= monthStart &&
                    t.CreatedAt < monthEnd);

                monthly.Add(new MonthlyCompletionDto
                {
                    MonthLabel = month.ToString("MMM"),
                    Count = count
                });
            }

            // ── Tasks by category ─────────────────────────────────────
            var byCategory = await _context.Categories
                .Select(c => new CategoryTaskCountDto
                {
                    CategoryName = c.Name,
                    TaskCount = _context.Tasks.Count(t => t.CategoryId == c.Id
                        && t.CreatedAt >= sixMonthsAgo)
                })
                .OrderByDescending(x => x.TaskCount)
                .ToListAsync();

            // ── User performance table ────────────────────────────────
            var userPerf = await _context.Users
                .Select(u => new
                {
                    u.FullName,
                    Assigned = _context.Tasks.Count(t => t.AssignedToUserId == u.Id),
                    Completed = _context.Tasks.Count(t => t.AssignedToUserId == u.Id
                                                       && t.Status == TaskStatusEnum.Completed),
                    Overdue = _context.Tasks.Count(t => t.AssignedToUserId == u.Id
                                                       && t.DueDate < now
                                                       && t.Status != TaskStatusEnum.Completed
                                                       && t.Status != TaskStatusEnum.Cancelled)
                })
                .Where(x => x.Assigned > 0)
                .OrderByDescending(x => x.Assigned)
                .ToListAsync();

            var performance = userPerf.Select(u => new UserPerformanceDto
            {
                UserName = u.FullName,
                TasksAssigned = u.Assigned,
                Completed = u.Completed,
                Overdue = u.Overdue,
                CompletionRate = u.Assigned > 0
                                    ? Math.Round((double)u.Completed / u.Assigned * 100, 0)
                                    : 0
            }).ToList();

            return new ReportsDto
            {
                TotalTasksCompleted = totalCompleted,
                CompletedThisWeek = completedThisWeek,
                AverageCompletionDays = avgDays,
                MostActiveUserName = mostActive?.FullName ?? "-",
                MostActiveUserTasks = mostActive?.Count ?? 0,
                OverdueRate = overdueRate,
                MonthlyCompletions = monthly,
                TasksByCategory = byCategory,
                UserPerformance = performance
            };
        }
    }
}
