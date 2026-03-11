namespace WebApi.Core.Models
{
    public class FeatureFlagDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class FeatureFlagCreateDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public bool IsEnabled { get; set; } = false;
    }

    public class FeatureFlagUpdateDto
    {
        public string? Description { get; set; }
        public bool IsEnabled { get; set; }
    }

    public class FeatureFlagToggleDto
    {
        public bool IsEnabled { get; set; }
    }

    /// <summary>
    /// Simplified DTO for client-side flag checking
    /// </summary>
    public class FeatureFlagStateDto
    {
        public string Name { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
    }
}
