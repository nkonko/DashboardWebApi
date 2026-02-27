using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using WebApi.Core.Models;
using WebApi.Infrastructure.Persistence;
using WebApi.Infrastructure.Services.Interfaces;

namespace WebApi.Infrastructure.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly WebApiDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(
            WebApiDbContext context,
            IHttpClientFactory httpClientFactory,
            ILogger<DashboardService> logger)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("IdentityAPI");
                
                // Llamar a IdentityAPI para obtener conteos de Users y Roles
                var response = await client.GetAsync("/api/internal/counts");
                
                int totalUsers = 0;
                int totalRoles = 0;

                if (response.IsSuccessStatusCode)
                {
                    var counts = await response.Content.ReadFromJsonAsync<InternalCountsDto>();
                    if (counts != null)
                    {
                        totalUsers = counts.TotalUsers;
                        totalRoles = counts.TotalRoles;
                    }
                }
                else
                {
                    _logger.LogWarning("Failed to fetch counts from IdentityAPI. Status: {StatusCode}", response.StatusCode);
                }

                // Contar subscripciones en nuestra propia BD
                var totalSubscriptions = await _context.Subscriptions.CountAsync();

                return new DashboardSummaryDto
                {
                    TotalUsers = totalUsers,
                    TotalRoles = totalRoles,
                    TotalSubscriptions = totalSubscriptions
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dashboard summary");
                return new DashboardSummaryDto();
            }
        }

        public async Task<DashboardMetricsDto> GetMetricsAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("IdentityAPI");
                var response = await client.GetAsync("/api/internal/counts");

                int activeUsers = 0;

                if (response.IsSuccessStatusCode)
                {
                    var counts = await response.Content.ReadFromJsonAsync<InternalCountsDto>();
                    if (counts != null)
                    {
                        activeUsers = counts.TotalUsers;
                    }
                }

                // Contar nuevas subscripciones este mes
                var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                var newSubscriptionsThisMonth = await _context.Subscriptions
                    .Where(s => s.CreatedAt >= startOfMonth)
                    .CountAsync();

                return new DashboardMetricsDto
                {
                    ActiveUsers = activeUsers,
                    NewUsersThisMonth = 0, // Placeholder
                    RevenueThisMonth = 0    // Placeholder
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dashboard metrics");
                return new DashboardMetricsDto();
            }
        }

        public Task<DashboardRecentDto> GetRecentAsync()
        {
            // Placeholder para actividades recientes
            return Task.FromResult(new DashboardRecentDto
            {
                RecentActivities = new List<string>()
            });
        }

        public Task<IEnumerable<DashboardNotificationDto>> GetNotificationsAsync()
        {
            // Placeholder para notificaciones
            return Task.FromResult<IEnumerable<DashboardNotificationDto>>(new List<DashboardNotificationDto>());
        }
    }

    // DTO interno para la respuesta de IdentityAPI
    internal class InternalCountsDto
    {
        public int TotalUsers { get; set; }
        public int TotalRoles { get; set; }
    }
}
