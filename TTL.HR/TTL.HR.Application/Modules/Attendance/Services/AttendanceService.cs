using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Linq;
using System.Globalization;
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
        public async Task<IEnumerable<AttendanceModel>> GetTimesheetsAsync(int month = 0, int year = 0, string? departmentId = null, string? searchTerm = null)
        {
            if (month == 0) month = DateTime.Now.Month;
            if (year == 0) year = DateTime.Now.Year;

            var url = $"{ApiEndpoints.Attendance.Timesheets}?month={month}&year={year}";
            if (!string.IsNullOrEmpty(departmentId)) url += $"&departmentId={departmentId}";
            if (!string.IsNullOrEmpty(searchTerm)) url += $"&searchTerm={searchTerm}";
            
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<PagedResult<AttendanceModel>>>(url);
            return response?.Data?.Items ?? new List<AttendanceModel>();
        }

        public async Task<IEnumerable<WorkScheduleModel>> GetWorkSchedulesAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var url = ApiEndpoints.Attendance.WorkSchedules;
            if (startDate.HasValue) url += $"?startDate={startDate.Value:yyyy-MM-dd}";
            if (endDate.HasValue) url += $"{(startDate.HasValue ? "&" : "?")}endDate={endDate.Value:yyyy-MM-dd}";

            var response = await _httpClient.GetFromJsonAsync<ApiResponse<PagedResult<EmployeeScheduleDto>>>(url);
            var dtos = response?.Data?.Items ?? new List<EmployeeScheduleDto>();

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            
            // If the requested range doesn't include today, we use the first day of the range as "Today" for display purposes
            var displayDate = (startDate.HasValue && (today < startDate.Value || today > endDate.Value)) ? startDate.Value : today;
            var nextDisplayDate = displayDate.AddDays(1);

            return dtos.Select(dto => 
            {
                var currentShift = dto.Schedules.FirstOrDefault(s => s.Date.Date == displayDate.Date);
                
                // For NextShift, we find the next upcoming assigned shift after the displayDate
                var nextShift = dto.Schedules
                                    .Where(s => s.Date.Date > displayDate.Date && !string.IsNullOrEmpty(s.ShiftName))
                                    .OrderBy(s => s.Date.Date)
                                    .FirstOrDefault();

                return new WorkScheduleModel
                {
                    Id = dto.EmployeeId, 
                    EmployeeId = dto.EmployeeCode,
                    EmployeeName = dto.EmployeeName,
                    Department = dto.Department,
                    Avatar = dto.AvatarUrl,
                    CurrentShiftId = currentShift?.ShiftId ?? "",
                    CurrentShift = currentShift?.ShiftName ?? (currentShift?.Status == "Holiday" ? "Nghỉ lễ" : (currentShift?.Status == "Leave" ? "Nghỉ phép" : "Chưa xếp ca")),
                    ShiftColor = $"badge-light-{currentShift?.ShiftColor ?? "secondary"}",
                    BulletBg = $"bg-{currentShift?.ShiftColor ?? "secondary"}",
                    
                    IsNextShiftAssigned = nextShift != null,
                    NextShift = nextShift?.ShiftName ?? "",
                    
                    WeeklySchedule = dto.Schedules.OrderBy(s => s.Date).Select(s => s.ShiftCode ?? "-").ToList()
                };
            }).ToList();
        }

        public async Task<PagedResult<ShiftRequestModel>> GetShiftRequestsAsync(int page = 1, int pageSize = 10, string? status = null, string? searchTerm = null)
        {
            var url = $"{ApiEndpoints.Attendance.ShiftRequests}?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(status)) url += $"&status={status}";
            if (!string.IsNullOrEmpty(searchTerm)) url += $"&searchTerm={searchTerm}";

            var response = await _httpClient.GetFromJsonAsync<ApiResponse<PagedResult<ShiftRequestModel>>>(url);
            return response?.Data ?? new PagedResult<ShiftRequestModel>();
        }
        
        public async Task<ShiftRequestSummaryModel> GetShiftRequestSummaryAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<ShiftRequestSummaryModel>>($"{ApiEndpoints.Attendance.ShiftRequests}/summary");
            return response?.Data ?? new ShiftRequestSummaryModel();
        }

        public async Task<bool> ProcessShiftRequestAsync(string id, bool approved, string? note)
        {
            var url = $"{ApiEndpoints.Attendance.ShiftRequests}/{id}/process";
            var response = await _httpClient.PostAsJsonAsync(url, new { Approved = approved, Note = note });
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<AttendanceDetailModel>> GetAttendanceDetailsAsync(string employeeId, DateTime month)
        {
            var url = $"{ApiEndpoints.Attendance.Base}/employee/{employeeId}?month={month.Month}&year={month.Year}";
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<EmployeeAttendanceDetailDto>>(url);
            
            // Map Logs to AttendanceDetailModel
            return response?.Data?.Logs?.Select(l => new AttendanceDetailModel
            {
                Date = l.Date,
                DayOfWeek = l.Date.ToString("dddd", new System.Globalization.CultureInfo("vi-VN")),
                CheckIn = !string.IsNullOrEmpty(l.CheckIn) ? DateTime.ParseExact(l.CheckIn, "HH:mm", null) : null,
                CheckOut = !string.IsNullOrEmpty(l.CheckOut) ? DateTime.ParseExact(l.CheckOut, "HH:mm", null) : null,
                WorkValue = l.WorkUnits,
                LateMinutes = l.LateMinutes,
                EarlyLeaveMinutes = l.EarlyLeaveMinutes,
                OvertimeHours = l.OvertimeHours,
                Status = l.Status,
                IsLate = l.Status == "Late" || l.Status == "L",
                IsEarlyLeave = l.Status == "EarlyLeave" || l.Status == "E",
                Note = l.Note
            }) ?? new List<AttendanceDetailModel>();
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

        public async Task<ApiResponse<object>> CloseMonthlyAsync(int month, int year)
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Attendance.Base}/close-monthly", new { Month = month, Year = year });
            return await response.Content.ReadFromJsonAsync<ApiResponse<object>>() ?? new ApiResponse<object> { Success = false, Message = "Lỗi xử lý yêu cầu" };
        }

        public async Task<IEnumerable<WorkShiftModel>> GetWorkShiftsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<IEnumerable<WorkShiftModel>>>($"{ApiEndpoints.Attendance.Base}/shifts");
            return response?.Data ?? new List<WorkShiftModel>();
        }

        public async Task<bool> AssignScheduleAsync(AssignWorkScheduleModel model)
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Attendance.Base}/schedule/assign", model);
            return response.IsSuccessStatusCode;
        }
    }
}
