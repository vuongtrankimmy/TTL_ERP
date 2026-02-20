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
        public int LateMinutes { get; set; }
        public int EarlyLeaveCount { get; set; }
        public int EarlyLeaveMinutes { get; set; }
        public int LeaveDays { get; set; }
        public int HolidayDays { get; set; }
        public double OvertimeHours { get; set; }
        public double TotalWorkingHours { get; set; }
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
        public int LateMinutes { get; set; }
        public int EarlyLeaveMinutes { get; set; }
        public double OvertimeHours { get; set; }
        public bool IsLate { get; set; }
        public bool IsEarlyLeave { get; set; }
        public string Status { get; set; } = "";
        public string? Note { get; set; }
    }
    public class WorkScheduleModel
    {
        public string Id { get; set; } = "";
        public string EmployeeId { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public string Department { get; set; } = "";
        public string Avatar { get; set; } = "";
        public string CurrentShiftId { get; set; } = "";
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
        
        [System.Text.Json.Serialization.JsonPropertyName("employeeId")]
        public string EmployeeId { get; set; } = "";
        
        [System.Text.Json.Serialization.JsonPropertyName("employeeName")]
        public string EmployeeName { get; set; } = "";
        
        [System.Text.Json.Serialization.JsonPropertyName("departmentName")]
        public string Department { get; set; } = "";
        
        [System.Text.Json.Serialization.JsonPropertyName("avatarUrl")]
        public string Avatar { get; set; } = "";
        
        [System.Text.Json.Serialization.JsonPropertyName("fromShiftName")]
        public string CurrentShift { get; set; } = "";
        
        public string CurrentShiftClass => "badge-light-info";
        
        [System.Text.Json.Serialization.JsonPropertyName("toShiftName")]
        public string TargetShift { get; set; } = "";
        
        public string TargetShiftClass => "badge-light-primary";
        
        [System.Text.Json.Serialization.JsonPropertyName("reason")]
        public string Reason { get; set; } = "";
        
        [System.Text.Json.Serialization.JsonPropertyName("date")]
        public DateTime RequestedDate { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("status")]
        public string Status { get; set; } = "Pending";
        
        public bool IsPending => Status == "Pending" || Status == "Chờ duyệt";
        
        [System.Text.Json.Serialization.JsonPropertyName("comment")]
        public string? ManagerNote { get; set; }

        public string StatusColor => Status switch
        {
            "Approved" => "success",
            "Rejected" => "danger",
            "Pending" => "warning",
            _ => "secondary"
        };
    }
    public class EmployeeAttendanceDetailDto
    {
        public string EmployeeId { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public List<AttendanceLogDto> Logs { get; set; } = new();
    }

    public class AttendanceLogDto
    {
        public DateTime Date { get; set; }
        public string? CheckIn { get; set; }
        public string? CheckOut { get; set; }
        public double WorkUnits { get; set; }
        public int LateMinutes { get; set; }
        public int EarlyLeaveMinutes { get; set; }
        public double OvertimeHours { get; set; }
        public string Status { get; set; } = "";
        public string? Note { get; set; }
    }

    public class ShiftRequestSummaryModel
    {
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public int TotalCount { get; set; }
    }

    public class AssignWorkScheduleModel
    {
        public List<string> EmployeeIds { get; set; } = new();
        public string ShiftId { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(1);
        public List<DayOfWeek> DaysOfWeek { get; set; } = new() { 
            DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, 
            DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday 
        };
        public string? Note { get; set; }
    }

    public class WorkShiftModel
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Code { get; set; } = "";
        public string StartTime { get; set; } = "";
        public string EndTime { get; set; } = "";
        public string Color { get; set; } = "primary";
    }

    // DTOs for API Response
    public class EmployeeScheduleDto
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public List<ScheduleDayDto> Schedules { get; set; } = new();
    }

    public class ScheduleDayDto
    {
        public DateTime Date { get; set; }
        public string? ShiftId { get; set; }
        public string? ShiftName { get; set; }
        public string? ShiftCode { get; set; }
        public string ShiftColor { get; set; } = "secondary";
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
