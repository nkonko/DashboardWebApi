using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities
{
    [Table("payment_webhooks")]
    public class PaymentWebhook
    {
        [Key]
        [Column("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [Column("event")]
        [MaxLength(100)]
        public string Event { get; set; } = string.Empty;

        [Required]
        [Column("data")]
        public string Data { get; set; } = string.Empty;

        [Required]
        [Column("processed")]
        public bool Processed { get; set; } = false;

        [Column("processed_at")]
        public DateTime? ProcessedAt { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
