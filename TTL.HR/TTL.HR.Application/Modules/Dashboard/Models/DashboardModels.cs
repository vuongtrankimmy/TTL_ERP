using System;
using System.Collections.Generic;

namespace TTL.HR.Application.Modules.Dashboard.Models
{
    public class DashboardOverviewModel
    {
        public DashboardStats Stats { get; set; } = new();
        public List<DepartmentStat> DepartmentDistribution { get; set; } = new();
        public List<UpcomingTraining> UpcomingTrainings { get; set; } = new();
        public List<PendingApproval> PendingApprovals { get; set; } = new();
        public List<ExpiringContract> ExpiringContracts { get; set; } = new();
        public AttendanceToday AttendanceToday { get; set; } = new();
    }

    public class DashboardStats
    {
        public int TotalEmployees { get; set; }
        public int TotalDepartments { get; set; }
        public int ActiveJobPostings { get; set; }
        public int TotalCourses { get; set; }
    }

    public class DepartmentStat
    {
        public string DepartmentName { get; set; } = "";
        public int EmployeeCount { get; set; }
    }

    public class UpcomingTraining
    {
        public string Title { get; set; } = "";
        public string Trainer { get; set; } = "";
        public double DurationHours { get; set; }
        public string Status { get; set; } = "";
    }

    public class PendingApproval
    {
        public string Type { get; set; } = "";
        public string Title { get; set; } = "";
        public DateTime Date { get; set; }
        public string Status { get; set; } = "";
    }

    public class ExpiringContract
    {
        public string EmployeeName { get; set; } = "";
        public string ContractType { get; set; } = "";
        public DateTime ExpiryDate { get; set; }
        public string Department { get; set; } = "";
    }

    public class AttendanceToday
    {
        public double Present { get; set; }
        public double Absent { get; set; }
        public double Late { get; set; }
        public int Total { get; set; }
    }
}
