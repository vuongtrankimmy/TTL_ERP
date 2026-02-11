using System.Collections.Generic;

namespace TTL.HR.Application.Modules.Common.Models
{
    public class PermissionDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
    }

    public class PermissionWithRolesDto : PermissionDto
    {
        public List<RoleShortDto> AssignedRoles { get; set; } = new();
    }

    public class RoleShortDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class RoleModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new();
        public int UsersCount { get; set; }
        public List<RoleMemberDto> Members { get; set; } = new();
    }

    public class RoleMemberDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
    }
}
