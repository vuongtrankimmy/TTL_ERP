using System.Collections.Generic;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Organization.Models;

namespace TTL.HR.Application.Modules.Organization.Interfaces
{
    public interface IDepartmentService
    {
        Task<List<DepartmentModel>> GetDepartmentsAsync();
        Task<DepartmentModel?> GetDepartmentAsync(string id);
        Task<DepartmentDetailModel?> GetDepartmentDetailAsync(string id);
        Task<DepartmentModel?> CreateDepartmentAsync(CreateDepartmentRequest request);
        Task<DepartmentModel?> UpdateDepartmentAsync(string id, UpdateDepartmentRequest request);
        Task<bool> DeleteDepartmentAsync(string id);
        Task<bool> AssignEmployeesAsync(string departmentId, List<string> employeeIds);
        Task<List<OrgNode>> GetOrganizationStructureAsync();
    }

    public interface IPositionService
    {
        Task<List<PositionModel>> GetPositionsAsync();
        Task<PositionModel?> GetPositionAsync(string id);
        Task<PositionModel?> CreatePositionAsync(CreatePositionRequest request);
        Task<PositionModel?> UpdatePositionAsync(string id, UpdatePositionRequest request);
        Task<bool> DeletePositionAsync(string id);
        Task<bool> AssignEmployeesAsync(string positionId, List<string> employeeIds, decimal? salary = null);
    }
}
