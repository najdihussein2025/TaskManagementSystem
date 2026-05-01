using TaskManagementSystem.Dtos.Dashboard;

namespace TaskManagementSystem.Interfaces.Services
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardDataAsync();
    }
}
