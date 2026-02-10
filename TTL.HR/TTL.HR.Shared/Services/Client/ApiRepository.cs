using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TTL.HR.Shared.Interfaces;
using TTL.HR.Shared.Models;

namespace TTL.HR.Shared.Services.Client
{
    public class ApiRepository<T> : IApiRepository<T> where T : class
    {
        private readonly HttpClient _httpClient;

        public ApiRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<T>> GetAllAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<T>>>(endpoint);
                return response?.Data ?? new List<T>();
            }
            catch
            {
                return new List<T>();
            }
        }

        public async Task<T?> GetByIdAsync(string endpoint, string id)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<T>>($"{endpoint}/{id}");
                return response != null ? response.Data : null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<T?> CreateAsync(string endpoint, T entity)
        {
            var response = await _httpClient.PostAsJsonAsync(endpoint, entity);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
                return apiResponse != null ? apiResponse.Data : null;
            }
            return null;
        }

        public async Task<bool> UpdateAsync(string endpoint, string id, T entity)
        {
            var response = await _httpClient.PutAsJsonAsync($"{endpoint}/{id}", entity);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(string endpoint, string id)
        {
            var response = await _httpClient.DeleteAsync($"{endpoint}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
