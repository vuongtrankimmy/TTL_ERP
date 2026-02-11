using System;
using System.Collections.Generic;
using TTL.HR.Application.Modules.Common.Entities;

namespace TTL.HR.Application.Modules.Payroll.Entities
{
    public class SalaryComponent : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public ComponentType Type { get; set; }
        public bool IsFixed { get; set; }
        public bool IsTaxable { get; set; }
    }

    public class PayrollPeriod : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool IsClosed { get; set; }
    }

    public class SalarySlip : BaseEntity
    {
        public string EmployeeId { get; set; } = string.Empty;

        public string PeriodId { get; set; } = string.Empty;

        public decimal TotalEarning { get; set; }
        public decimal TotalDeduction { get; set; }
        public decimal NetSalary { get; set; }

        public List<SalaryDetail> Details { get; set; } = new();

        public SlipStatus Status { get; set; } = SlipStatus.Draft;
    }

    public class SalaryDetail
    {
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public ComponentType Type { get; set; }
    }

    public enum ComponentType { Earning, Deduction, CompanyCost }
    public enum SlipStatus { Draft, Sent, Confirmed, Paid }
}
