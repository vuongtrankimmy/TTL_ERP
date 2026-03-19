using System.Collections.Generic;
using System.Linq;
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
            var response = await _httpClient.GetAsync(ApiEndpoints.Organization.Departments);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Không thể tải danh sách phòng ban: {(int)response.StatusCode}. {errorBody}");
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<DepartmentModel>>>();
            return result?.Data ?? new List<DepartmentModel>();
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
            
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new System.Exception($"Lỗi API ({response.StatusCode}): {errorBody}");
        }

        public async Task<DepartmentModel?> UpdateDepartmentAsync(string id, UpdateDepartmentRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoints.Organization.Departments}/{id}", request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<DepartmentModel>>();
                return result?.Data;
            }
            
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new System.Exception($"Lỗi API ({response.StatusCode}): {errorBody}");
        }

        public async Task<bool> DeleteDepartmentAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiEndpoints.Organization.Departments}/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new System.Exception($"Lỗi API ({response.StatusCode}): {errorBody}");
            }
            return true;
        }

        public async Task<bool> AssignEmployeesAsync(string departmentId, List<string> employeeIds)
        {
            var payload = new { departmentId, employeeIds };
            var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Organization.Departments}/{departmentId}/assign", payload);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new System.Exception($"Lỗi API ({response.StatusCode}): {errorBody}");
            }
            return true;
        }

        public async Task<List<OrgNode>> GetOrganizationStructureAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<OrganizationNode>>>(ApiEndpoints.Organization.Structure);
            var nodes = response?.Data ?? new List<OrganizationNode>();
            
            System.Console.WriteLine($"[OrgService] Loaded {nodes.Count} org nodes from API");
            
            var result = nodes.Select(n => MapToOrgNode(n)).ToList();
            return result;
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
                Type = node.Type,
                Name = node.Type == "Department" ? node.ManagerName : node.Name,
                Role = node.Type == "Department" ? node.Name : "Chức danh / Vị trí",
                Avatar = node.ManagerAvatar ?? (node.Type == "Department" ? "assets/media/avatars/blank.png" : "assets/media/misc/group.png"),
                IsManager = node.Type == "Department" || node.Type == "Employee",
                EmployeeCount = node.EmployeeCount,
                SortOrder = node.SortOrder,
                Children = new List<OrgNode>()
            };

            if (node.Type == "Employee")
            {
                orgNode.Name = node.Name;
                orgNode.Role = node.Code; // Or job role
                orgNode.EmployeeCode = node.Code;
            }

            var sortedChildren = node.Children?.OrderBy(n => n.SortOrder).ToList() ?? new List<OrganizationNode>();
            foreach (var child in sortedChildren)
            {
                orgNode.Children.Add(MapToOrgNode(child));
            }

            return orgNode;
        }

        public async Task<List<string>> GetManagedDepartmentIdsAsync(string managerEmployeeId)
        {
            if (string.IsNullOrEmpty(managerEmployeeId)) return new List<string>();

            // Collect all raw nodes from the structure API to handle ManagerId lookups
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<OrganizationNode>>>(ApiEndpoints.Organization.Structure);
            var rawNodes = response?.Data ?? new List<OrganizationNode>();
            var managedIds = new HashSet<string>();

            void Traverse(List<OrganizationNode> nodes, bool parentIsManaged)
            {
                foreach (var node in nodes)
                {
                    bool isManaged = parentIsManaged || (node.ManagerId == managerEmployeeId);
                    if (isManaged && node.Type == "Department")
                    {
                        managedIds.Add(node.Id);
                    }
                    if (node.Children != null && node.Children.Any())
                    {
                        Traverse(node.Children, isManaged);
                    }
                }
            }

            Traverse(rawNodes, false);
            return managedIds.ToList();
        }
    }

    public class PositionService : IPositionService
    {
        private readonly HttpClient _httpClient;
        public PositionService(HttpClient httpClient) => _httpClient = httpClient;
        public async Task<List<PositionModel>> GetPositionsAsync()
        {
            var response = await _httpClient.GetAsync(ApiEndpoints.Organization.Positions);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Không thể tải danh sách chức danh: {(int)response.StatusCode}. {errorBody}");
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<PositionModel>>>();
            return result?.Data ?? new List<PositionModel>();
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
            
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new System.Exception($"Lỗi API ({response.StatusCode}): {errorBody}");
        }

        public async Task<PositionModel?> UpdatePositionAsync(string id, UpdatePositionRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoints.Organization.Positions}/{id}", request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<PositionModel>>();
                return result?.Data;
            }
            
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new System.Exception($"Lỗi API ({response.StatusCode}): {errorBody}");
        }

        public async Task<bool> DeletePositionAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiEndpoints.Organization.Positions}/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new System.Exception($"Lỗi API ({response.StatusCode}): {errorBody}");
            }
            return true;
        }

        public async Task<bool> AssignEmployeesAsync(string positionId, List<string> employeeIds, decimal? salary = null)
        {
            var payload = new { positionId, employeeIds, salary };
            var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Organization.Positions}/{positionId}/assign", payload);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new System.Exception($"Lỗi API ({response.StatusCode}): {errorBody}");
            }
            return true;
        }
    }
}
