using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace TTL.HR.Application.Modules.Attendance.Models
{
    public class OvertimeRequestModel
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("employeeId")]
        public string EmployeeId { get; set; } = string.Empty;

        [JsonProperty("employeeName")]
        public string EmployeeName { get; set; } = string.Empty;

        [JsonProperty("employeeCode")]
        public string EmployeeCode { get; set; } = string.Empty;

        [JsonProperty("departmentName")]
        public string DepartmentName { get; set; } = string.Empty;

        [JsonProperty("avatarUrl")]
        public string AvatarUrl { get; set; } = string.Empty;

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("startTime")]
        public string StartTime { get; set; } = string.Empty;

        [JsonProperty("endTime")]
        public string EndTime { get; set; } = string.Empty;

        [JsonProperty("hours")]
        public double Hours { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; } = string.Empty;

        [JsonProperty("status")]
        public string Status { get; set; } = "Pending";

        [JsonProperty("statusId")]
        public int StatusId { get; set; } = 1;

        [JsonProperty("multiplier")]
        public double Multiplier { get; set; } = 1.5;

        [JsonProperty("managerNote")]
        public string? ManagerNote { get; set; }

        [JsonProperty("approvedBy")]
        public string? ApprovedBy { get; set; }

        [JsonProperty("approvedAt")]
        public DateTime? ApprovedAt { get; set; }

        [JsonProperty("color")]
        public string? Color { get; set; }
    }

    public class OvertimeSummaryModel
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
        
        [JsonProperty("totalHours")]
        [JsonPropertyName("totalHours")]
        public double TotalHours { get; set; }
    }
}
