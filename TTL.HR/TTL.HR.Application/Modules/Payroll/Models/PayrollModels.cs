using TTL.HR.Application.Modules.Common.Models;
using Newtonsoft.Json;
using System.Text.Json.Serialization;


namespace TTL.HR.Application.Modules.Payroll.Models
{
    public class PayrollModel
    {
        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        
        [JsonProperty("employeeId")]
        [JsonPropertyName("employeeId")]
        public string EmployeeId { get; set; } = string.Empty;
        
        [JsonProperty("employeeName")]
        [JsonPropertyName("employeeName")]
        public string EmployeeName { get; set; } = string.Empty;
        
        [JsonProperty("employeeCode")]
        [JsonPropertyName("employeeCode")]
        public string EmployeeCode { get; set; } = string.Empty;
        
        [JsonProperty("employeeAvatar")]
        [JsonPropertyName("employeeAvatar")]
        public string EmployeeAvatar { get; set; } = string.Empty;
        
        [JsonProperty("departmentName")]
        [JsonPropertyName("departmentName")]
        public string DepartmentName { get; set; } = string.Empty;
        
        [JsonProperty("positionName")]
        [JsonPropertyName("positionName")]
        public string PositionName { get; set; } = string.Empty;
        
        [JsonProperty("month")]
        [JsonPropertyName("month")]
        public int Month { get; set; }
        
        [JsonProperty("year")]
        [JsonPropertyName("year")]
        public int Year { get; set; }
        
        [JsonProperty("basicSalary")]
        [JsonPropertyName("basicSalary")]
        public decimal BasicSalary { get; set; }
        
        [JsonProperty("insuranceSalary")]
        [JsonPropertyName("insuranceSalary")]
        public decimal InsuranceSalary { get; set; }
        
        [JsonProperty("actualWorkDays")]
        [JsonPropertyName("actualWorkDays")]
        public double ActualWorkDays { get; set; }
        
        [JsonProperty("leaveDays")]
        [JsonPropertyName("leaveDays")]
        public double LeaveDays { get; set; }
        
        [JsonProperty("holidayDays")]
        [JsonPropertyName("holidayDays")]
        public double HolidayDays { get; set; }
        
        [JsonProperty("totalRequiredDays")]
        [JsonPropertyName("totalRequiredDays")]
        public double TotalRequiredDays { get; set; }
        
        [JsonProperty("overtimeHours")]
        public double OvertimeHours { get; set; }

        [JsonProperty("overtimeHoursWeekend")]
        public double OvertimeHoursWeekend { get; set; }

        [JsonProperty("overtimeHoursHoliday")]
        [JsonPropertyName("overtimeHoursHoliday")]
        public double OvertimeHoursHoliday { get; set; }

        [JsonProperty("overtimeSalaryWeekend")]
        public decimal OvertimeSalaryWeekend { get; set; }

        [JsonProperty("overtimeSalaryHoliday")]
        public decimal OvertimeSalaryHoliday { get; set; }
        
        [JsonProperty("unpaidLeaveDays")]
        [JsonPropertyName("unpaidLeaveDays")]
        public double UnpaidLeaveDays { get; set; }
        
        [JsonProperty("totalWorkSalary")]
        [JsonPropertyName("totalWorkSalary")]
        public decimal TotalWorkSalary { get; set; }
        
        [JsonProperty("overtimeSalary")]
        [JsonPropertyName("overtimeSalary")]
        public decimal OvertimeSalary { get; set; }
        
        [JsonProperty("allowance")]
        [JsonPropertyName("allowance")]
        public decimal Allowance { get; set; }
        
        [JsonProperty("allowanceDetails")]
        [JsonPropertyName("allowanceDetails")]
        public List<BreakdownItemModel> AllowanceDetails { get; set; } = new();
        
        [JsonProperty("bonus")]
        [JsonPropertyName("bonus")]
        public decimal Bonus { get; set; }
        
        [JsonProperty("bhxhAmount")]
        [JsonPropertyName("bhxhAmount")]
        public decimal BhxhAmount { get; set; }
        
        [JsonProperty("bhytAmount")]
        [JsonPropertyName("bhytAmount")]
        public decimal BhytAmount { get; set; }
        
        [JsonProperty("bhtnAmount")]
        [JsonPropertyName("bhtnAmount")]
        public decimal BhtnAmount { get; set; }
        
        [JsonProperty("unionFee")]
        [JsonPropertyName("unionFee")]
        public decimal UnionFee { get; set; }
        
        [JsonProperty("insuranceAmount")]
        [JsonPropertyName("insuranceAmount")]
        public decimal InsuranceAmount { get; set; }
        
        [JsonProperty("taxableIncome")]
        [JsonPropertyName("taxableIncome")]
        public decimal TaxableIncome { get; set; }
        
        [JsonProperty("personalDeduction")]
        [JsonPropertyName("personalDeduction")]
        public decimal PersonalDeduction { get; set; }
        
        [JsonProperty("dependentDeduction")]
        [JsonPropertyName("dependentDeduction")]
        public decimal DependentDeduction { get; set; }
        
        [JsonProperty("taxAmount")]
        [JsonPropertyName("taxAmount")]
        public decimal TaxAmount { get; set; }
        
