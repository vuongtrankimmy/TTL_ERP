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
        public string Code { get; set; }  // NV-001
        public string FullName { get; set; }
        public string Email { get; set; }
        public string CompanyEmail { get; set; }
        public string Phone { get; set; }
        public string AvatarUrl { get; set; }
        
        // Organizational Links
        [BsonRepresentation(BsonType.ObjectId)]
        public string DepartmentId { get; set; }
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string PositionId { get; set; }
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string ReportToId { get; set; }

        // Employment Status
        public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;
        public EmploymentType Type { get; set; } = EmploymentType.FullTime;
        public DateTime? JoinDate { get; set; }
        public DateTime? TerminationDate { get; set; }

        // Personal Info (Consider moving to a sub-document or separate collection for security)
        public PersonalInfo PersonalDetails { get; set; } = new();

        // System Access
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public List<string> Roles { get; set; } = new(); // ["Admin", "HR Manager", "Employee"]
    }

    public class PersonalInfo
    {
        public DateTime? DOB { get; set; }
        public string Gender { get; set; } // Consider Enum
        public string Address { get; set; }
        public string Hometown { get; set; }
        public string IdCardNumber { get; set; } // CCCD
        public DateTime? IdCardIssueDate { get; set; }
        public string IdCardPlace { get; set; }
        public string TaxCode { get; set; }
        public string BankAccount { get; set; }
        public string BankName { get; set; }
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
