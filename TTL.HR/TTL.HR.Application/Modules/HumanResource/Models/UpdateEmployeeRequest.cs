using System;
using System.Collections.Generic;

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
        public string? DepartmentId { get; set; }
        public string? PositionId { get; set; }
        public string? ReportToId { get; set; }
        public int? StatusId { get; set; }
        public int? ContractTypeId { get; set; }
        public DateTime JoinDate { get; set; }
        public decimal? Salary { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public int? WorkplaceId { get; set; }
        public string Workplace { get; set; } = string.Empty;
        public bool IsAccountActive { get; set; }
        public bool IsCreateAccount { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public PersonalDetailsUpdateDto PersonalDetails { get; set; } = new();
        public EmergencyContactUpdateDto EmergencyContact { get; set; } = new();
        public List<EducationDetailDto>? Education { get; set; } = new();
        public List<ExperienceDetailDto>? Experience { get; set; } = new();
    }

    public class PersonalDetailsUpdateDto
    {
        public DateTime? DOB { get; set; } // Changed to nullable to match UI model
        public int? GenderId { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Hometown { get; set; } = string.Empty;
        public string IdCardNumber { get; set; } = string.Empty;
        public DateTime? IdCardIssueDate { get; set; }
        public string IdCardPlace { get; set; } = string.Empty;
        public string TaxCode { get; set; } = string.Empty;
        public string BankAccount { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public int? NationalityId { get; set; }
        public string Nationality { get; set; } = string.Empty;
        public int? EthnicityId { get; set; }
        public string Ethnicity { get; set; } = string.Empty;
        public int? ReligionId { get; set; }
        public string Religion { get; set; } = string.Empty;
        public int? MaritalStatusId { get; set; }
        public string MaritalStatus { get; set; } = string.Empty;
        public string PlaceOfOrigin { get; set; } = string.Empty;
        public string Residence { get; set; } = string.Empty;
        public string SocialInsuranceId { get; set; } = string.Empty;
        public List<DependentDetailDto>? Dependents { get; set; } = new();
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class EmergencyContactUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Relation { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}
