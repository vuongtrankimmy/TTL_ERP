using System.Net.Http.Json;
using TTL.HR.Application.Modules.Common.Constants;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Application.Modules.Common.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly HttpClient _httpClient;
        private SystemSettingsModel? _cachedSettings;

        public SettingsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public SystemSettingsModel? CachedSettings => _cachedSettings;

        public event Action? OnSettingsUpdated;

        public async Task InitializeAsync()

        {
            if (_cachedSettings == null)
            {
                await GetSettingsAsync();
            }
        }

        public async Task<SystemSettingsModel?> GetSettingsAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<SystemSettingsModel>>(ApiEndpoints.System.Settings);
                if (response?.Data != null)
                {
                    _cachedSettings = response.Data;
                }
                return response?.Data;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateSettingsAsync(SystemSettingsModel settings)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.System.Settings, settings);
                if (response.IsSuccessStatusCode)
                {
                    _cachedSettings = settings;
                    OnSettingsUpdated?.Invoke();
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<CodeGeneratorConfigDto>> GetCodeGeneratorConfigsAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<CodeGeneratorConfigDto>>>(ApiEndpoints.System.CodeGeneratorConfigs);
                return response?.Data ?? new List<CodeGeneratorConfigDto>();
            }
            catch
            {
                return new List<CodeGeneratorConfigDto>();
            }
        }

        public async Task<bool> UpdateCodeGeneratorConfigsAsync(List<CodeGeneratorConfigDto> configs)
        {
            try
            {
                var wrapper = new { Configs = configs };
                var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.System.CodeGeneratorConfigs, wrapper);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }

}
