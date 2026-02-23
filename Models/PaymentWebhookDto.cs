namespace WebApi.Models
{
    public class PaymentWebhookDto
    {
        public string Event { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
    }
}
