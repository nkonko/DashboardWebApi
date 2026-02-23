using Microsoft.EntityFrameworkCore;
using WebApi.Models;
using WebApi.Persistence;
using WebApi.Services.Interfaces;

namespace WebApi.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly WebApiDbContext _context;
        private readonly ILogger<SettingsService> _logger;

        public SettingsService(WebApiDbContext context, ILogger<SettingsService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<SettingsDto> GetSettingsAsync()
        {
            var companyName = await GetSettingValueAsync("CompanyName");
            var supportEmail = await GetSettingValueAsync("SupportEmail");

            return new SettingsDto
            {
                CompanyName = companyName ?? "AzzDashboard",
                SupportEmail = supportEmail ?? "support@azzdashboard.com"
            };
        }

        public async Task<bool> UpdateSettingsAsync(SettingsUpdateDto dto)
        {
            try
            {
                await UpdateSettingAsync("CompanyName", dto.CompanyName);
                await UpdateSettingAsync("SupportEmail", dto.SupportEmail);

                await _context.SaveChangesAsync();

                _logger.LogInformation("Settings updated successfully");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating settings");
                return false;
            }
        }

        private async Task<string?> GetSettingValueAsync(string key)
        {
            var setting = await _context.AppSettings.FindAsync(key);
            return setting?.Value;
        }

        private async Task UpdateSettingAsync(string key, string value)
        {
            var setting = await _context.AppSettings.FindAsync(key);
            
            if (setting != null)
            {
                setting.Value = value;
                setting.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                _context.AppSettings.Add(new Entities.AppSetting
                {
                    Key = key,
                    Value = value,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }
    }
}
