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
        public string Amount { get; set; } = ""; // Human readable amount
        public decimal AmountValue { get; set; }
        public string TargetPerson { get; set; } = ""; // Target audience
        public string Status { get; set; } = "Active";
        public string Icon { get; set; } = "ki-briefcase";
        public string Category { get; set; } = "Allowance";
    }
}
