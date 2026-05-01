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

            // Skip all auth-related and Google OAuth paths
            if (path.StartsWith("/auth") ||
                path.StartsWith("/signin-google") ||
                path.StartsWith("/signout") ||
                path == "/" ||
                path.StartsWith("/css") ||
                path.StartsWith("/js") ||
                path.StartsWith("/lib") ||
                path.StartsWith("/images"))
            {
                await _next(context);
                return;
            }

            bool isAdminRoute = path.StartsWith("/admin");
            bool isUserRoute = path.StartsWith("/user");

            if (!isAdminRoute && !isUserRoute)
            {
                await _next(context);
                return;
            }

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

                    Console.WriteLine($"[MIDDLEWARE] Path: {path} | Role: {role} | Valid: {isValid}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[MIDDLEWARE] Token error: {ex.Message}");
                    isValid = false;
                }
            }
            else
            {
                Console.WriteLine($"[MIDDLEWARE] No JWT cookie for path: {path}");
            }

            if (!isValid)
            {
                context.Session.SetString("LoginRequired",
                    "Please log in first to access this page.");
                context.Response.Redirect("/Auth/Login");
                return;
            }

            if (isAdminRoute && role != "Admin")
            {
                context.Session.SetString("LoginRequired",
                    "Access denied. Admin privileges required.");
                context.Response.Redirect("/Auth/Login");
                return;
            }

            if (isUserRoute && role != "User")
            {
                context.Response.Redirect("/Admin/Dashboard");
                return;
            }

            await _next(context);
        }
    }
}
