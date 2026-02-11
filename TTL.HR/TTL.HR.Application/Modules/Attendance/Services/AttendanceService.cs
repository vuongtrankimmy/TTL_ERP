using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Attendance.Interfaces;
using TTL.HR.Application.Modules.Attendance.Models;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.Common.Constants;

namespace TTL.HR.Application.Modules.Attendance.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly HttpClient _httpClient;
        public AttendanceService(HttpClient httpClient) => _httpClient = httpClient;
        public async Task<IEnumerable<AttendanceModel>> GetTimesheetsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<AttendanceModel>>>(ApiEndpoints.Attendance.Timesheets);
            return response?.Data ?? new List<AttendanceModel>();
        }

        public async Task<IEnumerable<WorkScheduleModel>> GetWorkSchedulesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<WorkScheduleModel>>>(ApiEndpoints.Attendance.WorkSchedules);
            return response?.Data ?? new List<WorkScheduleModel>();
        }

        public async Task<IEnumerable<ShiftRequestModel>> GetShiftRequestsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<ShiftRequestModel>>>(ApiEndpoints.Attendance.ShiftRequests);
            return response?.Data ?? new List<ShiftRequestModel>();
        }

        public async Task<bool> ProcessShiftRequestAsync(string id, bool approved, string? note)
        {
            var url = $"{ApiEndpoints.Attendance.ShiftRequests}/{id}/process";
            var response = await _httpClient.PostAsJsonAsync(url, new { Approved = approved, Note = note });
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<AttendanceDetailModel>> GetAttendanceDetailsAsync(string employeeId, DateTime month)
        {
            var url = $"{ApiEndpoints.Attendance.Base}/details/{employeeId}?month={month:yyyy-MM}";
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<AttendanceDetailModel>>>(url);
            return response?.Data ?? new List<AttendanceDetailModel>();
        }

        public async Task<bool> CheckInAsync(AttendanceModel attendance)
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Attendance.Base}/check-in", attendance);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CheckOutAsync(string id, AttendanceModel attendance)
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Attendance.Base}/check-out/{id}", attendance);
            return response.IsSuccessStatusCode;
        }
    }
}
