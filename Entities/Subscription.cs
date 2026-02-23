using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities
{
    [Table("subscriptions")]
    public class Subscription
    {
        [Key]
        [Column("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [Column("user_id")]
        [MaxLength(450)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Column("plan")]
        [MaxLength(100)]
        public string Plan { get; set; } = string.Empty;

        [Required]
        [Column("start_date")]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        [Required]
        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}
