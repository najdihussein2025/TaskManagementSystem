using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.DTOs.Auth;
using TaskManagementSystem.Interfaces.Services;

namespace TaskManagementSystem.Controllers
{
    [Route("ForgotPassword")]
    public class ForgotPasswordController : Controller
    {
        private readonly IForgotPasswordService _forgotPasswordService;

        public ForgotPasswordController(
            IForgotPasswordService forgotPasswordService)
        {
            _forgotPasswordService = forgotPasswordService;
        }

        // ── Page 1: Enter Email ───────────────────────────────────
        // Anyone can access this page
        [HttpGet("")]
        public IActionResult Index()
        {
            // Clear any previous reset session
            HttpContext.Session.Remove("ResetEmail");
            HttpContext.Session.Remove("OtpVerified");
            HttpContext.Session.Remove("ResetOtp");
            return View();
        }

        [HttpPost("")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ForgotPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                ViewBag.Error = "Please enter your email.";
                return View(dto);
            }

            var (success, error) = await _forgotPasswordService
                .SendOtpAsync(dto.Email);

            // Save email in session — unlocks VerifyOtp page
            HttpContext.Session.SetString("ResetEmail", dto.Email);
            HttpContext.Session.Remove("OtpVerified");
            HttpContext.Session.Remove("ResetOtp");

            TempData["Message"] =
                "If this email exists, a code has been sent.";

            return RedirectToAction("VerifyOtp");
        }

        // ── Page 2: Enter OTP ─────────────────────────────────────
        // Only accessible if ResetEmail session exists
        [HttpGet("VerifyOtp")]
        public IActionResult VerifyOtp()
        {
            var email = HttpContext.Session.GetString("ResetEmail");
            if (string.IsNullOrEmpty(email))
            {
                // User skipped step 1 — send back
                TempData["FlowError"] =
                    "Please enter your email first.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Email = email;
            ViewBag.Message = TempData["Message"];
            return View();
        }

        [HttpPost("VerifyOtp")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyOtp(VerifyOtpDto dto)
        {
            var email = HttpContext.Session.GetString("ResetEmail");
            if (string.IsNullOrEmpty(email))
            {
                TempData["FlowError"] =
                    "Please enter your email first.";
                return RedirectToAction(nameof(Index));
            }

            // Always use session email — not form email
            // Prevents tampering
            dto.Email = email;

            var (success, error) = await _forgotPasswordService
                .VerifyOtpAsync(dto.Email, dto.OtpCode);

            if (!success)
            {
                ViewBag.Email = email;
                ViewBag.Error = error;
                return View(dto);
            }

            // OTP verified — save to session, unlocks ResetPassword
            HttpContext.Session.SetString("OtpVerified", "true");
            HttpContext.Session.SetString("ResetOtp", dto.OtpCode);

            return RedirectToAction("ResetPassword");
        }

        // ── Page 3: New Password ──────────────────────────────────
        // Only accessible if email + OTP both verified in session
        [HttpGet("ResetPassword")]
        public IActionResult ResetPassword()
        {
            var email = HttpContext.Session.GetString("ResetEmail");
            var otpVerified = HttpContext.Session.GetString("OtpVerified");
            var otp = HttpContext.Session.GetString("ResetOtp");

            // Block if step 1 not done
            if (string.IsNullOrEmpty(email))
            {
                TempData["FlowError"] =
                    "Please enter your email first.";
                return RedirectToAction(nameof(Index));
            }

            // Block if step 2 not done
            if (otpVerified != "true" || string.IsNullOrEmpty(otp))
            {
                TempData["FlowError"] =
                    "Please verify your code first.";
                return RedirectToAction(nameof(VerifyOtp));
            }

            ViewBag.Email = email;
            ViewBag.OtpCode = otp;
            return View();
        }

        [HttpPost("ResetPassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var email = HttpContext.Session.GetString("ResetEmail");
            var otpVerified = HttpContext.Session.GetString("OtpVerified");
            var otp = HttpContext.Session.GetString("ResetOtp");

            // Double check session integrity
            if (string.IsNullOrEmpty(email) || otpVerified != "true" ||
                string.IsNullOrEmpty(otp))
            {
                TempData["FlowError"] =
                    "Session expired. Please start again.";
                return RedirectToAction(nameof(Index));
            }

            // Use session values — never trust form hidden fields
            dto.Email = email;
            dto.OtpCode = otp;

            var (success, error) = await _forgotPasswordService
                .ResetPasswordAsync(
                    dto.Email, dto.OtpCode,
                    dto.NewPassword, dto.ConfirmPassword);

            if (!success)
            {
                ViewBag.Email = email;
                ViewBag.OtpCode = otp;
                ViewBag.Error = error;
                return View(dto);
            }

            // Clear all reset session data
            HttpContext.Session.Remove("ResetEmail");
            HttpContext.Session.Remove("OtpVerified");
            HttpContext.Session.Remove("ResetOtp");

            TempData["Success"] =
                "Password reset successfully. Please log in.";
            return RedirectToAction("Login", "Auth");
        }
    }
}
