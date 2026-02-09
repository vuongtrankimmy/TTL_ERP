using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TTL.HR.Shared.Entities.Base;

namespace TTL.HR.Shared.Entities.Attendance
{
    public class AttendanceRecord : BaseEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string EmployeeId { get; set; } = string.Empty;

        public DateTime Date { get; set; } // yyyy-MM-dd (Time part zeroed)
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        
        public string CheckInLocation { get; set; } = string.Empty; // GPS or "Office"
        public string CheckOutLocation { get; set; } = string.Empty;
        
        public string CheckInImageUrl { get; set; } = string.Empty; // Proof
        public string CheckOutImageUrl { get; set; } = string.Empty;
        
        public bool IsLate { get; set; }
        public bool IsEarlyLeave { get; set; }
        public double WorkingHours { get; set; }
        public string ShiftName { get; set; } = string.Empty; // Logged shift
    }

    public class LeaveRequest : BaseEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string EmployeeId { get; set; } = string.Empty;
        
        public LeaveType Type { get; set; } = LeaveType.Annual;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public double TotalDays { get; set; }
        
        public string Reason { get; set; } = string.Empty;
        public string AttachmentUrl { get; set; } = string.Empty; // Medical cert, etc.
        
        public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ApproverId { get; set; }
        public string? ApproverComment { get; set; }
        public DateTime? ApprovedAt { get; set; }
    }

    public class WorkShift : BaseEntity
    {
        public string Name { get; set; } = string.Empty; // "Hành chính", "Ca 1", "Ca 2"
        public string Code { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; } 
        public TimeSpan EndTime { get; set; }
        public TimeSpan BreakStartTime { get; set; }
        public TimeSpan BreakEndTime { get; set; }
        public bool IsOvernight { get; set; } // Requires complicated logic crossing midnight
        public List<DayOfWeek> WorkingDays { get; set; } = new(); // [Monday, Tuesday...]
    }

    public enum LeaveType
    {
        Annual, // Phép năm
        Sick, // Ốm đau
        Maternity, // Thai sản
        Personal, // Việc riêng
        Unpaid, // Không lương
        Compensatory // Nghỉ bù
    }

    public enum ApprovalStatus
    {
        Pending,
        Approved,
        Rejected,
        Cancelled
    }
}
