using System;
using System.Collections.Generic;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Models
{
    public class ApplicationUser
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public UserStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public int RoleId { get; set; }

        public Role Role { get; set; }

        public ICollection<TaskItem> CreatedTasks { get; set; }= new List<TaskItem>();

        public ICollection<TaskItem> AssignedTasks { get; set; }= new List<TaskItem>();

        public ICollection<TaskComment> Comments { get; set; }= new List<TaskComment>();
    }
}