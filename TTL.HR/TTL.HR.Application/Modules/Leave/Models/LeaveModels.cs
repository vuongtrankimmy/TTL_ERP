using System;

namespace TTL.HR.Application.Modules.Leave.Models
{
    public class LeaveRequestModel
    {
        public string Id { get; set; } = "";
        public string EmployeeId { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public string Department { get; set; } = "";
        public string Avatar { get; set; } = "";
        public string Type { get; set; } = "";
        public string SubType { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double TotalDays { get; set; }
        public string Status { get; set; } = "";
        public string Reason { get; set; } = "";
        public string? ManagerNote { get; set; }
        public bool IsPending => Status == "Pending" || Status == "Chờ phê duyệt" || Status == "Chờ duyệt";
    }
}
