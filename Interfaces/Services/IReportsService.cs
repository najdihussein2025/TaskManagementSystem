using TaskManagementSystem.Dtos.Reports;

namespace TaskManagementSystem.Interfaces.Services
{
    public interface IReportsService
    {
        Task<ReportsDto> GetReportsDataAsync();
    }
}
