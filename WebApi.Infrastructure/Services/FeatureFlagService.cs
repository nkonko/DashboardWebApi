using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using WebApi.Core.Entities;
using WebApi.Core.Models;
using WebApi.Infrastructure.Persistence;
using WebApi.Infrastructure.Services.Interfaces;

namespace WebApi.Infrastructure.Services
{
    public class FeatureFlagService : IFeatureFlagService
    {
        private readonly WebApiDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly ILogger<FeatureFlagService> _logger;
        
        private const string AllFlagsCacheKey = "FeatureFlags_All";
        private const string FlagStateCacheKeyPrefix = "FeatureFlag_";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(60);

        public FeatureFlagService(
            WebApiDbContext context,
            IMemoryCache cache,
            ILogger<FeatureFlagService> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IEnumerable<FeatureFlagDto>> GetAllAsync()
        {
            var flags = await _context.FeatureFlags
                .OrderBy(f => f.Name)
                .ToListAsync();

            return flags.Select(MapToDto);
        }

        public async Task<IEnumerable<FeatureFlagStateDto>> GetAllStatesAsync()
        {
            if (_cache.TryGetValue(AllFlagsCacheKey, out IEnumerable<FeatureFlagStateDto>? cachedStates) && cachedStates != null)
            {
                return cachedStates;
            }

            var flags = await _context.FeatureFlags
                .Select(f => new FeatureFlagStateDto
                {
                    Name = f.Name,
                    IsEnabled = f.IsEnabled
                })
                .ToListAsync();

            _cache.Set(AllFlagsCacheKey, flags, CacheDuration);

            return flags;
        }

        public async Task<FeatureFlagDto?> GetByIdAsync(int id)
        {
            var flag = await _context.FeatureFlags.FindAsync(id);
            return flag == null ? null : MapToDto(flag);
        }

        public async Task<FeatureFlagDto?> GetByNameAsync(string name)
        {
            var flag = await _context.FeatureFlags
                .FirstOrDefaultAsync(f => f.Name == name);
            return flag == null ? null : MapToDto(flag);
        }

        public async Task<bool> IsEnabledAsync(string name)
        {
            var cacheKey = $"{FlagStateCacheKeyPrefix}{name}";
            
            if (_cache.TryGetValue(cacheKey, out bool cachedValue))
            {
                return cachedValue;
            }

            var flag = await _context.FeatureFlags
                .FirstOrDefaultAsync(f => f.Name == name);

            var isEnabled = flag?.IsEnabled ?? false;

            _cache.Set(cacheKey, isEnabled, CacheDuration);

            return isEnabled;
        }

        public async Task<FeatureFlagDto> CreateAsync(FeatureFlagCreateDto dto, string? createdBy = null)
        {
            var existing = await _context.FeatureFlags
                .AnyAsync(f => f.Name == dto.Name);

            if (existing)
            {
                throw new InvalidOperationException($"Feature flag with name '{dto.Name}' already exists.");
            }

            var flag = new FeatureFlag
            {
                Name = dto.Name,
                Description = dto.Description,
                IsEnabled = dto.IsEnabled,
                CreatedAt = DateTime.UtcNow,
                UpdatedBy = createdBy
            };

            _context.FeatureFlags.Add(flag);
            await _context.SaveChangesAsync();

            InvalidateCache(flag.Name);
            _logger.LogInformation("Feature flag '{Name}' created by {User}", flag.Name, createdBy ?? "system");

            return MapToDto(flag);
        }

        public async Task<FeatureFlagDto?> UpdateAsync(int id, FeatureFlagUpdateDto dto, string? updatedBy = null)
        {
            var flag = await _context.FeatureFlags.FindAsync(id);
            if (flag == null) return null;

            flag.Description = dto.Description;
            flag.IsEnabled = dto.IsEnabled;
            flag.UpdatedAt = DateTime.UtcNow;
            flag.UpdatedBy = updatedBy;

            await _context.SaveChangesAsync();

            InvalidateCache(flag.Name);
            _logger.LogInformation("Feature flag '{Name}' updated by {User}. Enabled: {IsEnabled}", 
                flag.Name, updatedBy ?? "system", flag.IsEnabled);

            return MapToDto(flag);
        }

        public async Task<FeatureFlagDto?> ToggleAsync(int id, bool isEnabled, string? updatedBy = null)
        {
            var flag = await _context.FeatureFlags.FindAsync(id);
            if (flag == null) return null;

            flag.IsEnabled = isEnabled;
            flag.UpdatedAt = DateTime.UtcNow;
            flag.UpdatedBy = updatedBy;

            await _context.SaveChangesAsync();

            InvalidateCache(flag.Name);
            _logger.LogInformation("Feature flag '{Name}' toggled to {IsEnabled} by {User}", 
                flag.Name, isEnabled, updatedBy ?? "system");

            return MapToDto(flag);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var flag = await _context.FeatureFlags.FindAsync(id);
            if (flag == null) return false;

            var name = flag.Name;
            _context.FeatureFlags.Remove(flag);
            await _context.SaveChangesAsync();

            InvalidateCache(name);
            _logger.LogInformation("Feature flag '{Name}' deleted", name);

            return true;
        }

        private void InvalidateCache(string flagName)
        {
            _cache.Remove(AllFlagsCacheKey);
            _cache.Remove($"{FlagStateCacheKeyPrefix}{flagName}");
        }

        private static FeatureFlagDto MapToDto(FeatureFlag flag) => new()
        {
            Id = flag.Id,
            Name = flag.Name,
            Description = flag.Description,
            IsEnabled = flag.IsEnabled,
            CreatedAt = flag.CreatedAt,
            UpdatedAt = flag.UpdatedAt,
            UpdatedBy = flag.UpdatedBy
        };
    }
}
