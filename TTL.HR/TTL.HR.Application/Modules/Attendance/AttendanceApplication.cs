using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Attendance.Interfaces;
using TTL.HR.Application.Modules.Attendance.Models;

namespace TTL.HR.Application.Modules.Attendance
{
    public interface IAttendanceApplication
    {
        Task<IEnumerable<AttendanceModel>> GetCurrentTimesheetsAsync();
        Task<bool> PerformCheckInAsync(AttendanceModel attendance);
        Task<bool> PerformCheckOutAsync(string id, AttendanceModel attendance);
        Task<AttendanceOverview> GetAttendanceOverviewAsync();
    }

    public class AttendanceApplication : IAttendanceApplication
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceApplication(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        public async Task<IEnumerable<AttendanceModel>> GetCurrentTimesheetsAsync()
        {
            return await _attendanceService.GetTimesheetsAsync();
        }

        public async Task<bool> PerformCheckInAsync(AttendanceModel attendance)
        {
            if (attendance.Date.TimeOfDay > new TimeSpan(8, 30, 0))
            {
                attendance.Status = "Muộn";
            }
            else
            {
                attendance.Status = "Đúng giờ";
            }

            return await _attendanceService.CheckInAsync(attendance);
        }

        public async Task<bool> PerformCheckOutAsync(string id, AttendanceModel attendance)
        {
            return await _attendanceService.CheckOutAsync(id, attendance);
        }

        public async Task<AttendanceOverview> GetAttendanceOverviewAsync()
        {
            var data = await _attendanceService.GetTimesheetsAsync();
            var list = data.ToList();
            
            return new AttendanceOverview
            {
                TotalDays = list.Count,
                OnTimeCount = list.Count(x => x.Status == "Đúng giờ"),
                LateCount = list.Count(x => x.Status == "Muộn"),
                AbsentCount = 0
            };
        }
    }

    public class AttendanceOverview
    {
        public int TotalDays { get; set; }
        public int OnTimeCount { get; set; }
        public int LateCount { get; set; }
        public int AbsentCount { get; set; }
    }
}
