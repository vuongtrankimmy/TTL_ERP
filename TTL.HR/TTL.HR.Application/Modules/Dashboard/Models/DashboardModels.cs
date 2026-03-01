using System;
using System.Collections.Generic;

namespace TTL.HR.Application.Modules.Dashboard.Models
{
    public class DashboardOverviewModel
    {
        public DashboardStats Stats { get; set; } = new();
        public PayrollStats PayrollStats { get; set; } = new();
        public ContractDistribution ContractDistribution { get; set; } = new();
        public RecruitmentStats RecruitmentStats { get; set; } = new();
        public List<DepartmentStat> DepartmentDistribution { get; set; } = new();
        public List<UpcomingTraining> UpcomingTrainings { get; set; } = new();
        public List<PendingApproval> PendingApprovals { get; set; } = new();
        public List<ExpiringContract> ExpiringContracts { get; set; } = new();
        public AttendanceToday AttendanceToday { get; set; } = new();
        public List<PendingAssetClearance> PendingAssetClearances { get; set; } = new();
        public List<PayrollTrendDto> PayrollTrend { get; set; } = new();
    }

    public class PayrollTrendDto
    {
        public string Month { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    public class PendingAssetClearance
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string AssetName { get; set; } = string.Empty;
        public string AssetCode { get; set; } = string.Empty;
        public DateTime? DepartureDate { get; set; }
    }

    public class PayrollStats
    {
        public decimal TotalBudget { get; set; }
        public double GrowthPercentage { get; set; }
        public string Period { get; set; } = "";
    }

    public class ContractDistribution
    {
        public double OfficialPercentage { get; set; }
        public string Label { get; set; } = "";
    }

    public class RecruitmentStats
    {
        public int NewCandidates { get; set; }
        public int InterviewsThisWeek { get; set; }
        public int RecentlyHired { get; set; }
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
