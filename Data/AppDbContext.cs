using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;



namespace TaskManagementSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<TaskComment> Comments { get; set; }

    }
}