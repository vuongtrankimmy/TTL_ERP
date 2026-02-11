using System.Net.Http.Json;
using TTL.HR.Application.Modules.Common.Constants;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Application.Modules.Common.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly HttpClient _httpClient;

        public SettingsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SystemSettingsModel?> GetSettingsAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<SystemSettingsModel>>(ApiEndpoints.System.Settings);
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
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
