using System;
using System.Collections.Generic;
using TaskPriority = TaskManagementSystem.Enums.TaskPriority;
using TaskStatus = TaskManagementSystem.Enums.TaskStatus;

namespace TaskManagementSystem.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public TaskPriority Priority { get; set; }

        public TaskStatus Status { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CategoryId { get; set; }

        public int CreatedByUserId { get; set; }

        public int AssignedToUserId { get; set; }

        public Category Category { get; set; }

        public ApplicationUser CreatedByUser { get; set; }

        public ApplicationUser AssignedToUser { get; set; }

        public ICollection<TaskComment> Comments { get; set; }= new List<TaskComment>();
    }
}