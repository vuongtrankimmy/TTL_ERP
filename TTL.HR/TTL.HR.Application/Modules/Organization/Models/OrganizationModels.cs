using System.Collections.Generic;

namespace TTL.HR.Application.Modules.Organization.Models
{
    public class DepartmentModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ParentId { get; set; }
        public string? ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public string? ManagerTitle { get; set; }
        public string? ManagerAvatar { get; set; }
        public int EmployeeCount { get; set; }
        public int Capacity { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class DepartmentDetailModel : DepartmentModel
    {
        public int ActiveMembers { get; set; }
        public int PendingReviews { get; set; }
        public List<DepartmentMemberModel> Members { get; set; } = new();
    }

    public class DepartmentMemberModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string? PositionName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Status { get; set; }
        public System.DateTime JoinDate { get; set; }
    }

    public class CreateDepartmentRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ParentId { get; set; }

        public string? ManagerId { get; set; }
        public int Capacity { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateDepartmentRequest : CreateDepartmentRequest
    {
        public string Id { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class PositionModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal BaseSalaryRangeMin { get; set; }
        public decimal BaseSalaryRangeMax { get; set; }
        public string DepartmentId { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
        public string Level { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
        public string Status { get; set; } = "Active";
        public List<string>? Responsibilities { get; set; }
        public List<string>? Requirements { get; set; }
        public List<string>? Benefits { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class CreatePositionRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal BaseSalaryRangeMin { get; set; }
        public decimal BaseSalaryRangeMax { get; set; }
        public string DepartmentId { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
        public List<string>? Responsibilities { get; set; }
        public List<string>? Requirements { get; set; }
        public List<string>? Benefits { get; set; }
    }

    public class UpdatePositionRequest : CreatePositionRequest
    {
        public string Id { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class OrganizationNode
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? ParentId { get; set; }
        public string? ManagerId { get; set; }
        public string ManagerName { get; set; } = string.Empty;
        public string? ManagerAvatar { get; set; }
        public int EmployeeCount { get; set; }
        public List<OrganizationNode> Children { get; set; } = new();
    }

    public class OrgNode
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Department or Employee
        public string Status { get; set; } = string.Empty;
        public bool IsManager { get; set; }
        public List<OrgNode> Children { get; set; } = new List<OrgNode>();

        // Detail properties for UI
        public string EmployeeCode { get; set; } = string.Empty;
        public string CccdNumber { get; set; } = string.Empty;
        public System.DateOnly? DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string PlaceOfOrigin { get; set; } = string.Empty;
        public string Residence { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string PersonalEmail { get; set; } = string.Empty;
        public string WorkEmail { get; set; } = string.Empty;
        public string ContractTypeId { get; set; } = string.Empty;
        public string ContractTypeName { get; set; } = string.Empty;
        public string ContractStatusId { get; set; } = string.Empty;
        public string ContractStatusName { get; set; } = string.Empty;
        public System.DateOnly? OfficialJoinDate { get; set; }
        public string Workplace { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public string Ethnicity { get; set; } = string.Empty;
        public string Religion { get; set; } = string.Empty;
        public string TaxId { get; set; } = string.Empty;
        public string SocialInsuranceId { get; set; } = string.Empty;
        public string MaritalStatus { get; set; } = string.Empty;
        public string EmergencyContactName { get; set; } = string.Empty;
        public string EmergencyContactRelation { get; set; } = string.Empty;
        public string EmergencyContactPhone { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
    }
}
