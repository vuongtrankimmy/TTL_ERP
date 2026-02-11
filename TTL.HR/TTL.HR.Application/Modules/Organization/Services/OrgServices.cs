using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Organization.Interfaces;
using TTL.HR.Application.Modules.Organization.Models;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.Common.Constants;

namespace TTL.HR.Application.Modules.Organization.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly HttpClient _httpClient;
        public DepartmentService(HttpClient httpClient) => _httpClient = httpClient;
        public async Task<List<DepartmentModel>> GetDepartmentsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<DepartmentModel>>>(ApiEndpoints.Organization.Departments);
            return response?.Data ?? new List<DepartmentModel>();
        }
        public async Task<DepartmentModel?> GetDepartmentAsync(string id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<DepartmentModel>>($"{ApiEndpoints.Organization.Departments}/{id}");
            return response?.Data;
        }

        public async Task<DepartmentModel?> CreateDepartmentAsync(CreateDepartmentRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Organization.Departments, request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<DepartmentModel>>();
                return result?.Data;
            }
            return null;
        }

        public async Task<DepartmentModel?> UpdateDepartmentAsync(string id, UpdateDepartmentRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoints.Organization.Departments}/{id}", request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<DepartmentModel>>();
                return result?.Data;
            }
            return null;
        }

        public async Task<bool> DeleteDepartmentAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiEndpoints.Organization.Departments}/{id}");
            return response.IsSuccessStatusCode; // Should probably check for API response success flag too if needed
        }
    }

    public class PositionService : IPositionService
    {
        private readonly HttpClient _httpClient;
        public PositionService(HttpClient httpClient) => _httpClient = httpClient;
        public async Task<List<PositionModel>> GetPositionsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<PositionModel>>>(ApiEndpoints.Organization.Positions);
            return response?.Data ?? new List<PositionModel>();
        }
        public async Task<PositionModel?> GetPositionAsync(string id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<PositionModel>>($"{ApiEndpoints.Organization.Positions}/{id}");
            return response?.Data;
        }
        public async Task<PositionModel?> CreatePositionAsync(CreatePositionRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Organization.Positions, request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<PositionModel>>();
                return result?.Data;
            }
            return null;
        }

        public async Task<PositionModel?> UpdatePositionAsync(string id, UpdatePositionRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoints.Organization.Positions}/{id}", request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<PositionModel>>();
                return result?.Data;
            }
            return null;
        }

        public async Task<bool> DeletePositionAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiEndpoints.Organization.Positions}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
