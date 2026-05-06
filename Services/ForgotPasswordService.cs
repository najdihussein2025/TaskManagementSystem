using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Data;
using TaskManagementSystem.Interfaces.Services;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Services
{
    public class ForgotPasswordService : IForgotPasswordService
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public ForgotPasswordService(
            AppDbContext context,
            IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<(bool Success, string? Error)>
            SendOtpAsync(string email)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            // Always return success even if email not found
            // Prevents email enumeration attacks
            if (user == null)
                return (true, null);

            // Invalidate any existing unused tokens for this user
            var existing = await _context.PasswordResetTokens
                .Where(t => t.UserId == user.Id && !t.IsUsed)
                .ToListAsync();
            foreach (var t in existing) t.IsUsed = true;
            await _context.SaveChangesAsync();

            // Generate 6-digit OTP
            var otp = new Random().Next(100000, 999999).ToString();

            var token = new PasswordResetToken
            {
                UserId = user.Id,
                OtpCode = otp,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            await _context.PasswordResetTokens.AddAsync(token);
            await _context.SaveChangesAsync();

            // Send email
            try
            {
                await _emailService.SendOtpEmailAsync(
                    user.Email, user.FullName, otp);
            }
            catch (Exception ex)
            {
                token.IsUsed = true;
                await _context.SaveChangesAsync();
                Console.WriteLine($"[EMAIL] Failed sending OTP to {email}: {ex.Message}");
                return (false, "Could not send reset code. Please try again.");
            }

            Console.WriteLine($"[OTP] Generated {otp} for {email}");
            return (true, null);
        }

        public async Task<(bool Success, string? Error)>
            VerifyOtpAsync(string email, string otp)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return (false, "Invalid request.");

            var token = await _context.PasswordResetTokens
                .Where(t => t.UserId == user.Id &&
                            t.OtpCode == otp &&
                            !t.IsUsed &&
                            t.ExpiresAt > DateTime.UtcNow)
                .FirstOrDefaultAsync();

            if (token == null)
                return (false, "Invalid or expired code.");

            return (true, null);
        }

        public async Task<(bool Success, string? Error)>
            ResetPasswordAsync(
                string email, string otp,
                string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
                return (false, "Passwords do not match.");

            if (newPassword.Length < 6)
                return (false, "Password must be at least 6 characters.");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return (false, "Invalid request.");

            var token = await _context.PasswordResetTokens
                .Where(t => t.UserId == user.Id &&
                            t.OtpCode == otp &&
                            !t.IsUsed &&
                            t.ExpiresAt > DateTime.UtcNow)
                .FirstOrDefaultAsync();

            if (token == null)
                return (false, "Invalid or expired code.");

            // Update password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            // Mark token as used
            token.IsUsed = true;

            await _context.SaveChangesAsync();

            Console.WriteLine($"[OTP] Password reset for {email}");
            return (true, null);
        }
    }
}
