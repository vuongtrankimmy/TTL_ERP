using System;
using System.Collections.Generic;
using TTL.HR.Application.Modules.Common.Entities;

namespace TTL.HR.Application.Modules.Attendance.Entities
{
    public class AttendanceRecord : BaseEntity
    {
        public string? EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string? DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;

        public DateTime Date { get; set; } 
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        
        public string? ShiftId { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public string ShiftCode { get; set; } = string.Empty;

        public string Status { get; set; } = "Normal";
        public double WorkingHours { get; set; }
        public int LateMinutes { get; set; }
        public int EarlyLeaveMinutes { get; set; }
        public double OvertimeHours { get; set; }
        
        public string? CheckInLocation { get; set; }
        public string? CheckOutLocation { get; set; }
        public string? CheckInIp { get; set; }
        public string? CheckOutIp { get; set; }
        public string? Method { get; set; }
        public string? Source { get; set; }
        public string? DeviceName { get; set; }

        public string CheckInImageUrl { get; set; } = string.Empty;
        public string CheckOutImageUrl { get; set; } = string.Empty;
    }

    public class WorkShift : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; } 
        public TimeSpan EndTime { get; set; }
        public TimeSpan BreakStartTime { get; set; }
        public TimeSpan BreakEndTime { get; set; }
        public bool IsOvernight { get; set; }
        public List<DayOfWeek> WorkingDays { get; set; } = new();
    }
}
