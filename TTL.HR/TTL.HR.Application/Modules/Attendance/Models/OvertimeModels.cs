using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TTL.HR.Application.Modules.Attendance.Models
{
    public class OvertimeRequestModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("employeeId")]
        public string EmployeeId { get; set; } = string.Empty;

        [JsonPropertyName("employeeName")]
        public string EmployeeName { get; set; } = string.Empty;

        [JsonPropertyName("employeeCode")]
        public string EmployeeCode { get; set; } = string.Empty;

        [JsonPropertyName("departmentName")]
        public string DepartmentName { get; set; } = string.Empty;

        [JsonPropertyName("avatarUrl")]
        public string AvatarUrl { get; set; } = string.Empty;

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("startTime")]
        public string StartTime { get; set; } = string.Empty;

        [JsonPropertyName("endTime")]
        public string EndTime { get; set; } = string.Empty;

        [JsonPropertyName("hours")]
        public double Hours { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = "Pending";

        [JsonPropertyName("statusId")]
        public int StatusId { get; set; } = 1;

        [JsonPropertyName("multiplier")]
        public double Multiplier { get; set; } = 1.5;

        [JsonPropertyName("managerNote")]
        public string? ManagerNote { get; set; }

        [JsonPropertyName("approvedBy")]
        public string? ApprovedBy { get; set; }

        [JsonPropertyName("approvedAt")]
        public DateTime? ApprovedAt { get; set; }

        [JsonPropertyName("color")]
        public string? Color { get; set; }
    }

    public class OvertimeSummaryModel
    {
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public double TotalHours { get; set; }
    }
}
