using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Common.Constants;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Application.Modules.Common.Services
{
    public class NotificationService : INotificationService
    {
        private readonly HttpClient _httpClient;

        public NotificationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<NotificationModel>> GetMyNotificationsAsync(int limit = 20)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<NotificationModel>>>($"{ApiEndpoints.Notifications.My}?limit={limit}");
                return response?.Data ?? new List<NotificationModel>();
            }
            catch
            {
                return new List<NotificationModel>();
            }
        }

        public async Task<bool> MarkAsReadAsync(string id)
        {
            try
            {
                var response = await _httpClient.PostAsync(ApiEndpoints.Notifications.MarkAsRead(id), null);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> MarkAllAsReadAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync(ApiEndpoints.Notifications.MarkAllAsRead, null);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
