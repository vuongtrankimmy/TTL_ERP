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

        public async Task<bool> CreateCourseAsync(CourseModel course)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Training.Courses, course);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCourseAsync(string id, CourseModel course)
        {
            var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoints.Training.Courses}/{id}", course);
            return response.IsSuccessStatusCode;
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
