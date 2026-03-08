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
            try
            {
                var response = await _httpClient.GetAsync(ApiEndpoints.Organization.Departments);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<DepartmentModel>>>();
                    return result?.Data ?? new List<DepartmentModel>();
                }
            }
            catch { }
            return new List<DepartmentModel>();
        }
        public async Task<DepartmentModel?> GetDepartmentAsync(string id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<DepartmentModel>>($"{ApiEndpoints.Organization.Departments}/{id}");
            return response?.Data;
        }
        public async Task<DepartmentDetailModel?> GetDepartmentDetailAsync(string id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<DepartmentDetailModel>>($"{ApiEndpoints.Organization.Departments}/{id}");
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
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AssignEmployeesAsync(string departmentId, List<string> employeeIds)
        {
            var payload = new { departmentId, employeeIds };
            var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Organization.Departments}/{departmentId}/assign", payload);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<OrgNode>> GetOrganizationStructureAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<OrganizationNode>>>(ApiEndpoints.Organization.Structure);
            if (response?.Data == null) return new List<OrgNode>();

            return MapToOrgNodes(response.Data);
        }

        private List<OrgNode> MapToOrgNodes(List<OrganizationNode> nodes)
        {
            var result = new List<OrgNode>();
            foreach (var node in nodes)
            {
                result.Add(MapToOrgNode(node));
            }
            return result;
        }

        private OrgNode MapToOrgNode(OrganizationNode node)
        {
            var orgNode = new OrgNode
            {
                Id = node.Id,
                Name = node.ManagerName,
                Role = node.Name, // Department Name
                Avatar = node.ManagerAvatar ?? "assets/media/avatars/blank.png",
                Type = "Department",
                IsManager = true,
                EmployeeCount = node.EmployeeCount,
                Children = new List<OrgNode>()
            };

            foreach (var child in node.Children)
            {
                orgNode.Children.Add(MapToOrgNode(child));
            }

            return orgNode;
        }
    }

    public class PositionService : IPositionService
    {
        private readonly HttpClient _httpClient;
        public PositionService(HttpClient httpClient) => _httpClient = httpClient;
        public async Task<List<PositionModel>> GetPositionsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(ApiEndpoints.Organization.Positions);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<PositionModel>>>();
                    return result?.Data ?? new List<PositionModel>();
                }
            }
            catch { }
            return new List<PositionModel>();
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

        public async Task<bool> AssignEmployeesAsync(string positionId, List<string> employeeIds, decimal? salary = null)
        {
            var payload = new { positionId, employeeIds, salary };
            var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Organization.Positions}/{positionId}/assign", payload);
            return response.IsSuccessStatusCode;
        }
    }
}
