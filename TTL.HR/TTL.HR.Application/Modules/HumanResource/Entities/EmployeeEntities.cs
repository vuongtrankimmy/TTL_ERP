using System;
using System.Collections.Generic;
using TTL.HR.Application.Modules.Common.Entities;
using TTL.HR.Application.Modules.HumanResource.Models;
namespace TTL.HR.Application.Modules.HumanResource.Entities
{
    public class Employee : BaseEntity
    {
        public string Code { get; set; } = string.Empty;
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
        public DateTime? JoinDate { get; set; }
        public DateTime? DepartureDate { get; set; }
        public string? DepartureReason { get; set; }

        public decimal? Salary { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public int? WorkplaceId { get; set; }
        public string Workplace { get; set; } = string.Empty;
        public bool IsAccountActive { get; set; }
        public bool IsTrainer { get; set; }

        public PersonalInfo PersonalDetails { get; set; } = new();
        public EmergencyContact EmergencyContact { get; set; } = new();
        public List<EducationDetailDto> Education { get; set; } = new();
        public List<ExperienceDetailDto> Experience { get; set; } = new();

        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public bool IsCreateAccount { get; set; }
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class PersonalInfo
    {
        public DateTime? DOB { get; set; }
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
        public string Nationality { get; set; } = "Việt Nam";
        public int? EthnicityId { get; set; }
        public string Ethnicity { get; set; } = "Kinh";
        public int? ReligionId { get; set; }
        public string Religion { get; set; } = "Không";
        public int? MaritalStatusId { get; set; }
        public string MaritalStatus { get; set; } = "Độc thân";
        public string PlaceOfOrigin { get; set; } = string.Empty;
        public string Residence { get; set; } = string.Empty;
        public string SocialInsuranceId { get; set; } = string.Empty;
        public List<DependentDetailDto> Dependents { get; set; } = new();
        public double Latitude { get; set; } = 0;
        public double Longitude { get; set; }= 0;
    }

    public class EmergencyContact
    {
        public string Name { get; set; } = string.Empty;
        public string Relation { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    public enum EmployeeStatus { Active, Probation, MaternityLeave, Resigned, Terminated }
    public enum EmploymentType { FullTime, PartTime, Intern, Contractor }
}
