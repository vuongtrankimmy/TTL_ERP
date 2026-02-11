using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Payroll.Interfaces;
using TTL.HR.Application.Modules.Payroll.Models;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.Common.Constants;

namespace TTL.HR.Application.Modules.Payroll.Services
{
    public class PayrollService : IPayrollService
    {
        private readonly HttpClient _httpClient;
        public PayrollService(HttpClient httpClient) => _httpClient = httpClient;
        public async Task<IEnumerable<PayrollModel>> GetPayrollAsync(int? month = null, int? year = null)
        {
            var url = ApiEndpoints.Payroll.Periods;
            if (month.HasValue && year.HasValue) url += $"?month={month}&year={year}";
            
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<PayrollModel>>>(url);
            return response?.Data ?? new List<PayrollModel>();
        }
        public Task<IEnumerable<PayrollModel>> GetPayrollsAsync(int? month = null, int? year = null) => GetPayrollAsync(month, year);
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
    }
}
