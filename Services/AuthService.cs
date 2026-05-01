using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManagementSystem.Data;
using TaskManagementSystem.DTOs.Auth;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Interfaces.Services;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<(bool Success, string? Token, string? Role, string? Error)>
            LoginAsync(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                return (false, null, null, "Invalid email or password.");

            if ((int)user.Status != 1)
                return (false, null, null, "User is inactive. Contact admin.");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return (false, null, null, "Invalid email or password.");

            var role = user.RoleId == 2 ? "Admin" : "User";
            var token = GenerateJwtToken(user, role);

            return (true, token, role, null);
        }

        public async Task<(bool Success, string? Token, string? Role, string? Error)>
            GoogleLoginAsync(string googleId, string email, string fullName)
        {
            // Check if user already exists by GoogleId
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.GoogleId == googleId);

            // If not found by GoogleId, check by email
            if (user == null)
            {
                user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user != null)
                {
                    // Link GoogleId to existing account
                    user.GoogleId = googleId;
                    await _context.SaveChangesAsync();
                }
            }

            // If still not found - auto-register new user
            if (user == null)
            {
                user = new ApplicationUser
                {
                    FullName = fullName,
                    Email = email,
                    GoogleId = googleId,
                    PasswordHash = string.Empty, // no password for Google users
                    RoleId = 1, // default role = User
                    Status = UserStatus.Active, // active
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }

            if (user.Status == UserStatus.Inactive)
                return (false, null, null, "Account is inactive.");

            var role = user.RoleId == 2 ? "Admin" : "User";
            var token = GenerateJwtToken(user, role);

            return (true, token, role, null);
        }

        public async Task<(bool Success, string? Error)>
            RegisterAsync(RegisterDto dto)
        {
            var exists = await _context.Users
                .AnyAsync(u => u.Email == dto.Email);

            if (exists)
                return (false, "Email already registered.");

            var user = new ApplicationUser
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleId = 1,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Error)>
            ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return (false, "User not found.");

            if (string.IsNullOrWhiteSpace(dto.CurrentPassword))
                return (false, "Current password is required.");

            if (string.IsNullOrWhiteSpace(dto.NewPassword))
                return (false, "New password is required.");

            if (dto.NewPassword != dto.ConfirmPassword)
                return (false, "Passwords do not match.");

            if (dto.NewPassword.Length < 6)
                return (false, "Password must be at least 6 characters.");

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                return (false, "Current password is incorrect.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();
            return (true, null);
        }

        private string GenerateJwtToken(ApplicationUser user, string role)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role),
                new Claim("RoleId", user.RoleId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(
                    _config.GetValue("Jwt:ExpireHours", 8)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
