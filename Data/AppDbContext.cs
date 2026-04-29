using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Models;



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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(user => user.Role)
                .WithMany(role => role.Users)
                .HasForeignKey(user => user.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskItem>()
                .HasOne(task => task.Category)
                .WithMany(category => category.Tasks)
                .HasForeignKey(task => task.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskItem>()
                .HasOne(task => task.CreatedByUser)
                .WithMany(user => user.CreatedTasks)
                .HasForeignKey(task => task.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskItem>()
                .HasOne(task => task.AssignedToUser)
                .WithMany(user => user.AssignedTasks)
                .HasForeignKey(task => task.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskComment>()
                .HasOne(comment => comment.Task)
                .WithMany(task => task.Comments)
                .HasForeignKey(comment => comment.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskComment>()
                .HasOne(comment => comment.User)
                .WithMany(user => user.Comments)
                .HasForeignKey(comment => comment.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}