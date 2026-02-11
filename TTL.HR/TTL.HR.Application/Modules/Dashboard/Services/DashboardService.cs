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
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<DashboardOverviewModel>>(ApiEndpoints.Dashboard.Base);
            return response?.Data ?? new DashboardOverviewModel();
        }
    }
}
