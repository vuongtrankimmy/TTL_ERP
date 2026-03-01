using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Leave.Interfaces;
using TTL.HR.Application.Modules.Leave.Models;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.Common.Constants;

namespace TTL.HR.Application.Modules.Leave.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly HttpClient _httpClient;
        public LeaveService(HttpClient httpClient) => _httpClient = httpClient;
        public async Task<PagedResult<LeaveRequestModel>> GetLeaveRequestsAsync(int page = 1, int pageSize = 10, string? status = null, string? searchTerm = null)
        {
            var url = $"{ApiEndpoints.Leave.Base}?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(status)) url += $"&status={status}";
            if (!string.IsNullOrEmpty(searchTerm)) url += $"&searchTerm={searchTerm}";

            var response = await _httpClient.GetFromJsonAsync<ApiResponse<PagedResult<LeaveRequestModel>>>(url);
            return response?.Data ?? new PagedResult<LeaveRequestModel>();
        }

        public async Task<LeaveStateSummaryModel> GetLeaveSummaryAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<LeaveStateSummaryModel>>($"{ApiEndpoints.Leave.Base}/summary");
            return response?.Data ?? new LeaveStateSummaryModel();
        }

        public async Task<LeaveBalanceModel?> GetLeaveBalanceAsync(string employeeId, int year)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<LeaveBalanceModel>>($"{ApiEndpoints.Leave.Base}/balance/{employeeId}?year={year}");
            return response?.Data;
        }

        public async Task<bool> SubmitLeaveRequestAsync(LeaveRequestModel request)
        {
            // Map to Backend Command structure if needed, or send directly if structure matches
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Leave.Base, request);
            return response.IsSuccessStatusCode;
        }


        public async Task<bool> ProcessLeaveRequestAsync(string id, bool approved, string? note)
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Leave.Base}/{id}/process", new { Approved = approved, Note = note });
            return response.IsSuccessStatusCode;
        }
    }
}
