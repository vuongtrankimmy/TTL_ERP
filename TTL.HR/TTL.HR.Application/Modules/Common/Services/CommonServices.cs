using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;
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
        public async Task<List<LookupModel>> GetLookupsAsync(string type, string? lang = null)
        {
            var url = $"{ApiEndpoints.Lookups.Base}?type={type}";
            if (!string.IsNullOrEmpty(lang)) url += $"&LanguageCode={lang}";

            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<LookupModel>>>(url);
            return response?.Data ?? new List<LookupModel>();
        }
        public async Task<List<LookupModel>> GetCachedLookupsAsync(string type, string? lang = null)
        {
            var cacheKey = $"{type}_{lang ?? "default"}";
            if (!_cache.ContainsKey(cacheKey)) _cache[cacheKey] = await GetLookupsAsync(type, lang);
            return _cache[cacheKey];
        }
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly Microsoft.JSInterop.IJSRuntime _jsRuntime;
        private UserDto? _currentUser;
        private const string TokenKey = "authToken";
        private const string UserKey = "authUser";

        public AuthService(HttpClient httpClient, Microsoft.JSInterop.IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
        }

        public async Task InitializeAsync()
        {
            try
            {
                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey);
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var userJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", UserKey);
                    if (!string.IsNullOrEmpty(userJson))
                    {
                        _currentUser = System.Text.Json.JsonSerializer.Deserialize<UserDto>(userJson);
                    }
                }
            }
            catch { }
        }

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
                        var token = apiResponse.Data.AccessToken;
                        _currentUser = apiResponse.Data.User;
                        
                        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                        
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", UserKey, System.Text.Json.JsonSerializer.Serialize(_currentUser));
                    }
                    return apiResponse;
                }
                
                if (apiResponse != null && !apiResponse.Success && apiResponse.Errors != null && apiResponse.Errors.Any())
                {
                    apiResponse.Message = string.Join(". ", apiResponse.Errors);
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
                var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Auth.ChangePassword, new { CurrentPassword = currentPassword, NewPassword = newPassword });
                ApiResponse<bool>? apiResponse = null;
                try { apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(); } catch { }

                if (!response.IsSuccessStatusCode)
                {
                    string combinedMessage = apiResponse?.Message ?? response.ReasonPhrase ?? "Lỗi không xác định";
                    if (apiResponse?.Errors != null && apiResponse.Errors.Any())
                    {
                        combinedMessage = string.Join(". ", apiResponse.Errors);
                    }
                    return new ApiResponse<bool> { Success = false, Message = combinedMessage };
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
                _httpClient.DefaultRequestHeaders.Authorization = null;
                _currentUser = null;
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", UserKey);
            }
        }

        public async Task<ApiResponse<bool>> ConfirmEmailAsync(string token)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoints.Auth.ConfirmEmail}?token={token}");
                ApiResponse<bool>? apiResponse = null;
                try { apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(); } catch { }
                return apiResponse ?? new ApiResponse<bool> { Success = response.IsSuccessStatusCode, Message = "Xác thực email thất bại" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = $"Lỗi kết nối: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<bool>> RequestPasswordResetAsync(string email)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Auth.RequestPasswordReset, new { Email = email });
                ApiResponse<bool>? apiResponse = null;
                try { apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(); } catch { }
                return apiResponse ?? new ApiResponse<bool> { Success = response.IsSuccessStatusCode, Message = "Yêu cầu đặt lại mật khẩu thất bại" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = $"Lỗi kết nối: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<bool>> ResetPasswordAsync(string token, string newPassword)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Auth.ResetPassword, new { Token = token, NewPassword = newPassword });
                ApiResponse<bool>? apiResponse = null;
                try { apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(); } catch { }
                return apiResponse ?? new ApiResponse<bool> { Success = response.IsSuccessStatusCode, Message = "Đặt lại mật khẩu thất bại" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = $"Lỗi kết nối: {ex.Message}" };
            }
        }

        public async Task<UserDto?> GetCurrentUserAsync()
        {
            return _currentUser;
        }
    }
}
