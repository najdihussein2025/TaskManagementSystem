using TaskManagementSystem.Interfaces.Services;
using TaskManagementSystem.Interfaces.Repositories;
using TaskManagementSystem.Models;
using TaskManagementSystem.DTOs.User;

namespace TaskManagementSystem.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task CreateAsync(CreateUserDto dto)
        {
            var roleName = string.IsNullOrWhiteSpace(dto.RoleName) ? "User" : dto.RoleName;
            var role = await _userRepository.GetRoleByNameAsync(roleName);

            if (role is null)
            {
                role = new Role { Name = roleName };
                await _userRepository.AddRoleAsync(role);
            }

            var user = new ApplicationUser
            {
                FullName = dto.FullName ?? string.Empty,
                Email = dto.Email ?? string.Empty,
                PasswordHash = dto.Password ?? string.Empty,
                IsActive = (dto.Status ?? "Active") == "Active",
                CreatedAt = DateTime.UtcNow,
                RoleId = role.Id
            };

            await _userRepository.AddAsync(user);
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(user => new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                RoleName = user.Role?.Name,
                IsActive = user.IsActive
            }).ToList();
        }

        public async Task<UserProfileDto?> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user is null) return null;

            return new UserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                RoleName = user.Role?.Name,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task UpdateAsync(UpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.Id);
            if (user is null) return;

            user.FullName = dto.FullName ?? user.FullName;
            user.Email = dto.Email ?? user.Email;
            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteAsync(int id)
        {
            await _userRepository.DeleteAsync(id);
        }

        public async Task AssignRoleAsync(AssignRoleDto dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user is null) return;

            user.RoleId = dto.RoleId;
            await _userRepository.UpdateAsync(user);
        }
    }
}