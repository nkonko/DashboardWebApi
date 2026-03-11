using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Core.Entities
{
    [Table("feature_flags")]
    public class FeatureFlag
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [Column("is_enabled")]
        public bool IsEnabled { get; set; } = false;

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("updated_by")]
        [MaxLength(100)]
        public string? UpdatedBy { get; set; }
    }
}
