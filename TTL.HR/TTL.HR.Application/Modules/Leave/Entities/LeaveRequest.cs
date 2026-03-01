using System;
using TTL.HR.Application.Modules.Common.Entities;

namespace TTL.HR.Application.Modules.Leave.Entities
{
    public class LeaveRequestEntity : BaseEntity
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;

        public string LeaveTypeId { get; set; } = string.Empty;
        public string LeaveTypeName { get; set; } = string.Empty;
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double TotalDays { get; set; }
        
        public string Reason { get; set; } = string.Empty;
        public List<LeaveAttachment> Attachments { get; set; } = new();
        
        public string Status { get; set; } = "Pending";
        
        public int CurrentLevel { get; set; }
        public int TotalLevelsRequired { get; set; }
        public List<LeaveApprovalStep> ApprovalHistory { get; set; } = new();
        
        // Legacy compatibility
        public string? ApproverId { get; set; }
        public string? ApproverComment { get; set; }
        public DateTime? ApprovedAt { get; set; }
    }

    public class LeaveAttachment
    {
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
    }

    public class LeaveApprovalStep
    {
        public int Level { get; set; }
        public string ApproverId { get; set; } = string.Empty;
        public string ApproverName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Comment { get; set; }
        public DateTime ActionAt { get; set; }
    }

    public enum ApprovalStatus
    {
        Pending,
        Approved,
        Rejected,
        Cancelled
    }
}
