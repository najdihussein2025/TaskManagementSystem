using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TaskManagementSystem.Authentication;
using TaskManagementSystem.DTOs.Auth;
using TaskManagementSystem.Interfaces.Services;

namespace TaskManagementSystem.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            // Read redirect message from session
            var msg = HttpContext.Session.GetString("LoginRequired");
            if (!string.IsNullOrEmpty(msg))
            {
                ViewBag.Error = msg;
                HttpContext.Session.Remove("LoginRequired");
            }

            // If already logged in redirect away
            var token = Request.Cookies["jwt"];
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwt = handler.ReadJwtToken(token);
                    var exp = jwt.ValidTo;
                    if (exp > DateTime.UtcNow)
                    {
                        var role = jwt.Claims
                            .FirstOrDefault(c => c.Type == "role" ||
                                                 c.Type == ClaimTypes.Role)
                            ?.Value;
                        return role == "Admin"
                            ? RedirectToAction("Dashboard", "Admin")
                            : RedirectToAction("Dashboard", "User");
                    }
                }
                catch { }
            }

            return View(new LoginDto());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Please enter your email and password.";
                return View(dto);
            }

            var (success, token, role, error) = await _authService.LoginAsync(dto);

            if (!success)
            {
                ViewBag.Error = error ?? "Invalid email or password. Please try again.";
                return View(dto);
            }

            Response.Cookies.Append("jwt", token!, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(8)
            });

            return role == "Admin"
                ? RedirectToAction("Dashboard", "Admin")
                : RedirectToAction("Dashboard", "User");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register() => View(new RegisterDto());

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Please correct the errors below.";
                return View(dto);
            }

            var (success, error) = await _authService.RegisterAsync(dto);

            if (!success)
            {
                ViewBag.Error = error;
                return View(dto);
            }

            TempData["Success"] = "Account created. Please sign in.";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied() => View();

     
        [HttpGet]
        [AllowAnonymous]
        [ActionName("Logout")]
        public IActionResult LogoutGet()
        {
            Response.Cookies.Delete(AuthCookie.Jwt, new CookieOptions { Path = "/" });
            return RedirectToAction(nameof(Login), "Auth");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ActionName("Logout")]
        public IActionResult LogoutPost()
        {
            Response.Cookies.Delete(AuthCookie.Jwt, new CookieOptions { Path = "/" });
            return RedirectToAction(nameof(Login), "Auth");
        }

        // ── API endpoints (return JSON) ───────────────────────────

        [HttpPost("api/auth/login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginApi([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = "Invalid input." });

            var (success, token, role, error) = await _authService.LoginAsync(dto);

            if (!success)
                return Unauthorized(new { error });

            return Ok(new { token, role });
        }

        [HttpPost("api/auth/register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterApi([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = "Invalid input." });

            var (success, error) = await _authService.RegisterAsync(dto);

            if (!success)
                return BadRequest(new { error });

            return Ok(new { message = "Account created successfully." });
        }

        [HttpPost("api/auth/logout")]
        [AllowAnonymous]
        public IActionResult LogoutApi()
        {
            // JWT is stateless — client deletes the token
            return Ok(new { message = "Logged out." });
        }
    }
}
