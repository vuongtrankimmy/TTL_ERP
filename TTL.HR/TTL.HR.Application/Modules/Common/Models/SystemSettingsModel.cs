namespace TTL.HR.Application.Modules.Common.Models
{
    public class SystemSettingsModel
    {
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
        
        // Email & Notifications
        public bool EnableEmailNotifications { get; set; } = true;
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public string SmtpUser { get; set; } = string.Empty;
        
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
        public TimeOnly? WorkStartTime { get; set; } = new TimeOnly(8, 0);
        public TimeOnly? WorkEndTime { get; set; } = new TimeOnly(17, 30);
        public TimeOnly? BreakStartTime { get; set; } = new TimeOnly(12, 0);
        public TimeOnly? BreakEndTime { get; set; } = new TimeOnly(13, 30);
        public int LateGracePeriodMinutes { get; set; } = 15;
        public string WorkDays { get; set; } = "Mon;Tue;Wed;Thu;Fri";
        public bool AllowFlexibleHours { get; set; } = false;

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
        
        // Insurance Breakdown
        public double SocialInsurancePercentEmployee { get; set; } = 8.0;
        public double HealthInsurancePercentEmployee { get; set; } = 1.5;
        public double UnemploymentInsurancePercentEmployee { get; set; } = 1.0;
        public double SocialInsurancePercentCompany { get; set; } = 17.5;
        public double HealthInsurancePercentCompany { get; set; } = 3.0;
        public double UnemploymentInsurancePercentCompany { get; set; } = 1.0;
        
        public decimal DefaultPersonalDeduction { get; set; } = 11000000;
        public decimal DefaultDependentDeduction { get; set; } = 4400000;
        
        // Statutory Pay Rate (Lương cơ sở)
        public decimal StatutoryPayRate { get; set; } = 2340000;

        // Region Minimum Salaries
        public decimal Region1MinimumSalary { get; set; } = 4680000;
        public decimal Region2MinimumSalary { get; set; } = 4160000;
        public decimal Region3MinimumSalary { get; set; } = 3640000;
        public decimal Region4MinimumSalary { get; set; } = 3250000;

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
    }
}
