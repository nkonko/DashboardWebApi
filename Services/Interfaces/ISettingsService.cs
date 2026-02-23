using WebApi.Models;

namespace WebApi.Services.Interfaces
{
    public interface ISettingsService
    {
        Task<SettingsDto> GetSettingsAsync();
        Task<bool> UpdateSettingsAsync(SettingsUpdateDto dto);
    }
}
