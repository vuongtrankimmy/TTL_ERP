using System;
using System.Collections.Generic;

namespace TTL.HR.Application.Modules.Common.Models
{
    public class HolidayConfigModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; } = DateTime.Today;
        public bool IsRecurring { get; set; } = false; // Có lặp lại hàng năm hay không
    }

    public class TaxStepModel
    {
        public decimal Threshold { get; set; }
        public double Rate { get; set; }
        public decimal Deduction { get; set; }
    }

    public class UrlTitleMappingModel
    {
        public string Segment { get; set; } = string.Empty;
        public string TitleKey { get; set; } = string.Empty;
    }

    public class SystemSettingsModel
    {
        public string ActiveTab { get; set; } = string.Empty;

        // Company Information
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyAddress { get; set; } = string.Empty;
        public string CompanyPhone { get; set; } = string.Empty;
        public string CompanyEmail { get; set; } = string.Empty;
        public string TaxCode { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        
        public string LogoUrl { get; set; } = string.Empty;
        public string FaviconUrl { get; set; } = string.Empty;
        
        // Localization
        public string DateFormat { get; set; } = "dd/MM/yyyy";
        public string DefaultLanguage { get; set; } = "vi-VN";
        public string TimeZone { get; set; } = "SE Asia Standard Time";
        public string Currency { get; set; } = "VND";
        public string ThousandSeparator { get; set; } = ",";
        public string DecimalSeparator { get; set; } = ".";
        
        // Email & Notifications
        public bool EnableEmailNotifications { get; set; } = true;
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public string SmtpUser { get; set; } = string.Empty;
        public string SmtpPassword { get; set; } = string.Empty;
        
        // Notification Preferences
        public bool NotifyOnLeaveRequest { get; set; } = true;
        public bool NotifyOnLeaveApproval { get; set; } = true;
        public bool NotifyOnContractExpiry { get; set; } = true;
        public int ContractExpiryWarningDays { get; set; } = 30;
        public bool NotifyOnProbationEnd { get; set; } = true;
        public int ProbationEndWarningDays { get; set; } = 7;
        public bool NotifyOnBirthday { get; set; } = true;
        public bool NotifyOnWorkAnniversary { get; set; } = true;
        
        // Contact Person
        public string ContactPersonName { get; set; } = string.Empty;
        public string ContactPersonPhone { get; set; } = string.Empty;
        public string ContactPersonEmail { get; set; } = string.Empty;

        // Working Policy
        public string WorkStartTime { get; set; } = "08:00";
        public string WorkEndTime { get; set; } = "17:30";
        public string BreakStartTime { get; set; } = "12:00";
        public string BreakEndTime { get; set; } = "13:30";
        public int LateGracePeriodMinutes { get; set; } = 15;
        public List<string> WorkDays { get; set; } = new();
        public bool AllowFlexibleHours { get; set; } = false;
        public int AutoApproveLeaveDaysThreshold { get; set; } = 3;

        // Holidays and Off-days
        public List<HolidayConfigModel> Holidays { get; set; } = new();

        // Recruitment Config
        public bool AutoSendThankYouEmail { get; set; } = true;
        public string DefaultInterviewLocation { get; set; } = string.Empty;
        public int ResumeRetentionDays { get; set; } = 180;
        public bool EnableExternalJobBoard { get; set; } = false;

        // Training Config
        public int DefaultTrainingDurationHours { get; set; } = 4;
        public bool AutoEnrollOnboarding { get; set; } = true;
        public int CertificateExpiryWarningDays { get; set; } = 30;
        public bool EnableFeedback { get; set; } = true;
        public decimal MaxBudgetPerEmployee { get; set; } = 5000000;

        // Payroll Config
        public decimal StandardWorkDaysPerMonth { get; set; } = 26;
        public int PayDay { get; set; } = 5;
        public double OvertimeRateWeekday { get; set; } = 1.5;
        public double OvertimeRateWeekend { get; set; } = 2.0;
        public double OvertimeRateHoliday { get; set; } = 3.0;
        
        // Overtime Formulas & Policy
        public string OvertimeFormula { get; set; } = "(ActualHours - ShiftHours) * Rate";
        public string OvertimeCalculationMethod { get; set; } = "ApprovedOnly"; // Auto, ApprovedOnly, Hybrid
        public bool RequireOvertimeApproval { get; set; } = true;
        public int MinOvertimeMinutes { get; set; } = 30;
        public double OvertimeOvernightRate { get; set; } = 2.1;
        public string OvertimeCode { get; set; } = @"
// Variables: ActualHours, RatePerByHour, IsWeekend, IsHoliday, IsOvernight
// Constants: OvertimeRateWeekday, OvertimeRateWeekend, OvertimeRateHoliday, OvertimeOvernightRate

double multiplier = IsHoliday ? 3.0 : (IsWeekend ? 2.0 : 1.5);
if (IsOvernight) multiplier += 0.3; // VN Law: +30% for night shift

decimal total = (decimal)ActualHours * (decimal)RatePerByHour * (decimal)multiplier;
return Math.Round(total, 0);
";
        public string SalaryCode { get; set; } = @"
// Variables: BasicSalary, ActualWorkDays, StandardWorkDays, MealAllow, TravelAllow, PhoneAllow, DiligenceAllow, Insurance, TaxAmt
// Logic: Calculate base salary + standard allowances - deductions
decimal basePay = (BasicSalary * (decimal)ActualWorkDays) / (decimal)StandardWorkDays;
decimal totalAllowances = MealAllow + TravelAllow + PhoneAllow + DiligenceAllow;
decimal gross = basePay + totalAllowances;
decimal net = gross - Insurance - TaxAmt;

return Math.Round(net, 0);
";
        
        // Insurance Breakdown
        public double SocialInsurancePercentEmployee { get; set; } = 8.0;
        public double HealthInsurancePercentEmployee { get; set; } = 1.5;
        public double UnemploymentInsurancePercentEmployee { get; set; } = 1.0;
        public double SocialInsurancePercentCompany { get; set; } = 17.5;
        public double HealthInsurancePercentCompany { get; set; } = 3.0;
        public double UnemploymentInsurancePercentCompany { get; set; } = 1.0;
        
        public decimal DefaultPersonalDeduction { get; set; } = 11000000;
        public decimal DefaultDependentDeduction { get; set; } = 4400000;
        public List<TaxStepModel> PitSteps { get; set; } = new();
        public string PitExplanation { get; set; } = string.Empty;
        
        // Statutory Pay Rate (Lương cơ sở)
        public decimal StatutoryPayRate { get; set; } = 2340000;

        // Region Minimum Salaries
        public decimal Region1MinimumSalary { get; set; } = 4680000;
        public decimal Region2MinimumSalary { get; set; } = 4160000;
        public decimal Region3MinimumSalary { get; set; } = 3640000;
        public decimal Region4MinimumSalary { get; set; } = 3250000;

        // Global Allowances (Default amounts if not specified per employee)
        public decimal DefaultMealAllowance { get; set; } = 730000;
        public decimal DefaultTravelAllowance { get; set; } = 500000;
        public decimal DefaultPhoneAllowance { get; set; } = 300000;
        public decimal DefaultResponsibilityAllowance { get; set; } = 0;
        public decimal DefaultSeniorityAllowance { get; set; } = 0;
        public decimal DefaultDiligenceAllowance { get; set; } = 200000;

        // Security Config
        public int PasswordMinLength { get; set; } = 8;
        public bool PasswordRequireSpecialChar { get; set; } = true;
        public bool PasswordRequireDigit { get; set; } = true;
        public int MaxLoginAttempts { get; set; } = 5;
        public int LockoutDurationMinutes { get; set; } = 30;
        public int SessionTimeoutMinutes { get; set; } = 60;
        
        // File Upload Config
        public int MaxFileSizeMb { get; set; } = 10;
        public string AllowedFileExtensions { get; set; } = ".jpg;.png;.pdf;.docx;.xlsx";
        
        // Navigation & Breadcrumbs
        public List<UrlTitleMappingModel> UrlTitleMappings { get; set; } = new();
        public List<NavItem> SidebarMenu { get; set; } = new();
        public List<LanguageTranslationModel> Translations { get; set; } = new();
    }

    public class LanguageTranslationModel
    {
        public string Id { get; set; } = string.Empty;
        public int NavigationID { get; set; }
        public string LanguageCode { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
