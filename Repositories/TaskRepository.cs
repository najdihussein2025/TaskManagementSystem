using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Data;
using TaskManagementSystem.Interfaces.Repositories;
using TaskManagementSystem.Models;
using TaskPriority = TaskManagementSystem.Enums.TaskPriority;
using TaskStatus = TaskManagementSystem.Enums.TaskStatus;

namespace TaskManager.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;
 
        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }
 
        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            return await _context.Tasks
                .Include(t => t.AssignedTo)
                .Include(t => t.Category)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
 
        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            return await _context.Tasks
                .Include(t => t.AssignedTo)
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id);
        }
 
        public async Task<IEnumerable<TaskItem>> GetByUserIdAsync(int userId)
        {
            return await _context.Tasks
                .Include(t => t.AssignedTo)
                .Include(t => t.Category)
                .Where(t => t.AssignedToUserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
 
        public async Task<IEnumerable<TaskItem>> FilterAsync(
            string? search,
            string? status,
            string? priority,
            int? categoryId,
            int? userId)
        {
            var query = _context.Tasks
                .Include(t => t.AssignedTo)
                .Include(t => t.Category)
                .AsQueryable();
 
            // Search by title or description
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(t =>
                    t.Title.Contains(search) ||
                    (t.Description != null && t.Description.Contains(search)));
 
            // Filter by status enum
            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<TaskStatus>(status, out var parsedStatus))
                query = query.Where(t => t.Status == parsedStatus);
 
            // Filter by priority enum
            if (!string.IsNullOrWhiteSpace(priority) &&
                Enum.TryParse<PriorityLevel>(priority, out var parsedPriority))
                query = query.Where(t => t.Priority == parsedPriority);
 
            // Filter by category
            if (categoryId.HasValue)
                query = query.Where(t => t.CategoryId == categoryId);
 
            // Filter by assigned user
            if (userId.HasValue)
                query = query.Where(t => t.AssignedToUserId == userId);
 
            return await query
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
 
        public async Task AddAsync(TaskItem task)
        {
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
        }
 
        public async Task UpdateAsync(TaskItem task)
        {
            task.UpdatedAt = DateTime.UtcNow;
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }
 
        public async Task DeleteAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
        }
 
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Tasks.AnyAsync(t => t.Id == id);
        }
    }
}