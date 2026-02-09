using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TTL.HR.Shared.Interfaces;

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
                return await _httpClient.GetFromJsonAsync<IEnumerable<T>>(endpoint) ?? new List<T>();
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
                return await _httpClient.GetFromJsonAsync<T>($"{endpoint}/{id}");
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
                return await response.Content.ReadFromJsonAsync<T>();
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
