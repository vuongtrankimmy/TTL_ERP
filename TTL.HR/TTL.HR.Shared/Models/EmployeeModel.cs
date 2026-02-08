namespace TTL.HR.Shared.Models
{
    public class EmployeeModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CompanyEmail { get; set; }
        public string Dept { get; set; }
        public string Role { get; set; }
        public DateTime? JoinDate { get; set; }
        public string Status { get; set; }
        public string Avatar { get; set; }
        public string IdCard { get; set; }
        public DateTime? DOB { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Hometown { get; set; }
        public string ContractType { get; set; }
        public string Salary { get; set; }
        public DateTime? ContractExpiry { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }

        // Bổ sung các trường từ CCCD và thông tin quản lý
        public string Nationality { get; set; } = "Việt Nam";
        public string Ethnicity { get; set; } = "Kinh";
        public string Religion { get; set; } = "Không";
        public string PlaceOfOrigin { get; set; }
        public string Residence { get; set; }
        public string CccdIssueDate { get; set; }
        public string CccdIssuePlace { get; set; }
        public string TaxId { get; set; }
        public string SocialInsuranceId { get; set; }
        public string MaritalStatus { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankName { get; set; }
        public string BankBranch { get; set; }
        public string OfficialJoinDate { get; set; }
        public string Workplace { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactRelation { get; set; }
        public string EmergencyContactPhone { get; set; }
    }
}
