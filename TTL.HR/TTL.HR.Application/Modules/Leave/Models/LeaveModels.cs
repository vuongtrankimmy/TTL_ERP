using System;

namespace TTL.HR.Application.Modules.Leave.Models
{
    public class LeaveRequestModel
    {
        public string Id { get; set; } = "";
        
        [System.Text.Json.Serialization.JsonPropertyName("employeeId")]
        public string EmployeeId { get; set; } = "";
        
        [System.Text.Json.Serialization.JsonPropertyName("employeeName")]
        public string EmployeeName { get; set; } = "";
        
        [System.Text.Json.Serialization.JsonPropertyName("employeeCode")]
        public string EmployeeCode { get; set; } = "";
        
        [System.Text.Json.Serialization.JsonPropertyName("departmentName")]
        public string Department { get; set; } = "";
        
        [System.Text.Json.Serialization.JsonPropertyName("employeeAvatar")]
        public string Avatar { get; set; } = "";
        
        [System.Text.Json.Serialization.JsonPropertyName("leaveTypeId")]
        public string LeaveTypeId { get; set; } = "";
        
        [System.Text.Json.Serialization.JsonPropertyName("leaveTypeName")]
        public string Type { get; set; } = "";
        
        [System.Text.Json.Serialization.JsonPropertyName("leaveTypeColor")]
        public string TypeColor { get; set; } = "primary";
        
        public string SubType { get; set; } = "";
        
        [System.Text.Json.Serialization.JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("endDate")]
        public DateTime EndDate { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("totalDays")]
        public double TotalDays { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("status")]
        public string Status { get; set; } = "";
        
        [System.Text.Json.Serialization.JsonPropertyName("reason")]
        public string Reason { get; set; } = "";
        
        [System.Text.Json.Serialization.JsonPropertyName("comment")]
        public string? ManagerNote { get; set; }
        
        // Multi-level Approval
        [System.Text.Json.Serialization.JsonPropertyName("currentLevel")]
        public int CurrentLevel { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("totalLevelsRequired")]
        public int TotalLevelsRequired { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("pendingApproverId")]
        public string? PendingApproverId { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("nextApproverId")]
        public string? NextApproverId { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("approvalHistory")]
        public List<LeaveApprovalStepModel> ApprovalHistory { get; set; } = new();
        
        [System.Text.Json.Serialization.JsonPropertyName("attachments")]
        public List<LeaveAttachmentModel> Attachments { get; set; } = new();
        
        public bool IsPending => Status == "Pending" || Status == "PartiallyApproved" || Status == "Chờ phê duyệt" || Status == "Chờ duyệt";
        
        public string StatusColor => Status switch
        {
            "Approved" => "success",
            "PartiallyApproved" => "info",
            "Rejected" => "danger",
            "Pending" => "warning",
            "Cancelled" => "secondary",
            _ => "secondary"
        };

        public string StatusBadgeClass => $"badge-light-{StatusColor}";
    }

    public class LeaveAttachmentModel
    {
        [System.Text.Json.Serialization.JsonPropertyName("fileName")]
        public string FileName { get; set; } = string.Empty;
        
        [System.Text.Json.Serialization.JsonPropertyName("fileUrl")]
        public string FileUrl { get; set; } = string.Empty;
    }

    public class LeaveApprovalStepModel
    {
        [System.Text.Json.Serialization.JsonPropertyName("level")]
        public int Level { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("approverName")]
        public string ApproverName { get; set; } = string.Empty;
        
        [System.Text.Json.Serialization.JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
        
        [System.Text.Json.Serialization.JsonPropertyName("comment")]
        public string? Comment { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("actionAt")]
        public DateTime ActionAt { get; set; }
    }

    public class LeaveStateSummaryModel
    {
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public int CancelledCount { get; set; }
        public int AssignedToMeCount { get; set; }
        public int TotalCount { get; set; }
    }

    public class LeaveBalanceModel
    {
        public string EmployeeId { get; set; } = "";
        public int Year { get; set; }
        public double EntitledDays { get; set; }
        public double UsedDays { get; set; }
        public double RemainingDays { get; set; }
        public double PendingDays { get; set; }
    }

    public class LeaveTypeDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
        public string Color { get; set; } = string.Empty;
    }
}
