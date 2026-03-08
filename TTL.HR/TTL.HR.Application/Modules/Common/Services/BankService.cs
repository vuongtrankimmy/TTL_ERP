using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.Common.Constants;

namespace TTL.HR.Application.Modules.Common.Services
{
    public class BankService : IBankService
    {
        private readonly HttpClient _httpClient;

        public BankService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PagedResult<BankDto>> GetBanksAsync(GetBanksRequest request)
        {
            var queryString = $"?page={request.Page}&pageSize={request.PageSize}&searchTerm={request.SearchTerm}";
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<PagedResult<BankDto>>>($"{ApiEndpoints.System.Banks}{queryString}");
            return response?.Data ?? new PagedResult<BankDto>();
        }

        public async Task<BankDto?> GetBankByIdAsync(string id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<BankDto>>($"{ApiEndpoints.System.Banks}/{id}");
            return response?.Data;
        }

        public async Task<BankDto?> CreateBankAsync(CreateBankRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.System.Banks, request);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<BankDto>>();
                return apiResponse?.Data;
            }
            return null;
        }

        public async Task<bool> UpdateBankAsync(UpdateBankRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoints.System.Banks}/{request.Id}", request);
            return response.IsSuccessStatusCode;
        }

        public Task<bool> DeleteBankAsync(string id)
        {
            // Not implemented in API yet
            return Task.FromResult(false);
        }
    }
}
