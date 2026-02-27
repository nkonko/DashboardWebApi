using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using WebApi.Core.Entities;
using WebApi.Core.Models;
using WebApi.Infrastructure.Persistence;
using WebApi.Infrastructure.Services.Interfaces;

namespace WebApi.Infrastructure.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly WebApiDbContext _context;
        private readonly ILogger<SubscriptionService> _logger;

        public SubscriptionService(WebApiDbContext context, ILogger<SubscriptionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<SubscriptionDto>> GetSubscriptionsAsync()
        {
            var subscriptions = await _context.Subscriptions
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            return subscriptions.Select(s => MapToDto(s));
        }

        public async Task<SubscriptionDto?> GetSubscriptionByIdAsync(string id)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);
            return subscription != null ? MapToDto(subscription) : null;
        }

        public async Task<SubscriptionDto> CreateSubscriptionAsync(SubscriptionCreateDto dto)
        {
            var subscription = new Subscription
            {
                Id = Guid.NewGuid().ToString(),
                UserId = dto.UserId,
                Plan = dto.Plan,
                StartDate = dto.StartDate ?? DateTime.UtcNow,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Subscription {Id} created for user {UserId}", subscription.Id, subscription.UserId);

            return MapToDto(subscription);
        }

        public async Task<bool> UpdateSubscriptionAsync(string id, SubscriptionUpdateDto dto)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription == null)
                return false;

            if (!string.IsNullOrEmpty(dto.Plan))
                subscription.Plan = dto.Plan;

            if (dto.EndDate.HasValue)
                subscription.EndDate = dto.EndDate.Value;

            if (dto.IsActive.HasValue)
                subscription.IsActive = dto.IsActive.Value;

            subscription.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Subscription {Id} updated", id);

            return true;
        }

        public async Task<bool> DeleteSubscriptionAsync(string id)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription == null)
                return false;

            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Subscription {Id} deleted", id);

            return true;
        }

        private static SubscriptionDto MapToDto(Subscription subscription)
        {
            return new SubscriptionDto
            {
                Id = subscription.Id,
                UserId = subscription.UserId,
                Plan = subscription.Plan,
                StartDate = subscription.StartDate,
                EndDate = subscription.EndDate,
                IsActive = subscription.IsActive
            };
        }
    }
}
