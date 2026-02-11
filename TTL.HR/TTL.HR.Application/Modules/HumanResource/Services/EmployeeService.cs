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
        public async Task<List<EmployeeModel>> GetEmployeesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<EmployeeModel>>>(ApiEndpoints.Employees.Base);
            return response?.Data ?? new List<EmployeeModel>();
        }
        public async Task<EmployeeModel?> GetEmployeeAsync(string id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<EmployeeModel>>($"{ApiEndpoints.Employees.Base}/{id}");
            return response?.Data;
        }
        public async Task<Employee?> CreateEmployeeAsync(Employee employee)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Employees.Base, employee);
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<Employee>>();
            return apiResponse?.Data;
        }
        public async Task<Employee?> UpdateEmployeeAsync(string id, Employee employee)
        {
            var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoints.Employees.Base}/{id}", employee);
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<Employee>>();
            return apiResponse?.Data;
        }
        public async Task<bool> DeleteEmployeeAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiEndpoints.Employees.Base}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
