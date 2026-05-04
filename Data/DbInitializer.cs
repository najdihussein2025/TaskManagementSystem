using TaskManagementSystem.Data;
using TaskManagementSystem.Models;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Seed Roles if none exist
            if (!context.Roles.Any())
            {
                var roles = new Role[]
                {
                    new Role { Name = "Admin" },
                    new Role { Name = "User" }
                };
                context.Roles.AddRange(roles);
                context.SaveChanges();
            }

            // Seed Admin User if none exists
            if (!context.Users.Any(u => u.Email == "admin@example.com"))
            {
                var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Admin");

                if (adminRole != null)
                {
                    var adminUser = new ApplicationUser
                    {
                        FullName = "System Administrator",
                        Email = "admin@example.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                        RoleId = adminRole.Id,
                        Status = UserStatus.Active,
                        CreatedAt = DateTime.UtcNow
                    };

                    context.Users.Add(adminUser);
                    context.SaveChanges();
                }
            }
        }
    }
}