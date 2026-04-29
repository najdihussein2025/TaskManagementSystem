using System;

namespace TaskManagementSystem.Models
{
    public class TaskComment
    {
        public int Id { get; set; }

        public string CommentText { get; set; }

        public DateTime CreatedAt { get; set; }

        public int TaskId { get; set; }

        public int UserId { get; set; }

        public TaskItem Task { get; set; }

        public ApplicationUser User { get; set; }
    }
}