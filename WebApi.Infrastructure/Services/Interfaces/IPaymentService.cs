using WebApi.Core.Models;

namespace WebApi.Infrastructure.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<bool> ProcessWebhookAsync(PaymentWebhookDto dto);
        Task<IEnumerable<PaymentWebhookResponseDto>> GetWebhookHistoryAsync();
    }
}
