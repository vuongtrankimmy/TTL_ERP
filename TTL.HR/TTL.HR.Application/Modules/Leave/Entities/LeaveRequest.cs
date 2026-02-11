using System;
using TTL.HR.Application.Modules.Common.Entities;

namespace TTL.HR.Application.Modules.Leave.Entities
{
    public class LeaveRequestEntity : BaseEntity
    {
        public string EmployeeId { get; set; } = string.Empty;
        
        public LeaveType Type { get; set; } = LeaveType.Annual;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public double TotalDays { get; set; }
        
        public string Reason { get; set; } = string.Empty;
        public string AttachmentUrl { get; set; } = string.Empty;
        
        public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;
        
        public string? ApproverId { get; set; }
        public string? ApproverComment { get; set; }
        public DateTime? ApprovedAt { get; set; }
    }

    public enum LeaveType
    {
        Annual,
        Sick,
        Maternity,
        Personal,
        Unpaid,
        Compensatory
    }

    public enum ApprovalStatus
    {
        Pending,
        Approved,
        Rejected,
        Cancelled
    }
}
