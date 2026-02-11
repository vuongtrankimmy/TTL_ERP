using System;
using System.Collections.Generic;
using TTL.HR.Application.Modules.Common.Entities;

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
        
        public string DepartmentId { get; set; } = string.Empty;
        public string PositionId { get; set; } = string.Empty;
        public string ReportToId { get; set; } = string.Empty;

        public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;
        public EmploymentType Type { get; set; } = EmploymentType.FullTime;
        public DateTime? JoinDate { get; set; }
        public DateTime? TerminationDate { get; set; }

        public PersonalInfo PersonalDetails { get; set; } = new();

        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }

    public class PersonalInfo
    {
        public DateTime? DOB { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Hometown { get; set; } = string.Empty;
        public string IdCardNumber { get; set; } = string.Empty;
        public DateTime? IdCardIssueDate { get; set; }
        public string IdCardPlace { get; set; } = string.Empty;
        public string TaxCode { get; set; } = string.Empty;
        public string BankAccount { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
    }

    public enum EmployeeStatus { Active, Probation, MaternityLeave, Resigned, Terminated }
    public enum EmploymentType { FullTime, PartTime, Intern, Contractor }
}
