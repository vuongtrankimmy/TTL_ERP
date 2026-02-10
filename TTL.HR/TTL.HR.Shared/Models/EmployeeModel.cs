namespace TTL.HR.Shared.Models
{
    public class EmployeeModel
    {
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
        public string Password { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        // Bổ sung các trường từ CCCD và thông tin quản lý
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
        public string OfficialJoinDate { get; set; } = string.Empty;
        public string Workplace { get; set; } = string.Empty;
        public string EmergencyContactName { get; set; } = string.Empty;
        public string EmergencyContactRelation { get; set; } = string.Empty;
        public string EmergencyContactPhone { get; set; } = string.Empty;
    }
}
