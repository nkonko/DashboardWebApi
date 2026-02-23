using WebApi.Models;

namespace WebApi.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<bool> ProcessWebhookAsync(PaymentWebhookDto dto);
        Task<IEnumerable<PaymentWebhookResponseDto>> GetWebhookHistoryAsync();
    }
}
