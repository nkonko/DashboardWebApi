using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Core.Entities
{
    [Table("app_settings")]
    public class AppSetting
    {
        [Key]
        [Column("key")]
        [MaxLength(100)]
        public string Key { get; set; } = string.Empty;

        [Required]
        [Column("value")]
        [MaxLength(500)]
        public string Value { get; set; } = string.Empty;

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}
