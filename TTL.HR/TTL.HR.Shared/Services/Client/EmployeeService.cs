using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TTL.HR.Shared.Entities.HumanResource;
using TTL.HR.Shared.Interfaces;

namespace TTL.HR.Shared.Services.Client
{
    public class EmployeeService : IEmployeeService
    {
        private readonly HttpClient _httpClient;

        public EmployeeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Employee>>("api/employee");
        }

        public async Task<Employee> GetEmployeeAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<Employee>($"api/employee/{id}");
        }

        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            var response = await _httpClient.PostAsJsonAsync("api/employee", employee);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Employee>();
        }

        public async Task UpdateEmployeeAsync(string id, Employee employee)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/employee/{id}", employee);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteEmployeeAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"api/employee/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
