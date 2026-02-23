using Microsoft.EntityFrameworkCore;
using WebApi.Entities;
using WebApi.Models;
using WebApi.Persistence;
using WebApi.Services.Interfaces;

namespace WebApi.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly WebApiDbContext _context;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(WebApiDbContext context, ILogger<PaymentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> ProcessWebhookAsync(PaymentWebhookDto dto)
        {
            try
            {
                var webhook = new PaymentWebhook
                {
                    Id = Guid.NewGuid().ToString(),
                    Event = dto.Event,
                    Data = dto.Data,
                    Processed = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.PaymentWebhooks.Add(webhook);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Payment webhook {Id} received for event {Event}", webhook.Id, webhook.Event);

                // TODO: Implementar lógica de procesamiento de webhook según el evento
                // Por ahora solo lo almacenamos

                webhook.Processed = true;
                webhook.ProcessedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment webhook");
                return false;
            }
        }

        public async Task<IEnumerable<PaymentWebhookResponseDto>> GetWebhookHistoryAsync()
        {
            var webhooks = await _context.PaymentWebhooks
                .OrderByDescending(w => w.CreatedAt)
                .Take(50)
                .ToListAsync();

            return webhooks.Select(w => new PaymentWebhookResponseDto
            {
                Id = w.Id,
                Event = w.Event,
                Processed = w.Processed,
                ProcessedAt = w.ProcessedAt,
                CreatedAt = w.CreatedAt
            });
        }
    }
}
