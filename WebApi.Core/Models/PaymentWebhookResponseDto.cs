namespace WebApi.Core.Models
{
    public class PaymentWebhookResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Event { get; set; } = string.Empty;
        public bool Processed { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
