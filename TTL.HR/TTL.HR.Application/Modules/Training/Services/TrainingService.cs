using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
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
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<CourseModel>>>(ApiEndpoints.Training.TrainingCourses);
            return response?.Data ?? new List<CourseModel>();
        }
        public async Task<CourseModel?> GetCourseAsync(string id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<CourseModel>>($"{ApiEndpoints.Training.TrainingCourses}/{id}");
            return response?.Data;
        }

        public async Task<bool> CreateCourseAsync(CourseModel course)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Training.TrainingCourses, course);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCourseAsync(string id, CourseModel course)
        {
            var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoints.Training.TrainingCourses}/{id}", course);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCourseAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiEndpoints.Training.TrainingCourses}/{id}");
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> RegisterCourseAsync(string courseId)
        {
            var response = await _httpClient.PostAsync($"{ApiEndpoints.Training.TrainingCourses}/{courseId}/register", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<ParticipantModel>> GetParticipantsAsync(string courseId)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<ParticipantModel>>>($"{ApiEndpoints.Training.TrainingCourses}/{courseId}/participants");
            return response?.Data ?? new List<ParticipantModel>();
        }

        public async Task<bool> RemoveParticipantAsync(string courseId, string employeeId)
        {
            var response = await _httpClient.DeleteAsync($"{ApiEndpoints.Training.TrainingCourses}/{courseId}/participants/{employeeId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RegisterParticipantsAsync(string courseId, List<string> employeeIds)
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Training.TrainingCourses}/{courseId}/participants/batch", new { EmployeeIds = employeeIds });
            return response.IsSuccessStatusCode;
        }
    }
}
