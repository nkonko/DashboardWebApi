using WebApi.Core.Models;

namespace WebApi.Infrastructure.Services.Interfaces
{
    public interface ISubscriptionService
    {
        Task<IEnumerable<SubscriptionDto>> GetSubscriptionsAsync();
        Task<SubscriptionDto?> GetSubscriptionByIdAsync(string id);
        Task<SubscriptionDto> CreateSubscriptionAsync(SubscriptionCreateDto dto);
        Task<bool> UpdateSubscriptionAsync(string id, SubscriptionUpdateDto dto);
        Task<bool> DeleteSubscriptionAsync(string id);
    }
}
