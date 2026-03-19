using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.HumanResource.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Models;
using TTL.HR.Application.Modules.HumanResource.Entities;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.Common.Constants;

namespace TTL.HR.Application.Modules.HumanResource.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly HttpClient _httpClient;
        public EmployeeService(HttpClient httpClient) => _httpClient = httpClient;
        public async Task<List<EmployeeDto>> GetEmployeesAsync()
        {
            var response = await _httpClient.GetAsync($"{ApiEndpoints.Employees.Base}?pageSize=9999");
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Không thể tải danh sách nhân viên: {(int)response.StatusCode} {response.ReasonPhrase}. {errorBody}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var options = new System.Text.Json.JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true,
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
            };
            var result = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<PagedResult<EmployeeDto>>>(content, options);
            return result?.Data?.Items ?? new List<EmployeeDto>();
        }

        public async Task<PagedResult<EmployeeDto>> GetEmployeesPaginatedAsync(int pageIndex, int pageSize, string? searchTerm = null, IEnumerable<string>? departmentIds = null, string? status = null, string? workplace = null, string? sortBy = "name", bool sortDesc = false)
        {
            var url = $"{ApiEndpoints.Employees.Base}?pageIndex={pageIndex}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(searchTerm)) url += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";
            
            if (departmentIds != null && departmentIds.Any())
            {
                foreach (var id in departmentIds)
                {
                    if (!string.IsNullOrEmpty(id)) url += $"&departmentId={id}";
                }
            }
            if (!string.IsNullOrEmpty(status)) url += $"&status={status}";
            if (!string.IsNullOrEmpty(workplace)) url += $"&workplace={Uri.EscapeDataString(workplace)}";
            if (!string.IsNullOrEmpty(sortBy)) url += $"&sortBy={sortBy}";
            url += $"&sortDesc={sortDesc.ToString().ToLower()}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Lỗi tải dữ liệu phân trang nhân viên: {(int)response.StatusCode}. {errorBody}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var options = new System.Text.Json.JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true,
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
            };
            var result = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<PagedResult<EmployeeDto>>>(content, options);
            return result?.Data ?? new PagedResult<EmployeeDto>();
        }
        public async Task<EmployeeModel?> GetEmployeeAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiEndpoints.Employees.Base}/{id}");
                var content = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    try 
                    {
                        var options = new System.Text.Json.JsonSerializerOptions 
                        { 
                            PropertyNameCaseInsensitive = true,
                            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
                        };
                        var result = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<EmployeeModel>>(content, options);
                        
                        if (result?.Data == null)
                        {
                            Console.WriteLine($"[EmployeeService] Warning: API returned success but Data is null for employee {id}. Raw: {content.Substring(0, Math.Min(content.Length, 200))}");
                        }
                        
                        return result?.Data;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[EmployeeService] Deserialization error for employee {id}: {ex.Message}");
                        Console.WriteLine($"[EmployeeService] Raw JSON causing error: {content}");
                        return null; 
                    }
                }
                else
                {
                    Console.WriteLine($"[EmployeeService] GetEmployee failed: {response.StatusCode} - {content}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EmployeeService] Connection error for employee {id}: {ex.Message}");
            }
            return null;
        }

        public async Task<EmployeeModel?> GetMyEmployeeAsync()
        {
            try
            {
                var httpResponse = await _httpClient.GetAsync(ApiEndpoints.Employees.Me);
                var body = await httpResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"[GetMyEmployee] HTTP {(int)httpResponse.StatusCode}: {body.Substring(0, Math.Min(300, body.Length))}");

                if (!httpResponse.IsSuccessStatusCode) return null;

                var options = new System.Text.Json.JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
                };
                var response = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<EmployeeModel>>(body, options);
                Console.WriteLine($"[GetMyEmployee] EmployeeId={response?.Data?.Id}, Name={response?.Data?.FullName}");
                return response?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetMyEmployee] Exception: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> CreateEmployeeAsync(Employee employee)
        {
            try
            {
                Console.WriteLine($"[EmployeeService] POST {ApiEndpoints.Employees.Base}...");
                var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Employees.Base, employee);
                Console.WriteLine($"[EmployeeService] Received {(int)response.StatusCode} {response.ReasonPhrase}");
                
                if (response.IsSuccessStatusCode)
                {
                    try 
                    {
                        var successResult = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
                        return successResult?.Data;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[EmployeeService] Error parsing success JSON: {ex.Message}");
                        // If parsing fails but status is success, maybe it returned a plain string or something else
                        var content = await response.Content.ReadAsStringAsync();
                        return content; // Try returning raw content if it looks like an ID
                    }
                }

                Console.WriteLine($"[EmployeeService] Handling error branch...");
                try
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
                    if (result != null && !string.IsNullOrEmpty(result.Message))
                    {
                        var errorMsg = result.Message;
                        if (result.Errors != null && result.Errors.Any())
                        {
                            errorMsg += ": " + string.Join(", ", result.Errors);
                        }
                        return errorMsg; 
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[EmployeeService] Error parsing error JSON: {ex.Message}");
                }
                
                var rawError = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[EmployeeService] Raw error content: {rawError?.Substring(0, Math.Min(rawError?.Length ?? 0, 100))}");
                return string.IsNullOrEmpty(rawError) ? $"Error {response.StatusCode}" : rawError;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public async Task<string?> UpdateEmployeeAsync(string id, UpdateEmployeeRequest request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoints.Employees.Base}/{id}", request);
                if (response.IsSuccessStatusCode) return null;

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
                if (result != null && !string.IsNullOrEmpty(result.Message))
                {
                    var errorMsg = result.Message;
                    if (result.Errors != null && result.Errors.Any())
                    {
                        errorMsg += ": " + string.Join(", ", result.Errors);
                    }
                    return errorMsg;
                }
                
                var rawError = await response.Content.ReadAsStringAsync();
                return string.IsNullOrEmpty(rawError) ? "Unknown error occurred" : rawError;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public async Task<bool> DeleteEmployeeAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiEndpoints.Employees.Base}/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<DigitalProfileModel?> GetDigitalProfileAsync(string employeeId)
        {
            try
            {
                var httpResponse = await _httpClient.GetAsync(
                    ApiEndpoints.Employees.DigitalProfile(employeeId));

                if (!httpResponse.IsSuccessStatusCode)
                {
                    var errContent = await httpResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"[DigitalProfile] HTTP {(int)httpResponse.StatusCode}: {errContent}");
                    return null;
                }

                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"[DigitalProfile] Raw: {responseContent}");

                var options = new System.Text.Json.JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
                };
                var response = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<DigitalProfileModel>>(responseContent, options);
                return response?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DigitalProfile] Exception: {ex.Message}");
                return null;
            }
        }

        // Returns null on success, or error message string on failure
        public async Task<string?> UploadDocumentAsync(string employeeId, string documentType, Stream fileStream, string fileName, DateTime? expiryDate, string? note)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                content.Add(new StringContent(employeeId), "EmployeeId");
                content.Add(new StringContent(documentType), "DocumentType");

                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                    GetMimeType(fileName));
                content.Add(fileContent, "File", fileName);

                if (expiryDate.HasValue)
                    content.Add(new StringContent(expiryDate.Value.ToString("yyyy-MM-dd")), "ExpiryDate");
                if (!string.IsNullOrEmpty(note))
                    content.Add(new StringContent(note), "Note");

                var response = await _httpClient.PostAsync(
                    ApiEndpoints.Employees.Documents(employeeId), content);

                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[UploadDoc] HTTP {(int)response.StatusCode}: {responseBody}");

                if (response.IsSuccessStatusCode)
                {
                    // SUCCESS — return null (no error)
                    return null;
                }

                // Try to parse error message from ApiResponse
                try
                {
                    var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var errorResponse = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<object>>(responseBody, options);
                    if (!string.IsNullOrEmpty(errorResponse?.Message))
                        return errorResponse.Message;
                }
                catch { }

                return $"Lỗi {(int)response.StatusCode}: {response.ReasonPhrase}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UploadDoc] Exception: {ex.Message}");
                return $"Lỗi kết nối: {ex.Message}";
            }
        }

        public async Task<bool> LinkIdentityAsync(string employeeId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{ApiEndpoints.Employees.Base}/link-identity/{employeeId}", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LinkIdentity] Exception: {ex.Message}");
                return false;
            }
        }

        public async Task<byte[]?> ExportEmployeesAsync(string? searchTerm = null, IEnumerable<string>? departmentIds = null, string? status = null, string? workplace = null)
        {
            try
            {
                var url = $"{ApiEndpoints.Employees.Base}/export?1=1";
                if (!string.IsNullOrEmpty(searchTerm)) url += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";
                
                if (departmentIds != null && departmentIds.Any())
                {
                    foreach (var id in departmentIds)
                    {
                        if (!string.IsNullOrEmpty(id)) url += $"&departmentId={id}";
                    }
                }
                if (!string.IsNullOrEmpty(status)) url += $"&status={status}";
                if (!string.IsNullOrEmpty(workplace)) url += $"&workplace={Uri.EscapeDataString(workplace)}";

                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ExportEmployees] Exception: {ex.Message}");
                return null;
            }
        }
        public async Task<EmployeeStatusCounts> GetStatusCountsAsync(string? searchTerm = null, IEnumerable<string>? departmentIds = null, string? workplace = null)
        {
            var url = $"{ApiEndpoints.Employees.Base}/counts?1=1";
            if (!string.IsNullOrEmpty(searchTerm)) url += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";
            
            if (departmentIds != null && departmentIds.Any())
            {
                foreach (var id in departmentIds)
                {
                    if (!string.IsNullOrEmpty(id)) url += $"&departmentId={id}";
                }
            }
            if (!string.IsNullOrEmpty(workplace)) url += $"&workplace={Uri.EscapeDataString(workplace)}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Lỗi tải số liệu trạng thái: {(int)response.StatusCode}. {errorBody}");
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<EmployeeStatusCounts>>();
            return result?.Data ?? new EmployeeStatusCounts();
        }
        public async Task<string?> AccrueLeaveAsync(int month, int year)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Employees.Base}/accrue-leave", new { Month = month, Year = year });
                if (response.IsSuccessStatusCode) return null;

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
                return result?.Message ?? $"Lỗi {(int)response.StatusCode}";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<bool> SendCredentialsAsync(string employeeId, string channel, string? customMessage = null)
        {
            try
            {
                var url = ApiEndpoints.Notifications.SendCredentials(employeeId) + $"?channel={channel}";
                if (!string.IsNullOrEmpty(customMessage)) url += $"&message={Uri.EscapeDataString(customMessage)}";
                
                var response = await _httpClient.PostAsync(url, null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SendCredentials] Exception: {ex.Message}");
                return false;
            }
        }

        private static string GetMimeType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLower();
            return ext switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };
        }
    }
}
