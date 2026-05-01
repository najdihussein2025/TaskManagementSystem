namespace TaskManagementSystem.Dtos.Reports
{
    public class ReportsDto
    {
        // Stat cards
        public int TotalTasksCompleted { get; set; }
        public int CompletedThisWeek { get; set; }
        public double AverageCompletionDays { get; set; }
        public string MostActiveUserName { get; set; } = "-";
        public int MostActiveUserTasks { get; set; }
        public double OverdueRate { get; set; }

        // Chart — completed tasks per month (last 6 months)
        public List<MonthlyCompletionDto> MonthlyCompletions { get; set; } = new();

        // Tasks by category
        public List<CategoryTaskCountDto> TasksByCategory { get; set; } = new();

        // User performance table
        public List<UserPerformanceDto> UserPerformance { get; set; } = new();
    }

    public class MonthlyCompletionDto
    {
        public string MonthLabel { get; set; } = string.Empty;  // e.g. "Jan"
        public int Count { get; set; }
    }

    public class CategoryTaskCountDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public int TaskCount { get; set; }
    }

    public class UserPerformanceDto
    {
        public string UserName { get; set; } = string.Empty;
        public int TasksAssigned { get; set; }
        public int Completed { get; set; }
        public int Overdue { get; set; }
        public double CompletionRate { get; set; }
    }
}
