using System;

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
        
        // UI Helpers
        public string Status => IsActive ? "Active" : "Inactive";
        public string TargetPerson { get; set; } = "Toàn bộ nhân viên"; // Default for UI if not in DB
    }
}
