using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Application.Modules.Common.Interfaces
{
    public interface ISettingsService
    {
        Task<SystemSettingsModel?> GetSettingsAsync();
        Task<bool> UpdateSettingsAsync(SystemSettingsModel settings);
        
        Task<List<CodeGeneratorConfigDto>> GetCodeGeneratorConfigsAsync();
        Task<bool> UpdateCodeGeneratorConfigsAsync(List<CodeGeneratorConfigDto> configs);
        
        SystemSettingsModel? CachedSettings { get; }
        Task InitializeAsync();
        event Action? OnSettingsUpdated;
    }


}
