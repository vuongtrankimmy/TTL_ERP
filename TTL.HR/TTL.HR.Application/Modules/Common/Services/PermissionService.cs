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
            try
            {
                var response = await _httpClient.GetAsync(ApiEndpoints.Administration.Permissions);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<PermissionWithRolesDto>>>();
                    return result?.Data ?? new List<PermissionWithRolesDto>();
                }
            }
            catch { }
            return new List<PermissionWithRolesDto>();
        }

        public async Task<List<RoleModel>> GetRolesAsync(string? searchTerm = null)
        {
            try
            {
                var url = ApiEndpoints.Administration.Roles;
                if (!string.IsNullOrEmpty(searchTerm)) url += $"?SearchTerm={Uri.EscapeDataString(searchTerm)}";
                
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<RoleModel>>>();
                    return result?.Data?.Items ?? new List<RoleModel>();
                }
            }
            catch { }
            return new List<RoleModel>();
        }

        public async Task<RoleModel?> GetRoleByIdAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoints.Administration.Roles}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<RoleModel>>();
                    return result?.Data;
                }
            }
            catch { }
            return null;
        }

        public async Task<(bool Success, string Message)> CreateRoleAsync(RoleModel role)
        {
            var command = new {
                Name = role.Name,
                Description = role.Description,
                Permissions = role.Permissions
            };
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Administration.Roles, command);
            return await ParseResponseAsync(response, "Tạo vai trò thất bại");
        }

        public async Task<(bool Success, string Message)> UpdateRoleAsync(string id, RoleModel role)
        {
            var command = new {
                Id = id, // Ensure ID is sent
                Name = role.Name,
                Description = role.Description,
                Permissions = role.Permissions
            };
            var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoints.Administration.Roles}/{id}", command);
            return await ParseResponseAsync(response, "Cập nhật vai trò thất bại");
        }

        private async Task<(bool Success, string Message)> ParseResponseAsync(HttpResponseMessage response, string defaultError)
        {
            try
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
                if (result != null)
                {
                    if (result.Success) return (true, result.Message);
                    // Combine Message and Errors if available
                    var msg = result.Message;
                    if (result.Errors != null && result.Errors.Count > 0)
                        msg += ": " + string.Join(", ", result.Errors);
                    return (false, !string.IsNullOrEmpty(msg) ? msg : defaultError);
                }
            }
            catch {}
            return (response.IsSuccessStatusCode, response.IsSuccessStatusCode ? "Thành công" : defaultError);
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
        
        public async Task<bool> AssignRolesAsync(string roleId, List<string> employeeIds)
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Administration.Roles}/assign", new { roleId, employeeIds });
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UnassignRoleAsync(string roleId, string employeeId)
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Administration.Roles}/unassign", new { roleId, employeeId });
            return response.IsSuccessStatusCode;
        }
    }
}
