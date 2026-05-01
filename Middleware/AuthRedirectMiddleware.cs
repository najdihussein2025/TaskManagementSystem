using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TaskManagementSystem.Middleware
{
    public class AuthRedirectMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;

        public AuthRedirectMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";

            bool isAdminRoute = path.StartsWith("/admin");
            bool isUserRoute = path.StartsWith("/user");

            // Skip auth pages and static files
            if (!isAdminRoute && !isUserRoute)
            {
                await _next(context);
                return;
            }

            // Try to get and validate JWT from cookie
            var token = context.Request.Cookies["jwt"];
            string? role = null;
            bool isValid = false;

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var key = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
                    var handler = new JwtSecurityTokenHandler();

                    var principal = handler.ValidateToken(token,
                        new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = key,
                            ValidateIssuer = true,
                            ValidIssuer = _config["Jwt:Issuer"],
                            ValidateAudience = true,
                            ValidAudience = _config["Jwt:Audience"],
                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.Zero
                        }, out _);

                    isValid = true;
                    role = principal.FindFirst(ClaimTypes.Role)?.Value
                           ?? principal.FindFirst("role")?.Value;

                    Console.WriteLine($"[MIDDLEWARE] Path: {path} | Role: {role}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[MIDDLEWARE] Token invalid: {ex.Message}");
                    isValid = false;
                }
            }

            // ── Not logged in at all ───────────────────────────────
            if (!isValid)
            {
                context.Response.Cookies.Delete("jwt");
                context.Session.SetString("LoginRequired",
                    "Please log in first to access this page.");
                context.Response.Redirect("/Auth/Login");
                return;
            }

            // ── Logged in as User trying to access /admin ──────────
            if (isAdminRoute && role != "Admin")
            {
                context.Session.SetString("LoginRequired",
                    "Access denied. Admin privileges required.");
                context.Response.Redirect("/Auth/Login");
                return;
            }

            // ── Logged in as Admin trying to access /user ──────────
            if (isUserRoute && role != "User")
            {
                // Admin should not access user dashboard
                // Redirect admin to their own dashboard
                context.Response.Redirect("/Admin/Dashboard");
                return;
            }

            await _next(context);
        }
    }
}
