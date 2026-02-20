using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.HumanResource.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Models;
using TTL.HR.Application.Modules.HumanResource.Entities;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.Common.Constants;

namespace TTL.HR.Application.Modules.HumanResource.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly HttpClient _httpClient;
        public EmployeeService(HttpClient httpClient) => _httpClient = httpClient;
        public async Task<List<EmployeeDto>> GetEmployeesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<PagedResult<EmployeeDto>>>(ApiEndpoints.Employees.Base);
            return response?.Data?.Items ?? new List<EmployeeDto>();
        }

        public async Task<PagedResult<EmployeeDto>> GetEmployeesPaginatedAsync(int pageIndex, int pageSize, string? searchTerm = null, string? departmentId = null, string? status = null)
        {
            var url = $"{ApiEndpoints.Employees.Base}?pageIndex={pageIndex}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(searchTerm)) url += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";
            if (!string.IsNullOrEmpty(departmentId)) url += $"&departmentId={departmentId}";
            if (!string.IsNullOrEmpty(status)) url += $"&status={status}";

            var response = await _httpClient.GetFromJsonAsync<ApiResponse<PagedResult<EmployeeDto>>>(url);
            return response?.Data ?? new PagedResult<EmployeeDto>();
        }
        public async Task<EmployeeModel?> GetEmployeeAsync(string id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<EmployeeModel>>($"{ApiEndpoints.Employees.Base}/{id}");
            return response?.Data;
        }

        public async Task<EmployeeModel?> GetMyEmployeeAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<EmployeeModel>>(ApiEndpoints.Employees.Me);
                return response?.Data;
            }
            catch
            {
                return null;
            }
        }

        public async Task<string?> CreateEmployeeAsync(Employee employee)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Employees.Base, employee);
                if (response.IsSuccessStatusCode)
                {
                    try 
                    {
                        var successResult = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
                        return successResult?.Data;
                    }
                    catch 
                    {
                        // If parsing fails but status is success, maybe it returned a plain string or something else
                        var content = await response.Content.ReadAsStringAsync();
                        return content; // Try returning raw content if it looks like an ID
                    }
                }

                try
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
                    if (result != null && !string.IsNullOrEmpty(result.Message))
                    {
                        var errorMsg = result.Message;
                        if (result.Errors != null && result.Errors.Any())
                        {
                            errorMsg += ": " + string.Join(", ", result.Errors);
                        }
                        return errorMsg; 
                    }
                }
                catch
                {
                    // Ignore JSON parse error on failure, fall back to string
                }
                
                var rawError = await response.Content.ReadAsStringAsync();
                return string.IsNullOrEmpty(rawError) ? $"Error {response.StatusCode}" : rawError;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public async Task<string?> UpdateEmployeeAsync(string id, UpdateEmployeeRequest request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoints.Employees.Base}/{id}", request);
                if (response.IsSuccessStatusCode) return null;

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
                if (result != null && !string.IsNullOrEmpty(result.Message))
                {
                    var errorMsg = result.Message;
                    if (result.Errors != null && result.Errors.Any())
                    {
                        errorMsg += ": " + string.Join(", ", result.Errors);
                    }
                    return errorMsg;
                }
                
                var rawError = await response.Content.ReadAsStringAsync();
                return string.IsNullOrEmpty(rawError) ? "Unknown error occurred" : rawError;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public async Task<bool> DeleteEmployeeAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiEndpoints.Employees.Base}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
