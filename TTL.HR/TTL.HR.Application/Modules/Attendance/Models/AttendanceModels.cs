using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace TTL.HR.Application.Modules.Attendance.Models
{
    public class AttendanceModel
    {
        public string Id { get; set; } = "";
        public string EmployeeId { get; set; } = "";
        public string EmployeeCode { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public string Avatar { get; set; } = "";

        public DateTime Date { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        
        public string ShiftName { get; set; } = "";
        public string ShiftColor { get; set; } = "primary";
        public string Status { get; set; } = "";
        public int StatusId { get; set; } = 1;
        public string StatusColor { get; set; } = "primary";
        
        public double TotalWorkingHours { get; set; }

        public int LateMinutes { get; set; }
        public int EarlyLeaveMinutes { get; set; }

        public double OvertimeHours { get; set; }

        public string Role { get; set; } = "";
        public string Department { get; set; } = "";
        public double StandardWork { get; set; }
        public double ActualWork { get; set; }
        public int LateCount { get; set; }
        public int EarlyLeaveCount { get; set; }
        public int LeaveDays { get; set; }
        public int HolidayDays { get; set; }
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
        [JsonProperty("employeeCode")]
        public string EmployeeCode { get; set; } = "";
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
        
        [JsonProperty("employeeId")]
        public string EmployeeId { get; set; } = "";
        
        [JsonProperty("employeeName")]
        public string EmployeeName { get; set; } = "";
        
        [JsonProperty("employeeCode")]
        public string EmployeeCode { get; set; } = "";
        
        [JsonProperty("departmentName")]
        public string Department { get; set; } = "";
        
        [JsonProperty("avatarUrl")]
        public string Avatar { get; set; } = "";
        
        [JsonProperty("fromShiftName")]
        public string CurrentShift { get; set; } = "";

        [JsonProperty("fromShiftColor")]
        public string FromShiftColor { get; set; } = "info";
        
        [JsonProperty("toShiftName")]
        public string TargetShift { get; set; } = "";
        
        [JsonProperty("toShiftColor")]
        public string ToShiftColor { get; set; } = "primary";
        
        [JsonProperty("reason")]
        public string Reason { get; set; } = "";
        
        [JsonProperty("date")]
        public DateTime RequestedDate { get; set; }
        
        [JsonProperty("status")]
        public string Status { get; set; } = "Pending";
        
        public bool IsPending => Status != "Approved" && Status != "2" && Status != "Rejected" && Status != "3" && Status != "Withdrawn" && Status != "8" && Status != "Cancelled" && Status != "4";
        
        [JsonProperty("comment")]
        public string? ManagerNote { get; set; }

        // Multi-level Approval
        [JsonProperty("currentLevel")]
        public int CurrentLevel { get; set; }
        
        [JsonProperty("totalLevelsRequired")]
        public int TotalLevelsRequired { get; set; }
        
        [JsonProperty("pendingApproverId")]
        public string? PendingApproverId { get; set; }
        
        [JsonProperty("nextApproverId")]
        public string? NextApproverId { get; set; }

        [JsonProperty("statusColor")]
        public string StatusColor { get; set; } = "secondary";
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
        public int CancelledCount { get; set; }
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
        public bool IsFlexible { get; set; }
        public bool IsDelete { get; set; }
        public bool BypassQueue { get; set; } = true;
    }

    public class WorkShiftModel
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Code { get; set; } = "";
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public string? BreakStartTime { get; set; }
        public string? BreakEndTime { get; set; }
        public bool IsOvernight { get; set; }
        public int BreakMinutes { get; set; } = 60;
        public double TotalHours { get; set; } = 8.0;
        public bool IsFlexible { get; set; }
        public string Color { get; set; } = "primary";
        public int AssignedCount { get; set; }
        public List<DayOfWeek> WorkingDays { get; set; } = new();
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    // DTOs for API Response
    public class EmployeeScheduleDto
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
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
        public bool IsFlexible { get; set; }
    }

    public class CreateShiftRequestModel
    {
        public string EmployeeId { get; set; } = "";
        public DateTime Date { get; set; }
        public string ToShiftId { get; set; } = "";
        public string Reason { get; set; } = "";
        public string? ApproverId { get; set; }
        public bool AutoApprove { get; set; }
    }

    public class EmployeeStatsModel
    {
        [JsonProperty("employeeId")]
        [JsonPropertyName("employeeId")]
        public string EmployeeId { get; set; } = "";
        
        [JsonProperty("totalEntitledLeave")]
        [JsonPropertyName("totalEntitledLeave")]
        public double TotalEntitledLeave { get; set; }
        
        [JsonProperty("usedLeaveYear")]
        [JsonPropertyName("usedLeaveYear")]
        public double UsedLeaveYear { get; set; }
        
        [JsonProperty("usedLeaveMonth")]
        [JsonPropertyName("usedLeaveMonth")]
        public double UsedLeaveMonth { get; set; }
        
        [JsonProperty("remainingLeave")]
        [JsonPropertyName("remainingLeave")]
        public double RemainingLeave { get; set; }
        
        [JsonProperty("totalWorkingHoursMonth")]
        [JsonPropertyName("totalWorkingHoursMonth")]
        public double TotalWorkingHoursMonth { get; set; }
    }
}
