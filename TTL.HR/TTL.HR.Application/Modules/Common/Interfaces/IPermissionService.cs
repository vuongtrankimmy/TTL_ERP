using System.Collections.Generic;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Application.Modules.Common.Interfaces
{
    public interface IPermissionService
    {
        Task<List<PermissionWithRolesDto>> GetPermissionsAsync();
        Task<List<RoleModel>> GetRolesAsync(string? searchTerm = null);
        Task<RoleModel?> GetRoleByIdAsync(string id);
        Task<(bool Success, string Message)> CreateRoleAsync(RoleModel role);
        Task<(bool Success, string Message)> UpdateRoleAsync(string id, RoleModel role);
        Task<bool> DeleteRoleAsync(string id);
        Task<List<PermissionDto>> GetAllAvailablePermissionsAsync();
        Task<bool> AssignRoleAsync(string roleId, string employeeId);
        Task<bool> AssignRolesAsync(string roleId, List<string> employeeIds);
        Task<bool> UnassignRoleAsync(string roleId, string employeeId);
    }
}
