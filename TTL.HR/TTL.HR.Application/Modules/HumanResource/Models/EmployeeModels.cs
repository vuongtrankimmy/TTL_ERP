using System;

namespace TTL.HR.Application.Modules.HumanResource.Models
{
    public class EmployeeModel
    {
        public string Code { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
        public string Workplace { get; set; } = string.Empty;
        public DateTime? OfficialJoinDate { get; set; }
        public string Password { get; set; } = string.Empty;
        public EmployeePersonalDetails? PersonalDetails { get; set; } = new();

        // Compatibility properties or existing ones
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CompanyEmail { get; set; } = string.Empty;
        public string Dept { get; set; } = string.Empty;
        public string DeptId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string PositionId { get; set; } = string.Empty;
        public DateTime? JoinDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public string IdCard { get; set; } = string.Empty;
        public DateTime? DOB { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Hometown { get; set; } = string.Empty;
        public string ContractType { get; set; } = string.Empty;
        public string Salary { get; set; } = string.Empty;
        public DateTime? ContractExpiry { get; set; }
        public string Username { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Nationality { get; set; } = "Việt Nam";
        public string Ethnicity { get; set; } = "Kinh";
        public string Religion { get; set; } = "Không";
        public string PlaceOfOrigin { get; set; } = string.Empty;
        public string Residence { get; set; } = string.Empty;
        public string CccdIssueDate { get; set; } = string.Empty;
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
}
