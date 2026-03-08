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
        public async Task<PagedResult<AttendanceModel>> GetTimesheetsAsync(int month = 0, int year = 0, string? departmentId = null, string? searchTerm = null, int page = 1, int pageSize = 10)
        {
            if (month == 0) month = DateTime.Now.Month;
            if (year == 0) year = DateTime.Now.Year;

            var url = $"{ApiEndpoints.Attendance.Timesheets}?month={month}&year={year}&page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(departmentId)) url += $"&departmentId={departmentId}";
            if (!string.IsNullOrEmpty(searchTerm)) url += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";
            
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<PagedResult<AttendanceModel>>>(url);
            return response?.Data ?? new PagedResult<AttendanceModel>();
        }

        public async Task<IEnumerable<WorkScheduleModel>> GetWorkSchedulesAsync(DateTime? startDate = null, DateTime? endDate = null, int pageSize = 1000)
        {
            var url = $"{ApiEndpoints.Attendance.WorkSchedules}?pageSize={pageSize}";
            if (startDate.HasValue) url += $"&startDate={startDate.Value:yyyy-MM-dd}";
            if (endDate.HasValue) url += $"&endDate={endDate.Value:yyyy-MM-dd}";

            var response = await _httpClient.GetFromJsonAsync<ApiResponse<PagedResult<EmployeeScheduleDto>>>(url);
            var dtos = response?.Data?.Items ?? new List<EmployeeScheduleDto>();

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            
            // If the requested range doesn't include today, we use the first day of the range as "Today" for display purposes
            var displayDate = (startDate.HasValue && (today < startDate.Value || (endDate.HasValue && today > endDate.Value))) ? startDate.Value : today;
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
                    EmployeeId = dto.EmployeeId,
                    EmployeeCode = dto.EmployeeCode,
                    EmployeeName = dto.EmployeeName,
                    Department = dto.Department,
                    Avatar = dto.AvatarUrl,
                    CurrentShiftId = currentShift?.ShiftId ?? "",
                    CurrentShift = currentShift?.ShiftName ?? (currentShift?.Status == "Holiday" ? "Nghỉ lễ" : (currentShift?.Status == "Leave" ? "Nghỉ phép" : "Chưa xếp ca")),
                    ShiftColor = currentShift?.ShiftColor ?? "secondary",
                    BulletBg = currentShift?.ShiftColor ?? "secondary",
                    
                    IsNextShiftAssigned = nextShift != null,
                    NextShift = nextShift?.ShiftName ?? "",
                    
                    WeeklySchedule = dto.Schedules.OrderBy(s => s.Date).Select(s => s.ShiftCode ?? "-").ToList()
                };
            }).ToList();
        }

        public async Task<IEnumerable<EmployeeScheduleDto>> GetMonthlyWorkSchedulesAsync(DateTime startDate, DateTime endDate, int pageSize = 1000)
        {
            var url = $"{ApiEndpoints.Attendance.WorkSchedules}?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}&pageSize={pageSize}";
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<PagedResult<EmployeeScheduleDto>>>(url);
            return response?.Data?.Items ?? new List<EmployeeScheduleDto>();
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

        public async Task<bool> CheckOutAsync(AttendanceModel attendance)
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Attendance.Base}/check-out", attendance);
            return response.IsSuccessStatusCode;
        }

        public async Task<ApiResponse<object>> CloseMonthlyAsync(int month, int year, string? employeeId = null)
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Attendance.Base}/close-monthly", new { Month = month, Year = year, EmployeeId = employeeId });
            return await response.Content.ReadFromJsonAsync<ApiResponse<object>>() ?? new ApiResponse<object> { Success = false, Message = "Lỗi xử lý yêu cầu" };
        }

        public async Task<PagedResult<AttendanceModel>> GetAttendanceListAsync(int page = 1, int pageSize = 10, string? searchTerm = null, DateTime? date = null, string? status = null, string? orderBy = null)
        {
            var url = $"{ApiEndpoints.Attendance.Base}?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(searchTerm)) url += $"&searchTerm={searchTerm}";
            if (date.HasValue) url += $"&date={date.Value:yyyy-MM-dd}";
            if (!string.IsNullOrEmpty(status)) url += $"&status={status}";
            if (!string.IsNullOrEmpty(orderBy)) url += $"&orderBy={orderBy}";

            var response = await _httpClient.GetFromJsonAsync<ApiResponse<PagedResult<AttendanceModel>>>(url);
            return response?.Data ?? new PagedResult<AttendanceModel>();
        }

        public async Task<ApiResponse<ImportAttendanceResultModel>> ImportAttendanceAsync(string? rawData, byte[]? fileBytes, string? fileName, string source, int codeCol = 1, int timeCol = 2, bool isPreview = false)
        {
            try
            {
                var content = new MultipartFormDataContent();
                if (!string.IsNullOrEmpty(rawData)) content.Add(new StringContent(rawData), "RawData");
                content.Add(new StringContent(source), "Source");
                content.Add(new StringContent(codeCol.ToString()), "EmployeeCodeColumnIndex");
                content.Add(new StringContent(timeCol.ToString()), "TimestampColumnIndex");
                content.Add(new StringContent(isPreview.ToString()), "IsPreviewOnly");

                if (fileBytes != null && !string.IsNullOrEmpty(fileName))
                {
                    var fileContent = new ByteArrayContent(fileBytes);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    content.Add(fileContent, "File", fileName);
                }

                var response = await _httpClient.PostAsync($"{ApiEndpoints.Attendance.Base}/import", content);
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<ImportAttendanceResultModel>>();
                return result ?? new ApiResponse<ImportAttendanceResultModel> { Success = false, Message = "Lỗi phản hồi từ server" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ImportAttendanceResultModel> { Success = false, Message = $"Lỗi kết nối: {ex.Message}" };
            }
        }

        public async Task<IEnumerable<WorkShiftModel>> GetWorkShiftsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<IEnumerable<WorkShiftModel>>>(ApiEndpoints.Attendance.Shifts);
            return response?.Data ?? new List<WorkShiftModel>();
        }
        
        public async Task<WorkShiftModel?> GetWorkShiftByIdAsync(string id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<WorkShiftModel>>($"{ApiEndpoints.Attendance.Shifts}/{id}");
            return response?.Data;
        }

        public async Task<ApiResponse<string>> CreateWorkShiftAsync(WorkShiftModel model)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Attendance.Shifts, model);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
            return new ApiResponse<string> 
            { 
                Success = result?.Success ?? false, 
                Message = result?.Message ?? "Lỗi phản hồi từ server",
                Data = result?.Data?.ToString()
            };
        }

        public async Task<ApiResponse<bool>> UpdateWorkShiftAsync(string id, WorkShiftModel model)
        {
            var response = await _httpClient.PutAsJsonAsync(ApiEndpoints.Attendance.Shifts, model);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
            return new ApiResponse<bool> 
            { 
                Success = result?.Success ?? false, 
                Message = result?.Message ?? "Lỗi phản hồi từ server",
                Data = result?.Success ?? false
            };
        }

        public async Task<ApiResponse<bool>> DeleteWorkShiftAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiEndpoints.Attendance.Shifts}/{id}");
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
            return new ApiResponse<bool> 
            { 
                Success = result?.Success ?? false, 
                Message = result?.Message ?? "Lỗi phản hồi từ server",
                Data = result?.Success ?? false
            };
        }

        public async Task<bool> AssignScheduleAsync(AssignWorkScheduleModel model)
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Attendance.Base}/schedule/assign", model);
            return response.IsSuccessStatusCode;
        }

        public async Task<ApiResponse<bool>> CreateShiftRequestAsync(CreateShiftRequestModel model)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Attendance.ShiftRequests, model);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
            return new ApiResponse<bool>
            {
                Success = result?.Success ?? false,
                Message = result?.Message ?? "Lỗi phản hồi từ server",
                Data = result?.Success ?? false
            };
        }

        public async Task<EmployeeStatsModel> GetEmployeeStatsAsync(string employeeId, int month, int year)
        {
            var url = $"{ApiEndpoints.Attendance.Base}/employee/{employeeId}/stats?month={month}&year={year}";
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<EmployeeStatsModel>>(url);
            return response?.Data ?? new EmployeeStatsModel { EmployeeId = employeeId };
        }

        public async Task<bool> WithdrawShiftRequestAsync(string id)
        {
            var url = $"{ApiEndpoints.Attendance.ShiftRequests}/{id}/withdraw";
            var response = await _httpClient.PostAsJsonAsync(url, string.Empty);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteShiftRequestAsync(string id)
        {
            var url = $"{ApiEndpoints.Attendance.ShiftRequests}/{id}";
            var response = await _httpClient.DeleteAsync(url);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RecalculateAttendanceSummaryAsync(string employeeId, int month, int year)
        {
            var url = $"{ApiEndpoints.Attendance.Base}/recalculate-summary";
            var response = await _httpClient.PostAsJsonAsync(url, new { employeeId, month, year });
            return response.IsSuccessStatusCode;
        }

        public async Task<byte[]?> ExportTimesheetAsync(int month, int year, string? searchTerm = null, string? departmentId = null)
        {
            var url = $"{ApiEndpoints.Attendance.ExportTimesheet}?month={month}&year={year}";
            if (!string.IsNullOrEmpty(searchTerm)) url += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";
            if (!string.IsNullOrEmpty(departmentId)) url += $"&departmentId={departmentId}";

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            return null;
        }
    }
}
