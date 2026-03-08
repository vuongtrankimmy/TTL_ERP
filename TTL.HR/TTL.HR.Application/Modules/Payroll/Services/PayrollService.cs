using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Payroll.Interfaces;
using TTL.HR.Application.Modules.Payroll.Models;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.Common.Constants;
using System.Text.Json;
using System.Linq;

namespace TTL.HR.Application.Modules.Payroll.Services
{
    public class PayrollService : IPayrollService
    {
        private readonly HttpClient _httpClient;
        public PayrollService(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<IEnumerable<PayrollModel>> GetPayrollAsync(int? month = null, int? year = null)
        {
            var url = ApiEndpoints.Payroll.Base;
            if (month.HasValue && year.HasValue) url += $"?month={month}&year={year}";
            
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<PayrollModel>>>(url, options);
            return response?.Data ?? new List<PayrollModel>();
        }

        public Task<IEnumerable<PayrollModel>> GetPayrollsAsync(int? month = null, int? year = null) => GetPayrollAsync(month, year);

        public async Task<IEnumerable<PayrollPeriodModel>> GetPeriodsAsync(int? year = null, int? month = null)
        {
            var url = ApiEndpoints.Payroll.Periods;
            var queryParams = new List<string>();
            if (year.HasValue) queryParams.Add($"year={year}");
            if (month.HasValue) queryParams.Add($"month={month}");
            
            if (queryParams.Any()) url += "?" + string.Join("&", queryParams);
            
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<PayrollPeriodModel>>>(url, options);
            return response?.Data ?? new List<PayrollPeriodModel>();
        }

        public async Task<PayrollPeriodDetailModel?> GetPeriodDetailAsync(string id, string? searchTerm = null, string? departmentId = null, int page = 1, int pageSize = 10, int? year = null, int? month = null)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(searchTerm)) queryParams.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");
            if (!string.IsNullOrWhiteSpace(departmentId) && departmentId != "all") queryParams.Add($"departmentId={Uri.EscapeDataString(departmentId)}");
            queryParams.Add($"page={page}");
            queryParams.Add($"pageSize={pageSize}");

            var queryString = string.Join("&", queryParams);
            string url;
            
            if (!string.IsNullOrEmpty(id))
            {
                url = $"{ApiEndpoints.Payroll.Periods}/{id}/detail?{queryString}";
            }
            else
            {
                url = $"{ApiEndpoints.Payroll.Periods}/{year}/{month}/detail?{queryString}";
            }
            
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var responseStr = await _httpClient.GetStringAsync(url);
            Console.WriteLine($"[PayrollService] Raw JSON Response: {(responseStr.Length > 500 ? responseStr.Substring(0, 500) + "..." : responseStr)}");
            
            var response = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<PayrollPeriodDetailModel>>(responseStr, options);
            return response?.Data;
        }

        public async Task<bool> GeneratePayrollAsync(int month, int year)
        {
            var response = await _httpClient.PostAsync($"{ApiEndpoints.Payroll.Periods}/generate?month={month}&year={year}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdatePayrollAsync(string id, PayrollModel payroll)
        {
            var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoints.Payroll.Base}/{id}", payroll);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> LockPayrollAsync(int month, int year)
        {
            var response = await _httpClient.PostAsync($"{ApiEndpoints.Payroll.Periods}/lock?month={month}&year={year}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> LockPayrollAsync(string id)
        {
            var response = await _httpClient.PostAsync($"{ApiEndpoints.Payroll.Periods}/lock?id={id}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeletePeriodAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiEndpoints.Payroll.Periods}/{id}");
            return response.IsSuccessStatusCode;
        }
        
        public async Task<PagedResult<PayrollModel>> GetMyPayrollsAsync(string? employeeId = null, int? year = null, int page = 1, int pageSize = 10)
        {
            var url = $"{ApiEndpoints.Payroll.Me}?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(employeeId)) url += $"&employeeId={employeeId}";
            if (year.HasValue) url += $"&year={year}";
            
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<PagedResult<PayrollModel>>>(url, options);
            return response?.Data ?? new PagedResult<PayrollModel>();
        }
    }
}
