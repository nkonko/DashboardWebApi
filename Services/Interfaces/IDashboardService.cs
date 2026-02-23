using WebApi.Models;

namespace WebApi.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetSummaryAsync();
        Task<DashboardMetricsDto> GetMetricsAsync();
        Task<DashboardRecentDto> GetRecentAsync();
        Task<IEnumerable<DashboardNotificationDto>> GetNotificationsAsync();
    }
}
