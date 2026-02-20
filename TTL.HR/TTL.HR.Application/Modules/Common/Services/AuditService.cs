using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Common.Constants;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Application.Modules.Common.Services
{
    public class AuditService : IAuditService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;

        public AuditService(HttpClient httpClient, IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        public async Task<List<AuditLogModel>> GetMyAuditLogsAsync()
        {
            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null) return new List<AuditLogModel>();

            var pagedResult = await GetPagedAuditLogsAsync(1, 100, userId: currentUser.Id);
            return pagedResult.Items;
        }

        public async Task<PagedResult<AuditLogModel>> GetPagedAuditLogsAsync(int page, int pageSize, string? userId = null, string? action = null, string? entityName = null)
        {
            try
            {
                var queryParams = new List<string>
                {
                    $"page={page}",
                    $"pageSize={pageSize}"
                };

                if (!string.IsNullOrEmpty(userId)) queryParams.Add($"userId={userId}");
                if (!string.IsNullOrEmpty(action)) queryParams.Add($"action={action}");
                if (!string.IsNullOrEmpty(entityName)) queryParams.Add($"entityName={entityName}");

                var url = $"{ApiEndpoints.System.Audit}?{string.Join("&", queryParams)}";
                
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<PagedResult<AuditLogModel>>>(url);
                if (response != null && response.Success && response.Data != null)
                {
                    return response.Data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching audit logs: {ex.Message}");
            }

            return new PagedResult<AuditLogModel>();
        }
    }
}
