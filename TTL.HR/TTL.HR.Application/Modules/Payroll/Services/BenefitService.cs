using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Common.Constants;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.Payroll.Interfaces;
using TTL.HR.Application.Modules.Payroll.Models;

namespace TTL.HR.Application.Modules.Payroll.Services
{
    public class BenefitService : IBenefitService
    {
        private readonly HttpClient _httpClient;

        public BenefitService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<BenefitModel>> GetBenefitsAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<BenefitModel>>>(ApiEndpoints.Benefits.Base);
                return response?.Data ?? new List<BenefitModel>();
            }
            catch
            {
                return new List<BenefitModel>();
            }
        }

        public async Task<BenefitModel?> GetBenefitByIdAsync(string id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<BenefitModel>>($"{ApiEndpoints.Benefits.Base}/{id}");
            return response?.Data;
        }

        public async Task<bool> CreateBenefitAsync(BenefitModel benefit)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Benefits.Base, benefit);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateBenefitAsync(string id, BenefitModel benefit)
        {
            var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoints.Benefits.Base}/{id}", benefit);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteBenefitAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiEndpoints.Benefits.Base}/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AssignBenefitAsync(BenefitAssignRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Benefits.Base}/assign", new
                {
                    EmployeeId = request.EmployeeId,
                    BenefitId = request.BenefitId,
                    OverrideAmount = request.OverrideAmount,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Note = request.Note
                });
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<IEnumerable<EmployeeBenefitModel>> GetEmployeeBenefitsAsync(string employeeId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<EmployeeBenefitModel>>>($"{ApiEndpoints.Benefits.Base}/employee/{employeeId}");
                return response?.Data ?? new List<EmployeeBenefitModel>();
            }
            catch { return new List<EmployeeBenefitModel>(); }
        }

        public async Task<IEnumerable<EmployeeBenefitModel>> GetBenefitAllocationsAsync(string benefitId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<EmployeeBenefitModel>>>($"{ApiEndpoints.Benefits.Base}/{benefitId}/allocations");
                return response?.Data ?? new List<EmployeeBenefitModel>();
            }
            catch { return new List<EmployeeBenefitModel>(); }
        }

        public async Task<bool> RemoveEmployeeBenefitAsync(string allocationId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiEndpoints.Benefits.Base}/allocation/{allocationId}");
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<bool> BatchAssignBenefitsAsync(BatchAssignRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Benefits.Base}/batch-assign", request);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<byte[]?> ExportBenefitsAsync(string? searchTerm = null, string? category = null)
        {
            try
            {
                var url = $"{ApiEndpoints.Benefits.Base}/export?1=1";
                if (!string.IsNullOrEmpty(searchTerm)) url += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";
                if (!string.IsNullOrEmpty(category)) url += $"&category={category}";

                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ExportBenefits] Exception: {ex.Message}");
                return null;
            }
        }
    }
}
