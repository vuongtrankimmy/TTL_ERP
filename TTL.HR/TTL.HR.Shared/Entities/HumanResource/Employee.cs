using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TTL.HR.Shared.Entities.Base;

namespace TTL.HR.Shared.Entities.HumanResource
{
    public class Employee : BaseEntity
    {
        // Core Identity
        public string Code { get; set; } = string.Empty;  // NV-001
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CompanyEmail { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        
        // Organizational Links
        [BsonRepresentation(BsonType.ObjectId)]
        public string DepartmentId { get; set; } = string.Empty;
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string PositionId { get; set; } = string.Empty;
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string ReportToId { get; set; } = string.Empty;

        // Employment Status
        public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;
        public EmploymentType Type { get; set; } = EmploymentType.FullTime;
        public DateTime? JoinDate { get; set; }
        public DateTime? TerminationDate { get; set; }

        // Personal Info (Consider moving to a sub-document or separate collection for security)
        public PersonalInfo PersonalDetails { get; set; } = new();

        // System Access
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new(); // ["Admin", "HR Manager", "Employee"]
    }

    public class PersonalInfo
    {
        public DateTime? DOB { get; set; }
        public string Gender { get; set; } = string.Empty; // Consider Enum
        public string Address { get; set; } = string.Empty;
        public string Hometown { get; set; } = string.Empty;
        public string IdCardNumber { get; set; } = string.Empty; // CCCD
        public DateTime? IdCardIssueDate { get; set; }
        public string IdCardPlace { get; set; } = string.Empty;
        public string TaxCode { get; set; } = string.Empty;
        public string BankAccount { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
    }

    public enum EmployeeStatus
    {
        Active,
        Probation, // Thử việc
        MaternityLeave, // Thai sản
        Resigned, // Đã nghỉ
        Terminated // Sa thải
    }

    public enum EmploymentType
    {
        FullTime,
        PartTime,
        Intern,
        Contractor
    }
}
