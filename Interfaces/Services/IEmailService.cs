namespace TaskManagementSystem.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(string toEmail, string toName, string otpCode);
    }
}
