using Newtonsoft.Json;
using System.Text.Json.Serialization;


namespace TTL.HR.Application.Modules.Leave.Models
{
    public class LeaveRequestModel
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
        
        [JsonProperty("employeeAvatar")]
        public string Avatar { get; set; } = "";
        
        [JsonProperty("leaveTypeId")]
        public string LeaveTypeId { get; set; } = "";
        
        [JsonProperty("leaveTypeName")]
        public string Type { get; set; } = "";
        
        [JsonProperty("leaveTypeColor")]
        public string TypeColor { get; set; } = "primary";
        
        public string SubType { get; set; } = "";
        
        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }
        
        [JsonProperty("endDate")]
        public DateTime EndDate { get; set; }
        
        [JsonProperty("totalDays")]
        public double TotalDays { get; set; }
        
        [JsonProperty("status")]
        public string Status { get; set; } = "";
        
        [JsonProperty("reason")]
        public string Reason { get; set; } = "";
        
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
        
        [JsonProperty("approvalHistory")]
        public List<LeaveApprovalStepModel> ApprovalHistory { get; set; } = new();
        
        [JsonProperty("attachments")]
        public List<LeaveAttachmentModel> Attachments { get; set; } = new();
        
        public bool IsPending => Status == "Pending" || Status == "PartiallyApproved" || Status == "Chờ phê duyệt" || Status == "Chờ duyệt";
        
        [JsonProperty("statusColor")]
        public string StatusColor { get; set; } = "secondary";

        public string StatusBadgeClass => $"badge-light-{StatusColor}";
    }

    public class LeaveAttachmentModel
    {
        [JsonProperty("fileName")]
        public string FileName { get; set; } = string.Empty;
        
        [JsonProperty("fileUrl")]
        public string FileUrl { get; set; } = string.Empty;
    }

    public class LeaveApprovalStepModel
    {
        [JsonProperty("level")]
        public int Level { get; set; }
        
        [JsonProperty("approverName")]
        public string ApproverName { get; set; } = string.Empty;
        
        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;
        
        [JsonProperty("comment")]
        public string? Comment { get; set; }
        
        [JsonProperty("actionAt")]
        public DateTime ActionAt { get; set; }
    }

    public class LeaveStateSummaryModel
    {
        [JsonProperty("pendingCount")]
        [JsonPropertyName("pendingCount")]
        public int PendingCount { get; set; }

        [JsonProperty("approvedCount")]
        [JsonPropertyName("approvedCount")]
        public int ApprovedCount { get; set; }

        [JsonProperty("rejectedCount")]
        [JsonPropertyName("rejectedCount")]
        public int RejectedCount { get; set; }

        [JsonProperty("cancelledCount")]
        [JsonPropertyName("cancelledCount")]
        public int CancelledCount { get; set; }

        [JsonProperty("assignedToMeCount")]
        [JsonPropertyName("assignedToMeCount")]
        public int AssignedToMeCount { get; set; }

        [JsonProperty("totalCount")]
        [JsonPropertyName("totalCount")]
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
