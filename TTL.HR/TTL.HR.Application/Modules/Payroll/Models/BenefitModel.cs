using System;
using System.Collections.Generic;

namespace TTL.HR.Application.Modules.Payroll.Models
{
    public class BenefitModel
    {
        public string Id { get; set; } = "";
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Type { get; set; } = ""; // Monthly, Event, One-time
        public decimal Amount { get; set; }
        public string Category { get; set; } = "Allowance";
        public bool IsActive { get; set; } = true;
        public string Icon { get; set; } = "";
        public int EmployeeCount { get; set; }
        
        // UI Helpers
        public string Status => IsActive ? "Active" : "Inactive";
        public string TargetPerson { get; set; } = "Toàn bộ nhân viên"; // Default for UI if not in DB
    }

    public class BenefitAssignRequest
    {
        public string EmployeeId { get; set; } = "";
        public string BenefitId { get; set; } = "";
        public decimal? OverrideAmount { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime? EndDate { get; set; }
        public string? Note { get; set; }
    }

    public class EmployeeBenefitModel
    {
        public string Id { get; set; } = "";
        public string EmployeeId { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public string BenefitId { get; set; } = "";
        public string BenefitName { get; set; } = "";
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public string? Note { get; set; }
    }

    public class BatchAssignRequest
    {
        public string BenefitId { get; set; } = string.Empty;
        public List<string> EmployeeIdsToAssign { get; set; } = new();
        public List<string> EmployeeIdsToRemove { get; set; } = new();
        public decimal? OverrideAmount { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime? EndDate { get; set; }
        public string? Note { get; set; }
    }
}
