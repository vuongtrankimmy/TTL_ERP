using System;

namespace TTL.HR.Application.Modules.HumanResource.Models
{
    public class EmployeeModel
    {
        public string Code { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
        public string ReportToId { get; set; } = string.Empty;
        public string Workplace { get; set; } = string.Empty;
        public DateTime? OfficialJoinDate { get; set; }
        public string Password { get; set; } = string.Empty;
        public EmployeePersonalDetails? PersonalDetails { get; set; } = new();
        public EmployeeEmergencyContact? EmergencyContact { get; set; } = new();
        public EmployeeAttendanceSummary? AttendanceSummary { get; set; } = new();
        public List<EmployeeAuditLog>? AuditLogs { get; set; } = new();
        public List<EmployeeModulePermission>? ModulePermissions { get; set; } = new();

        // Display properties from Backend API
        public string DepartmentName { get; set; } = string.Empty;
        public string PositionName { get; set; } = string.Empty;
        public string ReportToName { get; set; } = string.Empty;

        // Compatibility properties or existing ones
        public string Id { get; set; } = string.Empty;
        public string TimekeepingCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CompanyEmail { get; set; } = string.Empty;
        public string Dept { get; set; } = string.Empty;
        public string DeptId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string PositionId { get; set; } = string.Empty;
        public DateTime? JoinDate { get; set; }
        public string StatusId { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public string IdCard { get; set; } = string.Empty;
        public DateTime? DOB { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Hometown { get; set; } = string.Empty;
        public string ContractTypeId { get; set; } = string.Empty;
        public string ContractTypeName { get; set; } = string.Empty;
        public string Salary { get; set; } = string.Empty;
        public decimal? SalaryAmount { get; set; }
        public DateTime? ContractExpiry { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public string Username { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsAccountActive { get; set; }
        public string Nationality { get; set; } = "Việt Nam";
        public string Ethnicity { get; set; } = "Kinh";
        public string Religion { get; set; } = "Không";
        public string PlaceOfOrigin { get; set; } = string.Empty;
        public string Residence { get; set; } = string.Empty;
        public DateTime? CccdIssueDate { get; set; }
        public string CccdIssuePlace { get; set; } = string.Empty;
        public string TaxId { get; set; } = string.Empty;
        public string SocialInsuranceId { get; set; } = string.Empty;
        public string MaritalStatus { get; set; } = string.Empty;
        public string BankAccountNumber { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string BankBranch { get; set; } = string.Empty;
        public string EmergencyContactName { get; set; } = string.Empty;
        public string EmergencyContactRelation { get; set; } = string.Empty;
        public string EmergencyContactPhone { get; set; } = string.Empty;
        public double Latitude { get; set; } = 0;
        public double Longitude { get; set; } = 0;
    }

    public class EmployeePersonalDetails
    {
        public string IdCardNumber { get; set; } = string.Empty;
        public string IdCard { get; set; } = string.Empty;
        public DateTime? DOB { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Hometown { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public string Ethnicity { get; set; } = string.Empty;
        public string Religion { get; set; } = string.Empty;
        public string TaxCode { get; set; } = string.Empty;
        public string BankAccount { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string MaritalStatus { get; set; } = string.Empty;
        public string PlaceOfOrigin { get; set; } = string.Empty;
        public string Residence { get; set; } = string.Empty;
        public string SocialInsuranceId { get; set; } = string.Empty;
        public DateTime? IdCardIssueDate { get; set; }
        public string IdCardPlace { get; set; } = string.Empty;
        public double Latitude { get; set; } = 0;
        public double Longitude { get; set; } = 0;
    }

    public class EmployeeEmergencyContact
    {
        public string Name { get; set; } = string.Empty;
        public string Relation { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    public class EmployeeAttendanceSummary
    {
        public string CurrentShift { get; set; } = string.Empty;
        public int TotalWorkingDays { get; set; }
        public int LeavesTaken { get; set; }
        public int RemainingLeaves { get; set; }
        public double OvertimeHours { get; set; }
    }

    public class EmployeeAuditLog
    {
        public DateTime Timestamp { get; set; }
        public string Action { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
    }

    public class EmployeeModulePermission
    {
        public string ModuleName { get; set; } = string.Empty;
        public bool CanView { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }

    public class CccdData
    {
        public string Name { get; set; } = string.Empty;
        public string IdCard { get; set; } = string.Empty;
        public DateTime? DOB { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Hometown { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Nationality { get; set; } = "Việt Nam";
        public string Ethnicity { get; set; } = "Kinh";
        public string Religion { get; set; } = "Không";
        public string IssueDate { get; set; } = string.Empty;
        public string IssuePlace { get; set; } = string.Empty;
    }
    public class EmployeeDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string PositionId { get; set; } = string.Empty;
        public string PositionName { get; set; } = string.Empty;
        public string StatusId { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public string ContractTypeId { get; set; } = string.Empty;
        public string ContractTypeName { get; set; } = string.Empty;
        public DateTime? JoinDate { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Workplace { get; set; } = string.Empty;
    }

    public class EmployeeDocumentModel
    {
        public string Id { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Status { get; set; } = "Pending";
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }

        public string StatusColor => Status switch
        {
            "Pending" => "warning",
            "Verified" => "success",
            "Rejected" => "danger",
            _ => "secondary"
        };

        public string StatusLabel => Status switch
        {
            "Pending" => "Chờ duyệt",
            "Verified" => "Đã xác thực",
            "Rejected" => "Từ chối",
            _ => Status
        };

        public string FileSizeDisplay => FileSize switch
        {
            > 1048576 => $"{FileSize / 1048576.0:F1} MB",
            > 1024 => $"{FileSize / 1024.0:F1} KB",
            _ => $"{FileSize} B"
        };
    }

    public class DigitalProfileModel
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public List<EmployeeDocumentModel> Documents { get; set; } = new();
        public double CompletionPercentage { get; set; }
    }
}
