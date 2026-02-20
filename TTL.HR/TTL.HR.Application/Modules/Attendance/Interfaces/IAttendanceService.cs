using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Attendance.Models;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Application.Modules.Attendance.Interfaces
{
    public interface IAttendanceService
    {
        Task<IEnumerable<AttendanceModel>> GetTimesheetsAsync(int month = 0, int year = 0, string? departmentId = null, string? searchTerm = null);
        Task<IEnumerable<WorkScheduleModel>> GetWorkSchedulesAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<PagedResult<ShiftRequestModel>> GetShiftRequestsAsync(int page = 1, int pageSize = 10, string? status = null, string? searchTerm = null);
        Task<ShiftRequestSummaryModel> GetShiftRequestSummaryAsync();
        Task<bool> ProcessShiftRequestAsync(string id, bool approved, string? note);
        Task<IEnumerable<AttendanceDetailModel>> GetAttendanceDetailsAsync(string employeeId, DateTime month);
        Task<bool> CheckInAsync(AttendanceModel attendance);
        Task<bool> CheckOutAsync(string id, AttendanceModel attendance);
        Task<IEnumerable<WorkShiftModel>> GetWorkShiftsAsync();
        Task<bool> AssignScheduleAsync(AssignWorkScheduleModel model);
        Task<ApiResponse<object>> CloseMonthlyAsync(int month, int year);
    }
}
