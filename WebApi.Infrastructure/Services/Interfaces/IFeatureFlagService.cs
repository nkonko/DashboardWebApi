using WebApi.Core.Models;

namespace WebApi.Infrastructure.Services.Interfaces
{
    public interface IFeatureFlagService
    {
        /// <summary>
        /// Get all feature flags
        /// </summary>
        Task<IEnumerable<FeatureFlagDto>> GetAllAsync();

        /// <summary>
        /// Get all feature flag states (simplified for clients)
        /// </summary>
        Task<IEnumerable<FeatureFlagStateDto>> GetAllStatesAsync();

        /// <summary>
        /// Get a feature flag by ID
        /// </summary>
        Task<FeatureFlagDto?> GetByIdAsync(int id);

        /// <summary>
        /// Get a feature flag by name
        /// </summary>
        Task<FeatureFlagDto?> GetByNameAsync(string name);

        /// <summary>
        /// Check if a feature is enabled by name
        /// </summary>
        Task<bool> IsEnabledAsync(string name);

        /// <summary>
        /// Create a new feature flag
        /// </summary>
        Task<FeatureFlagDto> CreateAsync(FeatureFlagCreateDto dto, string? createdBy = null);

        /// <summary>
        /// Update a feature flag
        /// </summary>
        Task<FeatureFlagDto?> UpdateAsync(int id, FeatureFlagUpdateDto dto, string? updatedBy = null);

        /// <summary>
        /// Toggle a feature flag on/off
        /// </summary>
        Task<FeatureFlagDto?> ToggleAsync(int id, bool isEnabled, string? updatedBy = null);

        /// <summary>
        /// Delete a feature flag
        /// </summary>
        Task<bool> DeleteAsync(int id);
    }
}
