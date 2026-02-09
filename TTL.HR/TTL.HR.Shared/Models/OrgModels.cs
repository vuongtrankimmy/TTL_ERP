using System.Collections.Generic;

namespace TTL.HR.Shared.Models
{
    public class OrgNode
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Department, Employee
        public string Status { get; set; } = string.Empty; // Active, Trial, etc.
        public bool IsManager { get; set; }
        
        // CCCD Info
        public string CccdNumber { get; set; } = string.Empty;
        public DateOnly? DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Nationality { get; set; } = "Việt Nam";
        public string Ethnicity { get; set; } = "Kinh";
        public string Religion { get; set; } = "Không";
        public string PlaceOfOrigin { get; set; } = string.Empty;
        public string Residence { get; set; } = string.Empty;
        public string CccdIssueDate { get; set; } = string.Empty;
        public string CccdIssuePlace { get; set; } = string.Empty;

        // Contact Info
        public string Phone { get; set; } = string.Empty;
        public string PersonalEmail { get; set; } = string.Empty;
        public string WorkEmail { get; set; } = string.Empty;

        // Finance & Insurance
        public string TaxId { get; set; } = string.Empty;
        public string SocialInsuranceId { get; set; } = string.Empty;
        public string MaritalStatus { get; set; } = string.Empty;
        public string BankAccountNumber { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string BankBranch { get; set; } = string.Empty;

        // Job Details
        public string EmployeeCode { get; set; } = string.Empty;
        public string ContractType { get; set; } = string.Empty;
        public DateOnly? OfficialJoinDate { get; set; }
        public string Workplace { get; set; } = string.Empty;

        // Emergency Contact
        public string EmergencyContactName { get; set; } = string.Empty;
        public string EmergencyContactRelation { get; set; } = string.Empty;
        public string EmergencyContactPhone { get; set; } = string.Empty;

        public List<OrgNode> Children { get; set; } = new List<OrgNode>();
    }
}
