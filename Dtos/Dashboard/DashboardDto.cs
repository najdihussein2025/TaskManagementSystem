namespace TaskManagementSystem.Dtos.Dashboard
{
    public class DashboardDto
    {
        public int TotalUsers { get; set; }
        public int NewUsersThisMonth { get; set; }
        public int TotalTasks { get; set; }
        public int NewTasksThisMonth { get; set; }
        public int InProgressTasks { get; set; }
        public int OverdueTasks { get; set; }
        public int PendingTasks { get; set; }
        public int CompletedTasks { get; set; }
        public List<RecentUserDto> RecentUsers { get; set; } = new();
        public List<RecentTaskDto> RecentTasks { get; set; } = new();
    }

    public class RecentUserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Initials { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int TaskCount { get; set; }
        public bool IsActive { get; set; }
    }

    public class RecentTaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string AssignedToName { get; set; } = "Unassigned";
        public string CategoryName { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
