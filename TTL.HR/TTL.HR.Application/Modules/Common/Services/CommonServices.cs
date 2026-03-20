using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.Common.Constants;
using System.Collections.Concurrent;
using Newtonsoft.Json;

namespace TTL.HR.Application.Modules.Common.Services
{
    public class ApiRepository<T> : IApiRepository<T> where T : class
    {
        private readonly HttpClient _httpClient;
        public ApiRepository(HttpClient httpClient) => _httpClient = httpClient;
        public async Task<List<T>> GetAllAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<T>>>(content);
                    return apiResponse?.Data ?? new List<T>();
                }
            }
            catch { }
            return new List<T>();
        }
        public async Task<T?> GetByIdAsync(string endpoint, string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{endpoint}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(content);
                    return apiResponse?.Data;
                }
            }
            catch { }
            return null;
        }
        public async Task<T> CreateAsync(string endpoint, T entity)
        {
            var json = JsonConvert.SerializeObject(entity);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(responseContent);
            return apiResponse?.Data!;
        }
        public async Task UpdateAsync(string endpoint, string id, T entity) 
        {
            var json = JsonConvert.SerializeObject(entity);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await _httpClient.PutAsync($"{endpoint}/{id}", content);
        }
        public async Task DeleteAsync(string endpoint, string id) => await _httpClient.DeleteAsync($"{endpoint}/{id}");
    }

    public class MasterDataService : IMasterDataService
    {
        private readonly HttpClient _httpClient;
        private static readonly ConcurrentDictionary<string, List<LookupModel>> _cache = new();
        private static readonly ConcurrentDictionary<string, List<CountryModel>> _countryCache = new();
        private static readonly ConcurrentDictionary<string, string> _codeToIdMap = new(); // Mapping from integer Code to string Id instance
        
        public MasterDataService(HttpClient httpClient) => _httpClient = httpClient;
        
        public async Task<List<LookupModel>> GetLookupsAsync(string type, string? lang = null)
        {
            try
            {
                var url = $"{ApiEndpoints.Lookups.Base}?type={type}";
                if (!string.IsNullOrEmpty(lang)) url += $"&LanguageCode={lang}";

                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<LookupModel>>>(content);
                    return apiResponse?.Data ?? new List<LookupModel>();
                }
            }
            catch { }
            return new List<LookupModel>();
        }

        public async Task<List<LookupModel>> GetCachedLookupsAsync(string type, string? lang = null)
        {
            var cacheKey = $"{type}_{lang ?? "default"}";
            if (_cache.TryGetValue(cacheKey, out var cached)) return cached;
            
            var data = await GetLookupsAsync(type, lang);
            _cache.TryAdd(cacheKey, data);
            return data;
        }

        public async Task<List<CountryModel>> GetCountriesAsync(string? lang = null)
        {
            try
            {
                var url = $"{ApiEndpoints.System.Countries}";
                if (!string.IsNullOrEmpty(lang)) url += $"?lang={lang}";

                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<CountryModel>>>(content);
                    return apiResponse?.Data ?? new List<CountryModel>();
                }
            }
            catch { }
            return new List<CountryModel>();
        }

        public async Task<List<CountryModel>> GetCachedCountriesAsync(string? lang = null)
        {
            var cacheKey = lang ?? "default";
            if (_countryCache.TryGetValue(cacheKey, out var cached)) return cached;
            
            var data = await GetCountriesAsync(lang);
            _countryCache.TryAdd(cacheKey, data);
            return data;
        }

        public async Task<List<LookupModel>> GetProvincesAsync(string? lang = null)
        {
            try
            {
                var url = ApiEndpoints.System.AdministrativeDivisions.Provinces;
                if (!string.IsNullOrEmpty(lang)) url += $"?lang={lang}";

                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<LookupModel>>>(content);
                    var list = apiResponse?.Data ?? new List<LookupModel>();
                    foreach (var item in list) if (!string.IsNullOrEmpty(item.Code)) _codeToIdMap[item.Code] = item.Id;
                    return list;
                }
            }
            catch { }
            return new List<LookupModel>();
        }

        public async Task<List<LookupModel>> GetDistrictsAsync(string provinceId, string? lang = null)
        {
            try
            {
                string finalId = provinceId;
                if (int.TryParse(provinceId, out _) && _codeToIdMap.TryGetValue(provinceId, out var mappedId))
                {
                    finalId = mappedId;
                }

                var url = ApiEndpoints.System.AdministrativeDivisions.Districts(finalId);
                if (!string.IsNullOrEmpty(lang)) url += $"?lang={lang}";

                Console.WriteLine($"[MasterDataService] Fetching districts from: {url}");
                var response = await _httpClient.GetAsync(url);
                Console.WriteLine($"[MasterDataService] Response status: {response.StatusCode} for {url}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<LookupModel>>>(content);
                    var list = apiResponse?.Data ?? new List<LookupModel>();
                    foreach (var item in list) if (!string.IsNullOrEmpty(item.Code)) _codeToIdMap[item.Code] = item.Id;
                    return list;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[MasterDataService] Error detail: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MasterDataService] EXCEPTION in GetDistrictsAsync: {ex.Message}");
            }
            return new List<LookupModel>();
        }

        public async Task<List<LookupModel>> GetWardsAsync(string? districtId = null, string? provinceId = null, string? lang = null)
        {
            try
            {
                string? finalDistrictId = districtId;
                string? finalProvinceId = provinceId;

                // Nếu districtId là số, thử map sang string ID
                if (!string.IsNullOrEmpty(districtId) && int.TryParse(districtId, out _) && _codeToIdMap.TryGetValue(districtId, out var dId))
                {
                    finalDistrictId = dId;
                }

                // Nếu provinceId là số, thử map sang string ID
                if (!string.IsNullOrEmpty(provinceId) && int.TryParse(provinceId, out _) && _codeToIdMap.TryGetValue(provinceId, out var pId))
                {
                    finalProvinceId = pId;
                }

                string url;
                if (!string.IsNullOrEmpty(finalDistrictId))
                {
                    url = ApiEndpoints.System.AdministrativeDivisions.Wards(finalDistrictId);
                }
                else if (!string.IsNullOrEmpty(finalProvinceId))
                {
                    url = ApiEndpoints.System.AdministrativeDivisions.WardsByProvince(finalProvinceId);
                }
                else
                {
                    return new List<LookupModel>();
                }

                if (!string.IsNullOrEmpty(lang)) url += $"?lang={lang}";

                var fullUrl = new Uri(_httpClient.BaseAddress!, url);
                Console.WriteLine($"[MasterDataService] Fetching wards from: {fullUrl}");
                var response = await _httpClient.GetAsync(url);
                Console.WriteLine($"[MasterDataService] Response status: {response.StatusCode} for {url}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<LookupModel>>>(content);
                    var list = apiResponse?.Data ?? new List<LookupModel>();
                    foreach (var item in list) if (!string.IsNullOrEmpty(item.Code)) _codeToIdMap[item.Code] = item.Id;
                    return list;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[MasterDataService] Error content: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MasterDataService] Exception in GetWardsAsync: {ex.Message}");
            }
            return new List<LookupModel>();
        }

        // Int overloads: dùng khi ID là số nguyên (SQL/API mới)
        public async Task<List<LookupModel>> GetDistrictsAsync(int provinceId, string? lang = null)
        {
            try
            {
                // Mapping: Chuyển từ int code sang string ID của MongoDB để gọi API
                string codeStr = provinceId.ToString();
                if (!_codeToIdMap.TryGetValue(codeStr, out var stringId))
                {
                    // Nếu chưa có trong map, nạp lại Provinces
                    var provinces = await GetProvincesAsync();
                    stringId = provinces.FirstOrDefault(p => p.Code == codeStr)?.Id;
                }

                if (string.IsNullOrEmpty(stringId))
                {
                    Console.WriteLine($"[MasterDataService] Cannot find string ID for Province Code: {provinceId}");
                    return new List<LookupModel>();
                }

                var url = ApiEndpoints.System.AdministrativeDivisions.Districts(stringId);
                if (!string.IsNullOrEmpty(lang)) url += $"?lang={lang}";

                Console.WriteLine($"[MasterDataService] Fetching districts for Code {provinceId} using ID {stringId} from: {url}");
                var response = await _httpClient.GetAsync(url);
                Console.WriteLine($"[MasterDataService] Response status: {response.StatusCode} for {url}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<LookupModel>>>(content);
                    var list = apiResponse?.Data ?? new List<LookupModel>();
                    foreach (var item in list) if (!string.IsNullOrEmpty(item.Code)) _codeToIdMap[item.Code] = item.Id;
                    return list;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[MasterDataService] Error detail: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MasterDataService] EXCEPTION in GetDistrictsAsync(int): {ex.Message}");
            }
            return new List<LookupModel>();
        }

        public async Task<List<LookupModel>> GetWardsAsync(int districtId, string? lang = null)
        {
            try
            {
                // Mapping: Chuyển từ int code sang string ID của MongoDB để gọi API
                string codeStr = districtId.ToString();
                if (!_codeToIdMap.TryGetValue(codeStr, out var stringId))
                {
                    // Ghi chú: Case này hiếm vì thường load district trước đó đã fill map rồi
                    Console.WriteLine($"[MasterDataService] Warning: Code {districtId} not found in map, attempt to load may fail if API doesn't support numeric code.");
                    stringId = codeStr; 
                }

                var url = ApiEndpoints.System.AdministrativeDivisions.Wards(stringId);
                if (!string.IsNullOrEmpty(lang)) url += $"?lang={lang}";

                Console.WriteLine($"[MasterDataService] Fetching wards for Code {districtId} using ID {stringId} from: {url}");
                var response = await _httpClient.GetAsync(url);
                Console.WriteLine($"[MasterDataService] Response status: {response.StatusCode} for {url}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<LookupModel>>>(content);
                    var list = apiResponse?.Data ?? new List<LookupModel>();
                    foreach (var item in list) if (!string.IsNullOrEmpty(item.Code)) _codeToIdMap[item.Code] = item.Id;
                    return list;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[MasterDataService] Error content: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MasterDataService] Exception in GetWardsAsync(int): {ex.Message}");
            }
            return new List<LookupModel>();
        }

        public async Task<List<LookupModel>> GetStreetsAsync(string? provinceId = null, string? wardId = null, string? lang = null)
        {
            try
            {
                string? finalProvinceId = provinceId;
                string? finalWardId = wardId;

                if (!string.IsNullOrEmpty(provinceId) && int.TryParse(provinceId, out _) && _codeToIdMap.TryGetValue(provinceId, out var pId))
                    finalProvinceId = pId;
                
                if (!string.IsNullOrEmpty(wardId) && int.TryParse(wardId, out _) && _codeToIdMap.TryGetValue(wardId, out var wId))
                    finalWardId = wId;

                var queryParams = new List<string>();
                if (!string.IsNullOrEmpty(finalProvinceId)) queryParams.Add($"provinceId={finalProvinceId}");
                if (!string.IsNullOrEmpty(finalWardId)) queryParams.Add($"wardId={finalWardId}");
                if (!string.IsNullOrEmpty(lang)) queryParams.Add($"lang={lang}");

                var url = $"{ApiEndpoints.System.AdministrativeDivisions.Base}/streets";
                if (queryParams.Any()) url += "?" + string.Join("&", queryParams);

                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<LookupModel>>>(content);
                    return apiResponse?.Data ?? new List<LookupModel>();
                }
            }
            catch { }
            return new List<LookupModel>();
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
                        _currentUser = JsonConvert.DeserializeObject<UserDto>(userJson);
                    }
                }
            }
            catch { }
        }

        public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
        {
            try
            {
                var requestJson = JsonConvert.SerializeObject(request);
                var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(ApiEndpoints.Auth.Login, requestContent);
                ApiResponse<AuthResponse>? apiResponse = null;

                try
                {
                    var content = await response.Content.ReadAsStringAsync();
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse<AuthResponse>>(content);
                }
                catch (JsonException)
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
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", UserKey, JsonConvert.SerializeObject(_currentUser));
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
                var requestJson = JsonConvert.SerializeObject(request);
                var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(ApiEndpoints.Auth.Register, requestContent);
                ApiResponse<bool>? apiResponse = null;
                
                try
                {
                    var content = await response.Content.ReadAsStringAsync();
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(content);
                }
                catch (JsonException)
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
                var requestJson = JsonConvert.SerializeObject(new { CurrentPassword = currentPassword, NewPassword = newPassword });
                var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(ApiEndpoints.Auth.ChangePassword, requestContent);
                ApiResponse<bool>? apiResponse = null;
                try { 
                    var content = await response.Content.ReadAsStringAsync();
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(content); 
                } catch { }

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
                try { 
                    var content = await response.Content.ReadAsStringAsync();
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(content); 
                } catch { }
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
                var requestJson = JsonConvert.SerializeObject(new { Email = email });
                var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(ApiEndpoints.Auth.RequestPasswordReset, requestContent);
                ApiResponse<bool>? apiResponse = null;
                try { 
                    var content = await response.Content.ReadAsStringAsync();
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(content); 
                } catch { }
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
                var requestJson = JsonConvert.SerializeObject(new { Token = token, NewPassword = newPassword });
                var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(ApiEndpoints.Auth.ResetPassword, requestContent);
                ApiResponse<bool>? apiResponse = null;
                try { 
                    var content = await response.Content.ReadAsStringAsync();
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(content); 
                } catch { }
                return apiResponse ?? new ApiResponse<bool> { Success = response.IsSuccessStatusCode, Message = "Đặt lại mật khẩu thất bại" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = $"Lỗi kết nối: {ex.Message}" };
            }
        }
        
        public async Task<ApiResponse<bool>> UpdateProfileAsync(UserDto request)
        {
            try
            {
                var command = new
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    Phone = request.Phone,
                    IdCardNumber = request.IdCardNumber,
                    JobTitle = request.JobTitle,
                    Hometown = request.Hometown,
                    CountryId = request.CountryId,
                    ProvinceId = request.ProvinceId,
                    DistrictId = request.DistrictId,
                    WardId = request.WardId,
                    Street = request.Street,
                    BankName = request.BankName,
                    BankAccount = request.BankAccount,
                    AvatarUrl = request.AvatarUrl
                };
                
                var requestJson = JsonConvert.SerializeObject(command);
                var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(ApiEndpoints.Auth.UpdateProfile, requestContent);
                ApiResponse<bool>? apiResponse = null;
                try { 
                    var content = await response.Content.ReadAsStringAsync();
                    apiResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(content); 
                } catch { }
                
                if (response.IsSuccessStatusCode && apiResponse?.Success == true)
                {
                    _currentUser = request;
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", UserKey, JsonConvert.SerializeObject(_currentUser));
                }
                
                if (apiResponse != null && !apiResponse.Success && apiResponse.Errors != null && apiResponse.Errors.Any())
                {
                    apiResponse.Message = string.Join(". ", apiResponse.Errors);
                }
                
                return apiResponse ?? new ApiResponse<bool> { Success = response.IsSuccessStatusCode, Message = "Cập nhật hồ sơ thất bại" };
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
