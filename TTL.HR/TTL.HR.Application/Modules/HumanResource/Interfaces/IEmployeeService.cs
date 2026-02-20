using System.Collections.Generic;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.HumanResource.Models;
using TTL.HR.Application.Modules.HumanResource.Entities;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Application.Modules.HumanResource.Interfaces
{
    public interface IEmployeeService
    {
        Task<List<EmployeeDto>> GetEmployeesAsync();
        Task<PagedResult<EmployeeDto>> GetEmployeesPaginatedAsync(int pageIndex, int pageSize, string? searchTerm = null, string? departmentId = null, string? status = null);
        Task<EmployeeModel?> GetEmployeeAsync(string id);
        Task<EmployeeModel?> GetMyEmployeeAsync();

        Task<string?> CreateEmployeeAsync(Employee employee);
        Task<string?> UpdateEmployeeAsync(string id, UpdateEmployeeRequest request);
        Task<bool> DeleteEmployeeAsync(string id);
    }
}
