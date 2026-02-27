using WebApi.Core.Models;

namespace WebApi.Infrastructure.Services.Interfaces
{
    public interface ISettingsService
    {
        Task<SettingsDto> GetSettingsAsync();
        Task<bool> UpdateSettingsAsync(SettingsUpdateDto dto);
    }
}
