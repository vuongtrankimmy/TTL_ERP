using System.Collections.Generic;

namespace TTL.HR.Shared.Models
{
    public class OrgNode
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Avatar { get; set; }
        public string Type { get; set; } // Department, Employee
        public string Status { get; set; } // Active, Trial, etc.
        public bool IsManager { get; set; }
        
        // CCCD Info
        public string CccdNumber { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Nationality { get; set; } = "Việt Nam";
        public string Ethnicity { get; set; } = "Kinh";
        public string Religion { get; set; } = "Không";
        public string PlaceOfOrigin { get; set; }
        public string Residence { get; set; }
        public string CccdIssueDate { get; set; }
        public string CccdIssuePlace { get; set; }

        // Contact Info
        public string Phone { get; set; }
        public string PersonalEmail { get; set; }
        public string WorkEmail { get; set; }

        // Finance & Insurance
        public string TaxId { get; set; }
        public string SocialInsuranceId { get; set; }
        public string MaritalStatus { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankName { get; set; }
        public string BankBranch { get; set; }

        // Job Details
        public string EmployeeCode { get; set; }
        public string ContractType { get; set; }
        public DateOnly? OfficialJoinDate { get; set; }
        public string Workplace { get; set; }

        // Emergency Contact
        public string EmergencyContactName { get; set; }
        public string EmergencyContactRelation { get; set; }
        public string EmergencyContactPhone { get; set; }

        public List<OrgNode> Children { get; set; } = new List<OrgNode>();
    }
}
