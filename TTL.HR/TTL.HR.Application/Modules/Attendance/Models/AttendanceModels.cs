using System;

namespace TTL.HR.Application.Modules.Attendance.Models
{
    public class AttendanceModel
    {
        public string Id { get; set; } = "";
        public string EmployeeId { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public string Role { get; set; } = "";
        public string Department { get; set; } = "";
        public string Avatar { get; set; } = "";
        public double StandardWork { get; set; }
        public double ActualWork { get; set; }
        public int LateCount { get; set; }
        public int EarlyLeaveCount { get; set; }
        public int LeaveDays { get; set; }
        public int HolidayDays { get; set; }
        public double OvertimeHours { get; set; }
        public DateTime Date { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public string Status { get; set; } = "";
    }

    public class AttendanceDetailModel
    {
        public DateTime Date { get; set; }
        public string DayOfWeek { get; set; } = "";
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public double WorkValue { get; set; }
        public bool IsLate { get; set; }
        public bool IsEarlyLeave { get; set; }
        public string Status { get; set; } = "";
    }
    public class WorkScheduleModel
    {
        public string Id { get; set; } = "";
        public string EmployeeId { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public string Department { get; set; } = "";
        public string Avatar { get; set; } = "";
        public string CurrentShift { get; set; } = "";
        public string ShiftColor { get; set; } = "badge-light-primary";
        public string BulletBg { get; set; } = "bg-primary";
        public bool IsNextShiftAssigned { get; set; }
        public string NextShift { get; set; } = "";
        public List<string> WeeklySchedule { get; set; } = new();
    }

    public class ShiftRequestModel
    {
        public string Id { get; set; } = "";
        public string EmployeeId { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public string Department { get; set; } = "";
        public string Avatar { get; set; } = "";
        public string CurrentShift { get; set; } = "";
        public string CurrentShiftClass { get; set; } = "badge-light-info";
        public string TargetShift { get; set; } = "";
        public string TargetShiftClass { get; set; } = "badge-light-primary";
        public string Reason { get; set; } = "";
        public DateTime RequestedDate { get; set; }
        public string Status { get; set; } = "Pending";
        public bool IsPending => Status == "Pending" || Status == "Chờ duyệt";
        public string? ManagerNote { get; set; }
    }
}
