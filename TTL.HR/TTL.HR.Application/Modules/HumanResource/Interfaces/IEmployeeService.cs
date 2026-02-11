using System.Collections.Generic;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.HumanResource.Models;
using TTL.HR.Application.Modules.HumanResource.Entities;

namespace TTL.HR.Application.Modules.HumanResource.Interfaces
{
    public interface IEmployeeService
    {
        Task<List<EmployeeModel>> GetEmployeesAsync();
        Task<EmployeeModel?> GetEmployeeAsync(string id);
        Task<Employee?> CreateEmployeeAsync(Employee employee);
        Task<Employee?> UpdateEmployeeAsync(string id, Employee employee);
        Task<bool> DeleteEmployeeAsync(string id);
    }
}
