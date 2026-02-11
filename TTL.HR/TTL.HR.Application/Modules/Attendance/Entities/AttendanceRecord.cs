using System;
using System.Collections.Generic;
using TTL.HR.Application.Modules.Common.Entities;

namespace TTL.HR.Application.Modules.Attendance.Entities
{
    public class AttendanceRecord : BaseEntity
    {
        public string EmployeeId { get; set; } = string.Empty;

        public DateTime Date { get; set; } 
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        
        public string CheckInLocation { get; set; } = string.Empty;
        public string CheckOutLocation { get; set; } = string.Empty;
        
        public string CheckInImageUrl { get; set; } = string.Empty;
        public string CheckOutImageUrl { get; set; } = string.Empty;
        
        public bool IsLate { get; set; }
        public bool IsEarlyLeave { get; set; }
        public double WorkingHours { get; set; }
        public string ShiftName { get; set; } = string.Empty;
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
