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
    }
}
