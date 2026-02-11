using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Assets.Interfaces;
using TTL.HR.Application.Modules.Assets.Models;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.Common.Constants;

namespace TTL.HR.Application.Modules.Assets.Services
{
    public class AssetService : IAssetService
    {
        private readonly HttpClient _httpClient;
        public AssetService(HttpClient httpClient) => _httpClient = httpClient;
        public async Task<IEnumerable<AssetModel>> GetAssetsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<AssetModel>>>(ApiEndpoints.Assets.Base);
            return response?.Data ?? new List<AssetModel>();
        }
        public async Task<AssetModel?> GetAssetAsync(string id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<AssetModel>>($"{ApiEndpoints.Assets.Base}/{id}");
            return response?.Data;
        }
        public async Task<bool> AssignAssetAsync(string assetId, string employeeId)
        {
            var response = await _httpClient.PostAsync($"{ApiEndpoints.Assets.Base}/{assetId}/assign/{employeeId}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ReturnAssetAsync(string assetId, string condition, string note)
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Assets.Base}/{assetId}/return", new { Condition = condition, Note = note });
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CreateAssetAsync(AssetModel asset)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Assets.Base, asset);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAssetAsync(string id, AssetModel asset)
        {
            var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoints.Assets.Base}/{id}", asset);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAssetAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiEndpoints.Assets.Base}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
