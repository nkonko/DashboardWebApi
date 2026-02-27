namespace WebApi.Core.Models
{
    public class SubscriptionUpdateDto
    {
        public string? Plan { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
