using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Linq;
using TTL.HR.Application.Modules.Training.Interfaces;
using TTL.HR.Application.Modules.Training.Models;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.Common.Constants;

namespace TTL.HR.Application.Modules.Training.Services
{
    public class TrainingService : ITrainingService
    {
        private readonly HttpClient _httpClient;
        public TrainingService(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<IEnumerable<CourseModel>> GetCoursesAsync()
        {
            try
            {
                // Revert to working endpoint for listing
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<PagedResult<CourseModel>>>(ApiEndpoints.Training.Courses);
                return response?.Data?.Items ?? new List<CourseModel>();
            }
            catch (Exception)
            {
                return new List<CourseModel>();
            }
        }

        public async Task<CourseModel?> GetCourseAsync(string id)
        {
            try
            {
                // Revert to working endpoint for detail
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<CourseModel>>($"{ApiEndpoints.Training.Courses}/{id}");
                return response?.Data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ApiResponse<string>> CreateCourseAsync(CourseModel course)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Training.Courses, course);
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ApiResponse<string>>() ?? new ApiResponse<string> { Success = true };
                }

                // Try to parse detailed error response
                return await ParseErrorResponse<string>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string> { Success = false, Message = $"Lỗi hệ thống: {ex.Message}" };
            }
        }

        private async Task<ApiResponse<T>> ParseErrorResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                // 1. Try standard ApiResponse
                var apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<T>>(content, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (apiResponse != null && (!string.IsNullOrEmpty(apiResponse.Message) || (apiResponse.Errors != null && apiResponse.Errors.Any())))
                {
                    return apiResponse;
                }

                // 2. Try standard .NET ValidationProblemDetails
                var problem = System.Text.Json.JsonSerializer.Deserialize<ValidationProblemDetails>(content, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (problem != null && problem.Errors != null && problem.Errors.Any())
                {
                    var errors = new List<string>();
                    foreach (var kvp in problem.Errors)
                    {
                        foreach (var err in kvp.Value)
                        {
                            errors.Add($"{kvp.Key}: {err}");
                        }
                    }
                    return new ApiResponse<T> { Success = false, Message = "Dữ liệu không hợp lệ", Errors = errors };
                }
            }
            catch (Exception)
            {
                // Fallback to raw content or status code
            }

            return new ApiResponse<T> { Success = false, Message = $"Lỗi: {response.StatusCode} - {content}" };
        }

        // Add this class at the end or in a separate file
        private class ValidationProblemDetails
        {
            public Dictionary<string, string[]>? Errors { get; set; }
        }

        public async Task<ApiResponse<bool>> UpdateCourseAsync(string id, CourseModel course)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoints.Training.Courses}/{id}", course);
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>() ?? new ApiResponse<bool> { Success = true, Data = true };
                }

                return await ParseErrorResponse<bool>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = $"Lỗi hệ thống: {ex.Message}" };
            }
        }

        public async Task<bool> DeleteCourseAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiEndpoints.Training.Courses}/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RegisterCourseAsync(string courseId)
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Training.Courses}/enroll", new { CourseId = courseId });
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<ParticipantModel>> GetParticipantsAsync(string courseId)
        {
            try
            {
                // Fetch the full course detail which already contains participants
                var course = await GetCourseAsync(courseId);
                return course?.EnrolledEmployees ?? new List<ParticipantModel>();
            }
            catch (Exception)
            {
                return new List<ParticipantModel>();
            }
        }

        public async Task<bool> RemoveParticipantAsync(string courseId, string employeeId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiEndpoints.Training.Courses}/{courseId}/participants/{employeeId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<ApiResponse<bool>> RegisterParticipantsAsync(string courseId, List<string> employeeIds)
        {
            try
            {
                // Using the more robust unified endpoint for assignment
                var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Training.TrainingCourses}/assign", new { CourseId = courseId, EmployeeIds = employeeIds });
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
                    return result ?? new ApiResponse<bool> { Success = true, Data = true };
                }

                try
                {
                    var error = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
                    if (error != null) return error;
                }
                catch { }

                return new ApiResponse<bool> 
                { 
                    Success = false, 
                    Message = $"Lỗi hệ thống: {response.StatusCode}" 
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool> { Success = false, Message = $"Lỗi kết nối: {ex.Message}" };
            }
        }

        public async Task<TrainingAnalyticsModel?> GetAnalyticsAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<TrainingAnalyticsModel>>(ApiEndpoints.Training.Base + "/analytics");
                return response?.Data;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
