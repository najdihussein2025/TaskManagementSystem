namespace TaskManagementSystem.Interfaces.Services
{
    public interface IForgotPasswordService
    {
        Task<(bool Success, string? Error)> SendOtpAsync(string email);
        Task<(bool Success, string? Error)> VerifyOtpAsync(string email, string otp);
        Task<(bool Success, string? Error)> ResetPasswordAsync(
            string email, string otp, string newPassword, string confirmPassword);
    }
}
