using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TTL.HR.Shared.Interfaces;
using TTL.HR.Shared.Models;
using System.Linq;

namespace TTL.HR.Shared.Services.Client
{
    public class MasterDataService : IMasterDataService
    {
        private readonly HttpClient _httpClient;
        private static List<LookupModel>? _cachedLookups;

        public MasterDataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<LookupModel>> GetLookupsAsync(string? type = null)
        {
            var url = "api/v1/lookups";
            if (!string.IsNullOrEmpty(type))
            {
                url += $"?type={type}";
            }

            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<LookupModel>>>(url);
                return response?.Data ?? new List<LookupModel>();
            }
            catch
            {
                return new List<LookupModel>();
            }
        }

        public async Task<List<LookupModel>> GetCachedLookupsAsync(string type)
        {
            if (_cachedLookups == null)
            {
                _cachedLookups = await GetLookupsAsync();
            }

            return _cachedLookups
                .Where(l => l.Type == type)
                .OrderBy(l => l.Order)
                .ToList();
        }

        public Task ClearCacheAsync()
        {
            _cachedLookups = null;
            return Task.CompletedTask;
        }
    }
}
