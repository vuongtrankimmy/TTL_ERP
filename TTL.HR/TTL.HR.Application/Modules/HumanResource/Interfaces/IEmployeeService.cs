using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.HumanResource.Models;
using TTL.HR.Application.Modules.HumanResource.Entities;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Application.Modules.HumanResource.Interfaces
{
    public interface IEmployeeService
    {
        Task<List<EmployeeDto>> GetEmployeesAsync();
        Task<PagedResult<EmployeeDto>> GetEmployeesPaginatedAsync(int pageIndex, int pageSize, string? searchTerm = null, string? departmentId = null, string? status = null, string? workplace = null, string? sortBy = "name", bool sortDesc = false);
        Task<EmployeeModel?> GetEmployeeAsync(string id);
        Task<EmployeeModel?> GetMyEmployeeAsync();
        Task<string?> CreateEmployeeAsync(Employee employee);
        Task<string?> UpdateEmployeeAsync(string id, UpdateEmployeeRequest request);
        Task<bool> DeleteEmployeeAsync(string id);
        Task<DigitalProfileModel?> GetDigitalProfileAsync(string employeeId);
        /// <summary>Returns null on success, or an error message string on failure.</summary>
        Task<string?> UploadDocumentAsync(string employeeId, string documentType, Stream fileStream, string fileName, DateTime? expiryDate, string? note);
        Task<bool> LinkIdentityAsync(string employeeId);
        Task<byte[]?> ExportEmployeesAsync(string? searchTerm = null, string? departmentId = null, string? status = null, string? workplace = null);
        Task<EmployeeStatusCounts> GetStatusCountsAsync(string? searchTerm = null, string? departmentId = null, string? workplace = null);
        Task<string?> AccrueLeaveAsync(int month, int year);
    }
}
