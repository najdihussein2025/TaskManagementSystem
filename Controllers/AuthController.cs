using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
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

            

            if (TempData["LoginError"] is string loginError && !string.IsNullOrWhiteSpace(loginError))
            {
                ViewBag.Error = loginError;
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
                SameSite = SameSiteMode.Lax,  // change here too
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

        // Step 1: User clicks Google button -> challenge Google
        [HttpGet]
        [Route("auth/google-login")]
        public IActionResult GoogleLogin()
        {
            Console.WriteLine("[GOOGLE] GoogleLogin triggered");
            var redirectUrl = Url.Action("GoogleCallback", "Auth", null, Request.Scheme);
            Console.WriteLine($"[GOOGLE] Callback URL: {redirectUrl}");

            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // Step 2: Google redirects back here after user approves
        [HttpGet]
        [Route("auth/google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            Console.WriteLine("[GOOGLE] GoogleCallback triggered");


            // Try authenticating with the external cookie scheme
            // Google uses "Google" scheme which stores in external cookie
            var result = await HttpContext.AuthenticateAsync("Google");

            Console.WriteLine($"[GOOGLE] Google scheme result: {result.Succeeded}");

            if (!result.Succeeded)
            {
                // Try with the Cookies scheme as fallback
                result = await HttpContext.AuthenticateAsync(
                    Microsoft.AspNetCore.Authentication.Cookies
                              .CookieAuthenticationDefaults.AuthenticationScheme);
                Console.WriteLine($"[GOOGLE] Cookie scheme result: {result.Succeeded}");
            }

            if (!result.Succeeded)
            {
                // Try reading directly from HttpContext.User
                // set by the Google middleware automatically
                Console.WriteLine("[GOOGLE] Trying HttpContext.User claims...");
                
                if (HttpContext.User?.Identity?.IsAuthenticated == true)
                {
                    var userClaims = HttpContext.User.Claims.ToList();
                    Console.WriteLine($"[GOOGLE] Claims count: {userClaims.Count}");
                    foreach (var c in userClaims)
                        Console.WriteLine($"[GOOGLE] Claim: {c.Type} = {c.Value}");
                }
                
                Console.WriteLine($"[GOOGLE] Failure reason: {result.Failure?.Message}");
                TempData["LoginError"] = "Google login failed. Please try again.";
                return RedirectToAction("Login");
            }

            var claims = result.Principal!.Claims.ToList();

            Console.WriteLine($"[GOOGLE] Claims found: {claims.Count}");
            foreach (var c in claims)
                Console.WriteLine($"[GOOGLE] {c.Type} = {c.Value}");

            var googleId = claims.FirstOrDefault(c =>
                               c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var email = claims.FirstOrDefault(c =>
                               c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
            var fullName = claims.FirstOrDefault(c =>
                               c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;

            Console.WriteLine($"[GOOGLE] GoogleId: {googleId}");
            Console.WriteLine($"[GOOGLE] Email: {email}");
            Console.WriteLine($"[GOOGLE] FullName: {fullName}");

            if (string.IsNullOrEmpty(googleId) || string.IsNullOrEmpty(email))
            {
                TempData["LoginError"] = "Could not get Google account info.";
                return RedirectToAction("Login");
            }

            var (success, token, role, error) =
                await _authService.GoogleLoginAsync(googleId, email, fullName ?? email);

            Console.WriteLine($"[GOOGLE] Service result - Success: {success}, Role: {role}, Error: {error}");

            if (!success)
            {
                TempData["LoginError"] = error;
                return RedirectToAction("Login");
            }

            /*
            Response.Cookies.Append("jwt", token!, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,  // Lax allows cookie on redirects
                Expires = DateTimeOffset.UtcNow.AddHours(8)
            });
            */

            

            Console.WriteLine($"[GOOGLE] JWT cookie set. Redirecting to {role} dashboard");

            return role == "Admin"
                ? RedirectToAction("Dashboard", "Admin")
                : RedirectToAction("Dashboard", "User");
        }

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
