using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Application.Modules.Payroll.Models
{
    public class PayrollModel
    {
        public string Id { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeAvatar { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string PositionName { get; set; } = string.Empty;
        
        public int Month { get; set; }
        public int Year { get; set; }

        public decimal BasicSalary { get; set; }
        public double ActualWorkDays { get; set; }
        public double TotalRequiredDays { get; set; }
        public double OvertimeHours { get; set; }
        public double UnpaidLeaveDays { get; set; }
        
        public decimal TotalWorkSalary { get; set; }
        public decimal OvertimeSalary { get; set; }
        public decimal Allowance { get; set; }
        public List<BreakdownItemModel> AllowanceDetails { get; set; } = new();
        public decimal Bonus { get; set; }
        
        public decimal InsuranceAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Deduction { get; set; }
        public List<BreakdownItemModel> DeductionDetails { get; set; } = new();
        
        public decimal TotalSalary { get; set; }
        public decimal NetSalary { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsConfirmed { get; set; }
    }

    public class PayrollPeriodModel
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string Status { get; set; } = "";
        public decimal TotalNetSalary { get; set; }
        public decimal TotalInsurance { get; set; }
        public decimal TotalTax { get; set; }
        public int EmployeeCount { get; set; }
    }

    public class PayrollPeriodDetailModel
    {
        public PayrollPeriodModel Period { get; set; } = new();
        public PagedResult<PayrollModel> Payrolls { get; set; } = new();
    }

    public class BreakdownItemModel
    {
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Note { get; set; }
    }
}
