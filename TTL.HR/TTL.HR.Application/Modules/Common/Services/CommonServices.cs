using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.Common.Constants;

namespace TTL.HR.Application.Modules.Common.Services
{
    public class ApiRepository<T> : IApiRepository<T> where T : class
    {
        private readonly HttpClient _httpClient;
        public ApiRepository(HttpClient httpClient) => _httpClient = httpClient;
        public async Task<List<T>> GetAllAsync(string endpoint)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<T>>>(endpoint);
            return response?.Data ?? new List<T>();
        }
        public async Task<T?> GetByIdAsync(string endpoint, string id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<T>>($"{endpoint}/{id}");
            return response?.Data;
        }
        public async Task<T> CreateAsync(string endpoint, T entity)
        {
            var response = await _httpClient.PostAsJsonAsync(endpoint, entity);
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
            return apiResponse?.Data!;
        }
        public async Task UpdateAsync(string endpoint, string id, T entity) => await _httpClient.PutAsJsonAsync($"{endpoint}/{id}", entity);
        public async Task DeleteAsync(string endpoint, string id) => await _httpClient.DeleteAsync($"{endpoint}/{id}");
    }

    public class MasterDataService : IMasterDataService
    {
        private readonly HttpClient _httpClient;
        private static readonly Dictionary<string, List<LookupModel>> _cache = new();
        public MasterDataService(HttpClient httpClient) => _httpClient = httpClient;
        public async Task<List<LookupModel>> GetLookupsAsync(string type)
        {
            var url = $"{ApiEndpoints.Lookups.Base}?type={type}";
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<LookupModel>>>(url);
            return response?.Data ?? new List<LookupModel>();
        }
        public async Task<List<LookupModel>> GetCachedLookupsAsync(string type)
        {
            if (!_cache.ContainsKey(type)) _cache[type] = await GetLookupsAsync(type);
            return _cache[type];
        }
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        public AuthService(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Auth.Login, request);
                ApiResponse<AuthResponse>? apiResponse = null;

                try
                {
                    apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();
                }
                catch (System.Text.Json.JsonException)
                {
                    return new ApiResponse<AuthResponse> { Success = false, Message = $"Lỗi máy chủ: {response.StatusCode} ({response.ReasonPhrase})" };
                }
                
                if (response.IsSuccessStatusCode && apiResponse?.Success == true)
                {
                    if (apiResponse.Data != null)
                    {
                        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiResponse.Data.AccessToken);
                    }
                    return apiResponse;
                }
                
                return apiResponse ?? new ApiResponse<AuthResponse> { Success = false, Message = "Đăng nhập thất bại" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<AuthResponse> { Success = false, Message = $"Lỗi kết nối: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<bool>> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Auth.Register, request);
                ApiResponse<bool>? apiResponse = null;
                
                try
                {
                    apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
                }
                catch (System.Text.Json.JsonException)
                {
                    return new ApiResponse<bool> { Success = false, Message = $"Lỗi máy chủ: {response.StatusCode} ({response.ReasonPhrase})" };
                }
                return apiResponse ?? new ApiResponse<bool> { Success = response.IsSuccessStatusCode, Message = "Kết quả đăng ký không xác định" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = $"Lỗi kết nối: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<bool>> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Auth.ChangePassword, new { currentPassword, newPassword });
                ApiResponse<bool>? apiResponse = null;

                try
                {
                    apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
                }
                catch (System.Text.Json.JsonException)
                {
                    return new ApiResponse<bool> { Success = false, Message = $"Lỗi máy chủ: {response.StatusCode} ({response.ReasonPhrase})" };
                }
                return apiResponse ?? new ApiResponse<bool> { Success = response.IsSuccessStatusCode, Message = "Kết quả đổi mật khẩu không xác định" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = $"Lỗi kết nối: {ex.Message}" };
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                await _httpClient.PostAsync(ApiEndpoints.Auth.Logout, null);
            }
            catch { }
            finally
            {
                // Xóa token khỏi header HttpClient cho dù gọi API thành công hay thất bại
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<UserDto?> GetCurrentUserAsync()
        {
            // Implement profile detection if needed
            return null;
        }
    }
}
