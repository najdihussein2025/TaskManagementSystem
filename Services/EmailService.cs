using System.Net;
using System.Net.Mail;
using TaskManagementSystem.Interfaces.Services;

namespace TaskManagementSystem.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendOtpEmailAsync(
            string toEmail, string toName, string otpCode)
        {
            var host = _config["Email:Host"]!;
            var port = _config.GetValue<int>("Email:Port");
            var username = _config["Email:Username"]!;
            var password = _config["Email:Password"]!;
            var from = _config["Email:From"]!;
            var fromName = _config["Email:FromName"]!;

            var smtpClient = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            var message = new MailMessage
            {
                From = new MailAddress(from, fromName),
                Subject = "TaskFlow — Your Password Reset Code",
                IsBodyHtml = true,
                Body = $@"
<!DOCTYPE html>
<html>
<body style='font-family: Arial, sans-serif; background:#f4f4f4; padding:20px;'>
  <div style='max-width:480px; margin:0 auto; background:#fff; 
              border-radius:12px; padding:32px; box-shadow:0 2px 8px rgba(0,0,0,0.1);'>
    
    <div style='text-align:center; margin-bottom:24px;'>
      <h1 style='color:#3b82f6; margin:0;'>✓ TaskFlow</h1>
    </div>
    
    <h2 style='color:#1e293b; margin-bottom:8px;'>Password Reset Request</h2>
    <p style='color:#64748b; margin-bottom:24px;'>
      Hi {toName}, we received a request to reset your password.
      Use the code below — it expires in <strong>10 minutes</strong>.
    </p>
    
    <div style='background:#f1f5f9; border-radius:8px; 
                padding:24px; text-align:center; margin-bottom:24px;'>
      <p style='color:#64748b; font-size:14px; margin:0 0 8px;'>
        Your verification code
      </p>
      <h1 style='color:#3b82f6; font-size:48px; 
                 letter-spacing:8px; margin:0;'>
        {otpCode}
      </h1>
    </div>
    
    <p style='color:#94a3b8; font-size:13px; text-align:center;'>
      If you did not request this, ignore this email.<br/>
      Your password will not be changed.
    </p>
    
  </div>
</body>
</html>"
            };

            message.To.Add(new MailAddress(toEmail, toName));

            await smtpClient.SendMailAsync(message);
            Console.WriteLine($"[EMAIL] OTP sent to {toEmail}");
        }
    }
}
