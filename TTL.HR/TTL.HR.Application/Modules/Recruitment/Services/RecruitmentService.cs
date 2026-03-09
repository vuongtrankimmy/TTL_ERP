using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Recruitment.Interfaces;
using TTL.HR.Application.Modules.Recruitment.Models;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.Common.Constants;

namespace TTL.HR.Application.Modules.Recruitment.Services
{
    public class RecruitmentService : IRecruitmentService
    {
        private readonly HttpClient _httpClient;
        public RecruitmentService(HttpClient httpClient) => _httpClient = httpClient;
        public async Task<IEnumerable<JobDetail>> GetJobsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<JobDetail>>>(ApiEndpoints.Recruitment.Jobs);
            return response?.Data ?? new List<JobDetail>();
        }
        public async Task<JobDetail?> GetJobAsync(string id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<JobDetail>>($"{ApiEndpoints.Recruitment.Jobs}/{id}");
            return response?.Data;
        }
        public async Task<JobDetail?> CreateJobAsync(JobDetail job)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Recruitment.Jobs, job);
            if (!response.IsSuccessStatusCode)
            {
                var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
                throw new System.Exception(errorResult?.Message ?? $"API Error {response.StatusCode}");
            }
            
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<JobDetail>>();
            return apiResponse?.Data;
        }
        public async Task<bool> UpdateJobAsync(string id, JobDetail job)
        {
            var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoints.Recruitment.Jobs}/{id}", job);
            if (!response.IsSuccessStatusCode)
            {
                var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
                throw new System.Exception(errorResult?.Message ?? $"API Error {response.StatusCode}");
            }
            return true;
        }
        public async Task<bool> DeleteJobAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiEndpoints.Recruitment.Jobs}/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
                throw new System.Exception(errorResult?.Message ?? $"API Error {response.StatusCode}");
            }
            return true;
        }

        public async Task<IEnumerable<ApplicantItem>> GetApplicantsAsync(string jobId)
        {
            var url = $"{ApiEndpoints.Recruitment.CandidatesFull}?jobPostingId={jobId}&pageSize=100";
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<PagedResult<ApplicantItem>>>(url);
            return response?.Data?.Items ?? new List<ApplicantItem>();
        }

        public async Task<bool> AddApplicantAsync(string jobId, ApplicantRequest applicant)
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Recruitment.Jobs}/{jobId}/applicants", applicant);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateApplicantStatusAsync(string applicantId, string? status, int? statusId = null, string? note = null)
        {
            var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoints.Recruitment.Candidates}/{applicantId}/status", new { Status = status, StatusId = statusId, Note = note });
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ScheduleInterviewAsync(string applicantId, InterviewScheduleModel schedule)
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Recruitment.Candidates}/{applicantId}/schedule", schedule);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteApplicantAsync(string applicantId)
        {
            var response = await _httpClient.DeleteAsync($"{ApiEndpoints.Recruitment.Candidates}/{applicantId}");
            return response.IsSuccessStatusCode;
        }
    }
}
