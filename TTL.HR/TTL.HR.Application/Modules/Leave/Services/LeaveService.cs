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
        public async Task<IEnumerable<LeaveRequestModel>> GetLeaveRequestsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<LeaveRequestModel>>>(ApiEndpoints.Leave.Base);
            return response?.Data ?? new List<LeaveRequestModel>();
        }
        public async Task<bool> SubmitLeaveRequestAsync(LeaveRequestModel request)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Leave.Base, request);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> ApproveLeaveRequestAsync(string id, string status)
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Leave.Base}/{id}/approve", new { Status = status });
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ProcessLeaveRequestAsync(string id, bool approved, string? note)
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Leave.Base}/{id}/process", new { Approved = approved, Note = note });
            return response.IsSuccessStatusCode;
        }
    }
}
