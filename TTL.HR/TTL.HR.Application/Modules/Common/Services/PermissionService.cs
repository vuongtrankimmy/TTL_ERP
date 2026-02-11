using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.Common.Constants;

namespace TTL.HR.Application.Modules.Common.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly HttpClient _httpClient;

        public PermissionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<PermissionWithRolesDto>> GetPermissionsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<PermissionWithRolesDto>>>(ApiEndpoints.Administration.Permissions);
            return response?.Data ?? new List<PermissionWithRolesDto>();
        }

        public async Task<List<RoleModel>> GetRolesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<RoleModel>>>(ApiEndpoints.Administration.Roles);
            return response?.Data ?? new List<RoleModel>();
        }

        public async Task<RoleModel?> GetRoleByIdAsync(string id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<RoleModel>>($"{ApiEndpoints.Administration.Roles}/{id}");
            return response?.Data;
        }

        public async Task<bool> CreateRoleAsync(RoleModel role)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Administration.Roles, role);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateRoleAsync(string id, RoleModel role)
        {
            var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoints.Administration.Roles}/{id}", role);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteRoleAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiEndpoints.Administration.Roles}/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<PermissionDto>> GetAllAvailablePermissionsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<PermissionDto>>>(ApiEndpoints.Administration.PermissionsList);
            return response?.Data ?? new List<PermissionDto>();
        }

        public async Task<bool> AssignRoleAsync(string roleId, string employeeId)
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Administration.Roles}/assign", new { roleId, employeeId });
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UnassignRoleAsync(string roleId, string employeeId)
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Administration.Roles}/unassign", new { roleId, employeeId });
            return response.IsSuccessStatusCode;
        }
    }
}
