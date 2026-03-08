using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Dashboard.Interfaces;
using TTL.HR.Application.Modules.Dashboard.Models;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.Common.Constants;

namespace TTL.HR.Application.Modules.Dashboard.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly HttpClient _httpClient;
        public DashboardService(HttpClient httpClient) => _httpClient = httpClient;
        public async Task<DashboardOverviewModel> GetOverviewAsync()
        {
            var response = await _httpClient.GetAsync(ApiEndpoints.Dashboard.Base);
            if (!response.IsSuccessStatusCode)
            {
                string errorMessage;
                try 
                {
                    var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
                    errorMessage = errorResult?.Message ?? $"API Error ({(int)response.StatusCode}: {response.ReasonPhrase})";
                }
                catch
                {
                    errorMessage = $"Lỗi kết nối máy chủ ({(int)response.StatusCode}: {response.ReasonPhrase})";
                }
                throw new System.Exception(errorMessage);
            }
            
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<DashboardOverviewModel>>();
            return result?.Data ?? new DashboardOverviewModel();
        }
    }
}
