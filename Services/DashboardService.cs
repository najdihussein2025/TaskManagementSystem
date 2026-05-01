using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Data;
using TaskManagementSystem.Dtos.Dashboard;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Interfaces.Services;
using TaskStatusEnum = TaskManagementSystem.Enums.TaskStatus;

namespace TaskManagementSystem.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardDto> GetDashboardDataAsync()
        {
            var now = DateTime.UtcNow;
            var monthStart = new DateTime(now.Year, now.Month, 1);

            var totalUsers = await _context.Users.CountAsync();
            var newUsersThisMonth = await _context.Users.CountAsync(u => u.CreatedAt >= monthStart);
            var totalTasks = await _context.Tasks.CountAsync();
            var newTasksThisMonth = await _context.Tasks.CountAsync(t => t.CreatedAt >= monthStart);
            var inProgressTasks = await _context.Tasks.CountAsync(t => t.Status == TaskStatusEnum.InProgress);
            var overdueTasks = await _context.Tasks.CountAsync(t =>
                t.DueDate < now &&
                t.Status != TaskStatusEnum.Completed &&
                t.Status != TaskStatusEnum.Cancelled);
            var pendingTasks = await _context.Tasks.CountAsync(t => t.Status == TaskStatusEnum.Pending);
            var completedTasks = await _context.Tasks.CountAsync(t => t.Status == TaskStatusEnum.Completed);

            var recentUserEntities = await _context.Users
                .Include(u => u.Role)
                .OrderByDescending(u => u.CreatedAt)
                .Take(5)
                .ToListAsync();

            var recentUserIds = recentUserEntities.Select(u => u.Id).ToList();
            var taskCounts = await _context.Tasks
                .Where(t => recentUserIds.Contains(t.AssignedToUserId))
                .GroupBy(t => t.AssignedToUserId)
                .Select(g => new { UserId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.UserId, x => x.Count);

            var recentUsers = recentUserEntities.Select(u => new RecentUserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Initials = GetInitials(u.FullName),
                Role = u.Role?.Name ?? string.Empty,
                TaskCount = taskCounts.TryGetValue(u.Id, out var count) ? count : 0,
                IsActive = u.Status == UserStatus.Active
            }).ToList();

            var recentTasks = await _context.Tasks
                .Include(t => t.AssignedToUser)
                .Include(t => t.Category)
                .OrderByDescending(t => t.CreatedAt)
                .Take(5)
                .Select(t => new RecentTaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    AssignedToName = t.AssignedToUser != null ? t.AssignedToUser.FullName : "Unassigned",
                    CategoryName = t.Category != null ? t.Category.Name : "-",
                    DueDate = t.DueDate,
                    Priority = t.Priority.ToString(),
                    Status = t.Status.ToString()
                })
                .ToListAsync();

            return new DashboardDto
            {
                TotalUsers = totalUsers,
                NewUsersThisMonth = newUsersThisMonth,
                TotalTasks = totalTasks,
                NewTasksThisMonth = newTasksThisMonth,
                InProgressTasks = inProgressTasks,
                OverdueTasks = overdueTasks,
                PendingTasks = pendingTasks,
                CompletedTasks = completedTasks,
                RecentUsers = recentUsers,
                RecentTasks = recentTasks
            };
        }

        private static string GetInitials(string? fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return string.Empty;

            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return string.Empty;
            if (parts.Length == 1)
                return parts[0][0].ToString().ToUpperInvariant();

            return $"{char.ToUpperInvariant(parts[0][0])}{char.ToUpperInvariant(parts[^1][0])}";
        }
    }
}
