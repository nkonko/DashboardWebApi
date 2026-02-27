namespace WebApi.Core.Models
{
    public class SubscriptionCreateDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Plan { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
    }
}
