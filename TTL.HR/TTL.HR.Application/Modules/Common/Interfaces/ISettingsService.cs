using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Application.Modules.Common.Interfaces
{
    public interface ISettingsService
    {
        Task<SystemSettingsModel?> GetSettingsAsync();
        Task<bool> UpdateSettingsAsync(SystemSettingsModel settings);
        SystemSettingsModel? CachedSettings { get; }
        Task InitializeAsync();
        event Action? OnSettingsUpdated;
    }


}
