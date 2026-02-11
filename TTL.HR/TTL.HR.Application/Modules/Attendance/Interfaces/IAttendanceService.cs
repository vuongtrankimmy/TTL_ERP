using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Attendance.Models;

namespace TTL.HR.Application.Modules.Attendance.Interfaces
{
    public interface IAttendanceService
    {
        Task<IEnumerable<AttendanceModel>> GetTimesheetsAsync();
        Task<IEnumerable<WorkScheduleModel>> GetWorkSchedulesAsync();
        Task<IEnumerable<ShiftRequestModel>> GetShiftRequestsAsync();
        Task<bool> ProcessShiftRequestAsync(string id, bool approved, string? note);
        Task<IEnumerable<AttendanceDetailModel>> GetAttendanceDetailsAsync(string employeeId, DateTime month);
        Task<bool> CheckInAsync(AttendanceModel attendance);
        Task<bool> CheckOutAsync(string id, AttendanceModel attendance);
    }
}
