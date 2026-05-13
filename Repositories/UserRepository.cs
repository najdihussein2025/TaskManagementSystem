using TaskManagementSystem.Interfaces.Repositories;
using TaskManagementSystem.Models;
using TaskManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ApplicationUser>> GetAllAsync()
        {
            return await _context.Users.Include(user => user.Role).ToListAsync();
        }

        public async Task<ApplicationUser?> GetByIdAsync(int id)
        {
            return await _context.Users.Include(user => user.Role).FirstOrDefaultAsync(user => user.Id == id);
        }

        public async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Include(user => user.Role)
                .FirstOrDefaultAsync(user => user.Email == email);
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeUserId = null)
        {
            var normalized = email?.Trim() ?? string.Empty;
            var query = string.IsNullOrEmpty(normalized)
                ? _context.Users.Where(user => user.Email == string.Empty)
                : _context.Users.Where(user => user.Email.ToLower() == normalized.ToLower());

            if (excludeUserId.HasValue)
                query = query.Where(user => user.Id != excludeUserId.Value);

            return await query.AnyAsync();
        }

        public async Task AddAsync(ApplicationUser user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            return await _context.Roles.FirstOrDefaultAsync(role => role.Name == roleName);
        }

        public async Task AddRoleAsync(Role role)
        {
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user is null) return;
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ApplicationUser>> GetByStatusAsync(UserStatus status)
        {
            return await _context.Users
                .Include(user => user.Role)
                .Where(user => user.Status == status)
                .ToListAsync();
        }
    }
}