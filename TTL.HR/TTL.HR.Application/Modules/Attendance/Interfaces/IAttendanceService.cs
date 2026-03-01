using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Attendance.Models;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Application.Modules.Attendance.Interfaces
{
    public interface IAttendanceService
    {
        Task<PagedResult<AttendanceModel>> GetTimesheetsAsync(int month = 0, int year = 0, string? departmentId = null, string? searchTerm = null, int page = 1, int pageSize = 10);
        Task<IEnumerable<WorkScheduleModel>> GetWorkSchedulesAsync(DateTime? startDate = null, DateTime? endDate = null, int pageSize = 1000);
        Task<IEnumerable<EmployeeScheduleDto>> GetMonthlyWorkSchedulesAsync(DateTime startDate, DateTime endDate, int pageSize = 1000);
        Task<PagedResult<ShiftRequestModel>> GetShiftRequestsAsync(int page = 1, int pageSize = 10, string? status = null, string? searchTerm = null);
        Task<ShiftRequestSummaryModel> GetShiftRequestSummaryAsync();
        Task<bool> ProcessShiftRequestAsync(string id, bool approved, string? note);
        Task<IEnumerable<AttendanceDetailModel>> GetAttendanceDetailsAsync(string employeeId, DateTime month);
        Task<bool> CheckInAsync(AttendanceModel attendance);
        Task<bool> CheckOutAsync(AttendanceModel attendance);
        Task<IEnumerable<WorkShiftModel>> GetWorkShiftsAsync();
        Task<WorkShiftModel?> GetWorkShiftByIdAsync(string id);
        Task<ApiResponse<string>> CreateWorkShiftAsync(WorkShiftModel model);
        Task<ApiResponse<bool>> UpdateWorkShiftAsync(string id, WorkShiftModel model);
        Task<ApiResponse<bool>> DeleteWorkShiftAsync(string id);
        Task<bool> AssignScheduleAsync(AssignWorkScheduleModel model);
        Task<ApiResponse<object>> CloseMonthlyAsync(int month, int year);
        Task<PagedResult<AttendanceModel>> GetAttendanceListAsync(int page = 1, int pageSize = 10, string? searchTerm = null, DateTime? date = null, string? status = null, string? orderBy = null);
        Task<ApiResponse<ImportAttendanceResultModel>> ImportAttendanceAsync(string? rawData, byte[]? fileBytes, string? fileName, string source, int codeCol = 1, int timeCol = 2, bool isPreview = false);
        Task<bool> CreateShiftRequestAsync(CreateShiftRequestModel model);
        Task<EmployeeStatsModel> GetEmployeeStatsAsync(string employeeId, int month, int year);
        Task<bool> WithdrawShiftRequestAsync(string id);
        Task<bool> RecalculateAttendanceSummaryAsync(string employeeId, int month, int year);
    }
}
