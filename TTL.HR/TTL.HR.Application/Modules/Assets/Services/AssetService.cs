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
        public async Task<IEnumerable<AssetModel>> GetAssetsAsync(string? status = null, int pageSize = 100)
        {
            var url = $"{ApiEndpoints.Assets.Base}?Page=1&PageSize={pageSize}";
            if (!string.IsNullOrEmpty(status)) url += $"&Status={status}";
            
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<PagedResult<AssetModel>>>(url);
            return response?.Data?.Items ?? new List<AssetModel>();
        }

        public async Task<PagedResult<AssetAllocationDto>> GetAllocationsAsync(int page, int pageSize, string? status = null, string? searchTerm = null)
        {
            var url = $"{ApiEndpoints.Assets.Base}/allocations?Page={page}&PageSize={pageSize}";
            if (!string.IsNullOrEmpty(status)) url += $"&Status={status}";
            if (!string.IsNullOrEmpty(searchTerm)) url += $"&SearchTerm={searchTerm}";

            var response = await _httpClient.GetFromJsonAsync<ApiResponse<PagedResult<AssetAllocationDto>>>(url);
            return response?.Data ?? new PagedResult<AssetAllocationDto> { Items = new List<AssetAllocationDto>(), PageIndex = page, PageSize = pageSize };
        }
        public async Task<AssetModel?> GetAssetAsync(string id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<AssetModel>>($"{ApiEndpoints.Assets.Base}/{id}");
            return response?.Data;
        }
        public async Task<IEnumerable<AssetHistoryModel>> GetAssetHistoryAsync(string assetId)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<IEnumerable<AssetHistoryModel>>>($"{ApiEndpoints.Assets.Base}/{assetId}/history");
            return response?.Data ?? new List<AssetHistoryModel>();
        }

        public async Task<bool> AssignAssetAsync(string assetId, string employeeId, string condition, string note)
        {
            var command = new { AssetId = assetId, EmployeeId = employeeId, Condition = condition, Note = note, AllocatedDate = DateTime.Now };
            var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Assets.Base}/allocate", command);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ReturnAssetAsync(string assetId, string condition, string note)
        {
            var command = new { AssetId = assetId, Condition = condition, Note = note, ReturnedDate = DateTime.Now };
            var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Assets.Base}/revoke", command);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RequestMaintenanceAsync(string assetId, string issue, string priority)
        {
            var command = new { AssetId = assetId, IssueDescription = issue, Priority = priority, RequestedDate = DateTime.Now };
            var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Assets.Base}/maintenance", command);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CreateAssetAsync(AssetModel asset)
        {
            if (string.IsNullOrEmpty(asset.CategoryId)) asset.CategoryId = asset.Category;
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Assets.Base, asset);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAssetAsync(string id, AssetModel asset)
        {
            if (string.IsNullOrEmpty(asset.CategoryId)) asset.CategoryId = asset.Category;
            var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoints.Assets.Base}/{id}", asset);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAssetAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiEndpoints.Assets.Base}/{id}");
            return response.IsSuccessStatusCode;
        }
        public async Task<IEnumerable<AssetCategoryDto>> GetCategoriesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<IEnumerable<AssetCategoryDto>>>($"{ApiEndpoints.Assets.Base}/categories");
            return response?.Data ?? new List<AssetCategoryDto>();
        }
    }
}
