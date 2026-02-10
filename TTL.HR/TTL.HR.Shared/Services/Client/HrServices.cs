using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TTL.HR.Shared.Interfaces;
using TTL.HR.Shared.Models;

namespace TTL.HR.Shared.Services.Client
{
    public class DashboardService : IDashboardService
    {
        private readonly HttpClient _httpClient;
        public DashboardService(HttpClient httpClient) => _httpClient = httpClient;
        public async Task<DashboardOverviewModel?> GetOverviewAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<DashboardOverviewModel>>("api/v1/Dashboard/overview");
            return response?.Data;
        }
    }

    public class AttendanceService : IAttendanceService
    {
        private readonly HttpClient _httpClient;
        public AttendanceService(HttpClient httpClient) => _httpClient = httpClient;
        public async Task<IEnumerable<AttendanceModel>> GetTimesheetsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<AttendanceModel>>>("api/v1/Attendance");
            return response?.Data ?? new List<AttendanceModel>();
        }
        public async Task<bool> CheckInAsync(AttendanceModel attendance)
        {
            var response = await _httpClient.PostAsJsonAsync("api/v1/Attendance/check-in", attendance);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> CheckOutAsync(string id, AttendanceModel attendance)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/v1/Attendance/check-out/{id}", attendance);
            return response.IsSuccessStatusCode;
        }
    }

    public class LeaveService : ILeaveService
    {
        private readonly HttpClient _httpClient;
        public LeaveService(HttpClient httpClient) => _httpClient = httpClient;
        public async Task<IEnumerable<LeaveRequestModel>> GetLeaveRequestsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<LeaveRequestModel>>>("api/v1/Leave");
            return response?.Data ?? new List<LeaveRequestModel>();
        }
        public async Task<bool> SubmitLeaveRequestAsync(LeaveRequestModel request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/v1/Leave", request);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> ApproveLeaveRequestAsync(string id, string status)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/v1/Leave/{id}/approve", new { Status = status });
            return response.IsSuccessStatusCode;
        }
    }

    public class TrainingService : ITrainingService
    {
        private readonly HttpClient _httpClient;
        public TrainingService(HttpClient httpClient) => _httpClient = httpClient;
        public async Task<IEnumerable<CourseModel>> GetCoursesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<CourseModel>>>("api/v1/Training");
            return response?.Data ?? new List<CourseModel>();
        }
        public async Task<CourseModel?> GetCourseAsync(string id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<CourseModel>>($"api/v1/Training/{id}");
            return response?.Data;
        }
        public async Task<bool> RegisterCourseAsync(string courseId)
        {
            var response = await _httpClient.PostAsync($"api/v1/Training/{courseId}/register", null);
            return response.IsSuccessStatusCode;
        }
    }

    public class PayrollService : IPayrollService
    {
        private readonly HttpClient _httpClient;
        public PayrollService(HttpClient httpClient) => _httpClient = httpClient;
        public async Task<IEnumerable<PayrollModel>> GetPayrollsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<PayrollModel>>>("api/v1/Payroll");
            return response?.Data ?? new List<PayrollModel>();
        }
        public async Task<PayrollModel?> GetPayrollAsync(string id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<PayrollModel>>($"api/v1/Payroll/{id}");
            return response?.Data;
        }
    }

    public class AssetService : IAssetService
    {
        private readonly HttpClient _httpClient;
        public AssetService(HttpClient httpClient) => _httpClient = httpClient;
        public async Task<IEnumerable<AssetModel>> GetAssetsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<AssetModel>>>("api/v1/Assets");
            return response?.Data ?? new List<AssetModel>();
        }
        public async Task<AssetModel?> GetAssetAsync(string id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<AssetModel>>($"api/v1/Assets/{id}");
            return response?.Data;
        }
        public async Task<bool> AllocateAssetAsync(string assetId, string employeeId)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/v1/Assets/{assetId}/allocate", new { EmployeeId = employeeId });
            return response.IsSuccessStatusCode;
        }
    }

    public class RecruitmentService : IRecruitmentService
    {
        private readonly HttpClient _httpClient;
        public RecruitmentService(HttpClient httpClient) => _httpClient = httpClient;
        public async Task<IEnumerable<JobDetail>> GetJobsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<JobDetail>>>("api/v1/Recruitment");
            return response?.Data ?? new List<JobDetail>();
        }
        public async Task<JobDetail?> GetJobAsync(string id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<JobDetail>>($"api/v1/Recruitment/{id}");
            return response?.Data;
        }
        public async Task<JobDetail?> CreateJobAsync(JobDetail job)
        {
            var response = await _httpClient.PostAsJsonAsync("api/v1/Recruitment", job);
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<JobDetail>>();
            return apiResponse?.Data;
        }
        public async Task UpdateJobAsync(string id, JobDetail job) => await _httpClient.PutAsJsonAsync($"api/v1/Recruitment/{id}", job);
        public async Task DeleteJobAsync(string id) => await _httpClient.DeleteAsync($"api/v1/Recruitment/{id}");
    }
}
