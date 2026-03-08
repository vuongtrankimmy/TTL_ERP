using TTL.HR.Application.Modules.Common.Models;
using System.Text.Json.Serialization;

namespace TTL.HR.Application.Modules.Payroll.Models
{
    public class PayrollModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        
        [JsonPropertyName("employeeId")]
        public string EmployeeId { get; set; } = string.Empty;
        
        [JsonPropertyName("employeeName")]
        public string EmployeeName { get; set; } = string.Empty;
        
        [JsonPropertyName("employeeCode")]
        public string EmployeeCode { get; set; } = string.Empty;
        
        [JsonPropertyName("employeeAvatar")]
        public string EmployeeAvatar { get; set; } = string.Empty;
        
        [JsonPropertyName("departmentName")]
        public string DepartmentName { get; set; } = string.Empty;
        
        [JsonPropertyName("positionName")]
        public string PositionName { get; set; } = string.Empty;
        
        [JsonPropertyName("month")]
        public int Month { get; set; }
        
        [JsonPropertyName("year")]
        public int Year { get; set; }
        
        [JsonPropertyName("basicSalary")]
        public decimal BasicSalary { get; set; }
        
        [JsonPropertyName("insuranceSalary")]
        public decimal InsuranceSalary { get; set; }
        
        [JsonPropertyName("actualWorkDays")]
        public double ActualWorkDays { get; set; }
        
        [JsonPropertyName("leaveDays")]
        public double LeaveDays { get; set; }
        
        [JsonPropertyName("holidayDays")]
        public double HolidayDays { get; set; }
        
        [JsonPropertyName("totalRequiredDays")]
        public double TotalRequiredDays { get; set; }
        
        [JsonPropertyName("overtimeHours")]
        public double OvertimeHours { get; set; }

        [JsonPropertyName("overtimeHoursWeekend")]
        public double OvertimeHoursWeekend { get; set; }

        [JsonPropertyName("overtimeHoursHoliday")]
        public double OvertimeHoursHoliday { get; set; }

        [JsonPropertyName("overtimeSalaryWeekend")]
        public decimal OvertimeSalaryWeekend { get; set; }

        [JsonPropertyName("overtimeSalaryHoliday")]
        public decimal OvertimeSalaryHoliday { get; set; }
        
        [JsonPropertyName("unpaidLeaveDays")]
        public double UnpaidLeaveDays { get; set; }
        
        [JsonPropertyName("totalWorkSalary")]
        public decimal TotalWorkSalary { get; set; }
        
        [JsonPropertyName("overtimeSalary")]
        public decimal OvertimeSalary { get; set; }
        
        [JsonPropertyName("allowance")]
        public decimal Allowance { get; set; }
        
        [JsonPropertyName("allowanceDetails")]
        public List<BreakdownItemModel> AllowanceDetails { get; set; } = new();
        
        [JsonPropertyName("bonus")]
        public decimal Bonus { get; set; }
        
        [JsonPropertyName("bhxhAmount")]
        public decimal BhxhAmount { get; set; }
        
        [JsonPropertyName("bhytAmount")]
        public decimal BhytAmount { get; set; }
        
        [JsonPropertyName("bhtnAmount")]
        public decimal BhtnAmount { get; set; }
        
        [JsonPropertyName("unionFee")]
        public decimal UnionFee { get; set; }
        
        [JsonPropertyName("insuranceAmount")]
        public decimal InsuranceAmount { get; set; }
        
        [JsonPropertyName("taxableIncome")]
        public decimal TaxableIncome { get; set; }
        
        [JsonPropertyName("personalDeduction")]
        public decimal PersonalDeduction { get; set; }
        
        [JsonPropertyName("dependentDeduction")]
        public decimal DependentDeduction { get; set; }
        
        [JsonPropertyName("taxAmount")]
        public decimal TaxAmount { get; set; }
        
        [JsonPropertyName("advanceAmount")]
        public decimal AdvanceAmount { get; set; }
        
        [JsonPropertyName("deduction")]
        public decimal Deduction { get; set; }
        
        [JsonPropertyName("deductionDetails")]
        public List<BreakdownItemModel> DeductionDetails { get; set; } = new();
        
        [JsonPropertyName("grossSalary")]
        public decimal GrossSalary { get; set; }
        
        [JsonPropertyName("totalDeduction")]
        public decimal TotalDeduction { get; set; }
        
        [JsonPropertyName("netSalary")]
        public decimal NetSalary { get; set; }
        
        [JsonPropertyName("statusId")]
        public int? StatusId { get; set; }
        
        [JsonPropertyName("statusName")]
        public string StatusName { get; set; } = string.Empty;
        
        [JsonPropertyName("statusColor")]
        public string StatusColor { get; set; } = string.Empty;
        
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
        
        [JsonPropertyName("warningMessage")]
        public string? WarningMessage { get; set; }
        
        [JsonPropertyName("isConfirmed")]
        public bool IsConfirmed { get; set; }
        
        [JsonPropertyName("confirmationComment")]
        public string? ConfirmationComment { get; set; }
        
        [JsonPropertyName("approvedByName")]
        public string? ApprovedByName { get; set; }
    }

    public class PayrollPeriodModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";
        
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
        
        [JsonPropertyName("month")]
        public int Month { get; set; }
        
        [JsonPropertyName("year")]
        public int Year { get; set; }
        
        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }
        
        [JsonPropertyName("endDate")]
        public DateTime EndDate { get; set; }
        
        [JsonPropertyName("paymentDate")]
        public DateTime? PaymentDate { get; set; }
        
        [JsonPropertyName("status")]
        public string Status { get; set; } = "";

        [JsonPropertyName("statusId")]
        public int? StatusId { get; set; }

        [JsonPropertyName("statusName")]
        public string? StatusName { get; set; }

        [JsonPropertyName("statusColor")]
        public string? StatusColor { get; set; }
        
        [JsonPropertyName("totalNetSalary")]
        public decimal TotalNetSalary { get; set; }
        
        [JsonPropertyName("totalInsurance")]
        public decimal TotalInsurance { get; set; }
        
        [JsonPropertyName("totalTax")]
        public decimal TotalTax { get; set; }
        
        [JsonPropertyName("employeeCount")]
        public int EmployeeCount { get; set; }

        [JsonPropertyName("otRateWeekday")]
        public double OtRateWeekday { get; set; }

        [JsonPropertyName("otRateWeekend")]
        public double OtRateWeekend { get; set; }

        [JsonPropertyName("otRateHoliday")]
        public double OtRateHoliday { get; set; }
    }

    public class PayrollPeriodDetailModel
    {
        [JsonPropertyName("period")]
        public PayrollPeriodModel Period { get; set; } = new();
        
        [JsonPropertyName("payrolls")]
        public PagedResult<PayrollModel> Payrolls { get; set; } = new();
    }

    public class BreakdownItemModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
        
        [JsonPropertyName("note")]
        public string? Note { get; set; }
    }
}
