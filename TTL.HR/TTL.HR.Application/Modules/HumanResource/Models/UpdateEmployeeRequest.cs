using System;

namespace TTL.HR.Application.Modules.HumanResource.Models
{
    public class UpdateEmployeeRequest
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CompanyEmail { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public string TimekeepingCode { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
        public string PositionId { get; set; } = string.Empty;
        public string? ReportToId { get; set; }
        public string? StatusId { get; set; }
        public string? ContractTypeId { get; set; }
        public DateTime JoinDate { get; set; }
        public decimal? Salary { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public string Workplace { get; set; } = string.Empty;
        public bool IsAccountActive { get; set; }
        public PersonalDetailsUpdateDto PersonalDetails { get; set; } = new();
        public EmergencyContactUpdateDto EmergencyContact { get; set; } = new();
    }

    public class PersonalDetailsUpdateDto
    {
        public DateTime? DOB { get; set; } // Changed to nullable to match UI model
        public string Gender { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Hometown { get; set; } = string.Empty;
        public string IdCardNumber { get; set; } = string.Empty;
        public DateTime? IdCardIssueDate { get; set; }
        public string IdCardPlace { get; set; } = string.Empty;
        public string TaxCode { get; set; } = string.Empty;
        public string BankAccount { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string Nationality { get; set; } = "Việt Nam";
        public string Ethnicity { get; set; } = "Kinh";
        public string Religion { get; set; } = "Không";
        public string MaritalStatus { get; set; } = "Độc thân";
        public string PlaceOfOrigin { get; set; } = string.Empty;
        public string Residence { get; set; } = string.Empty;
        public string SocialInsuranceId { get; set; } = string.Empty;
    }

    public class EmergencyContactUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Relation { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}