        [JsonProperty("advanceAmount")]
        [JsonPropertyName("advanceAmount")]
        public decimal AdvanceAmount { get; set; }
        
        [JsonProperty("paymentDetails")]
        [JsonPropertyName("paymentDetails")]
        public List<PaymentItemModel> PaymentDetails { get; set; } = new();
        
        [JsonProperty("deduction")]
        [JsonPropertyName("deduction")]
        public decimal Deduction { get; set; }
        
        [JsonProperty("deductionDetails")]
        [JsonPropertyName("deductionDetails")]
        public List<BreakdownItemModel> DeductionDetails { get; set; } = new();
        
        [JsonProperty("grossSalary")]
        [JsonPropertyName("grossSalary")]
        public decimal GrossSalary { get; set; }
        
        [JsonProperty("totalDeduction")]
        [JsonPropertyName("totalDeduction")]
        public decimal TotalDeduction { get; set; }
        
        [JsonProperty("netSalary")]
        [JsonPropertyName("netSalary")]
        public decimal NetSalary { get; set; }
        
        [JsonProperty("statusId")]
        [JsonPropertyName("statusId")]
        public int? StatusId { get; set; }
        
        [JsonProperty("statusName")]
        [JsonPropertyName("statusName")]
        public string StatusName { get; set; } = string.Empty;
        
        [JsonProperty("statusColor")]
        [JsonPropertyName("statusColor")]
        public string StatusColor { get; set; } = string.Empty;
        
        [JsonProperty("status")]
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
        
        [JsonProperty("warningMessage")]
        [JsonPropertyName("warningMessage")]
        public string? WarningMessage { get; set; }
        
        [JsonProperty("isConfirmed")]
        [JsonPropertyName("isConfirmed")]
        public bool IsConfirmed { get; set; }
        
        [JsonProperty("confirmationComment")]
        [JsonPropertyName("confirmationComment")]
        public string? ConfirmationComment { get; set; }
        
        [JsonPropertyName("approvedByName")]
        public string? ApprovedByName { get; set; }
    }


    public class PayrollPeriodModel
    {
        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";
        
        [JsonProperty("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
        
        [JsonProperty("month")]
        [JsonPropertyName("month")]
        public int Month { get; set; }
        
        [JsonProperty("year")]
        [JsonPropertyName("year")]
        public int Year { get; set; }
        
        [JsonProperty("startDate")]
        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }
        
        [JsonProperty("endDate")]
        [JsonPropertyName("endDate")]
        public DateTime EndDate { get; set; }
        
        [JsonProperty("paymentDate")]
        [JsonPropertyName("paymentDate")]
        public DateTime? PaymentDate { get; set; }
        
        [JsonProperty("status")]
        [JsonPropertyName("status")]
        public string Status { get; set; } = "";

        [JsonProperty("statusId")]
        [JsonPropertyName("statusId")]
        public int? StatusId { get; set; }

        [JsonProperty("statusName")]
        [JsonPropertyName("statusName")]
        public string? StatusName { get; set; }

        [JsonProperty("statusColor")]
        [JsonPropertyName("statusColor")]
        public string? StatusColor { get; set; }
        
        [JsonProperty("totalNetSalary")]
        [JsonPropertyName("totalNetSalary")]
        public decimal TotalNetSalary { get; set; }
        
        [JsonProperty("totalInsurance")]
        [JsonPropertyName("totalInsurance")]
        public decimal TotalInsurance { get; set; }
        
        [JsonProperty("totalTax")]
        [JsonPropertyName("totalTax")]
        public decimal TotalTax { get; set; }
        
        [JsonProperty("employeeCount")]
        [JsonPropertyName("employeeCount")]
        public int EmployeeCount { get; set; }

        [JsonProperty("otRateWeekday")]
        [JsonPropertyName("otRateWeekday")]
        public double OtRateWeekday { get; set; }

        [JsonProperty("otRateWeekend")]
        [JsonPropertyName("otRateWeekend")]
        public double OtRateWeekend { get; set; }

        [JsonProperty("otRateHoliday")]
        [JsonPropertyName("otRateHoliday")]
        public double OtRateHoliday { get; set; }
    }

    public class PayrollPeriodDetailModel
    {
        [JsonProperty("period")]
        [JsonPropertyName("period")]
        public PayrollPeriodModel Period { get; set; } = new();
        
        [JsonProperty("payrolls")]
        [JsonPropertyName("payrolls")]
        public PagedResult<PayrollModel> Payrolls { get; set; } = new();
    }

    public class BreakdownItemModel
    {
        [JsonProperty("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        
        [JsonProperty("amount")]
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
        
        [JsonProperty("note")]
        [JsonPropertyName("note")]
        public string? Note { get; set; }
    }

    public class PaymentItemModel
    {
        [JsonProperty("paymentDate")]
        [JsonPropertyName("paymentDate")]
        public DateTime? PaymentDate { get; set; }
        
        [JsonProperty("amount")]
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
        
        [JsonProperty("description")]
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        
        [JsonProperty("paymentMethod")]
        [JsonPropertyName("paymentMethod")]
        public string? PaymentMethod { get; set; }
    }
}
