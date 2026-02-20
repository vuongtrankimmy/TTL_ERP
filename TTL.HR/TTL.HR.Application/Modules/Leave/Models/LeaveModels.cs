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
        
        [System.Text.Json.Serialization.JsonPropertyName("departmentName")]
        public string Department { get; set; } = "";
        
        [System.Text.Json.Serialization.JsonPropertyName("employeeAvatar")]
        public string Avatar { get; set; } = "";
        
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
        
        public bool IsPending => Status == "Pending" || Status == "Chờ phê duyệt" || Status == "Chờ duyệt";
        
        public string StatusColor => Status switch
        {
            "Approved" => "success",
            "Rejected" => "danger",
            "Pending" => "warning",
            "Cancelled" => "secondary",
            _ => "secondary"
        };

        public string StatusBadgeClass => $"badge-light-{StatusColor}";
    }

    public class LeaveStateSummaryModel
    {
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public int TotalCount { get; set; }
    }
}
