using System.Collections.Generic;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.HumanResource.Models;

namespace TTL.HR.Application.Modules.HumanResource.Interfaces
{
    public interface IEmployeeImportService
    {
        Task<byte[]> ExportEmployeeTemplateAsync();
        Task<ImportResultModel> ImportEmployeesFromExcelAsync(byte[] fileContent);
        Task<byte[]> ExportEmployeeListAsync(IEnumerable<EmployeeModel> employees);
    }

    public class ImportResultModel
    {
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
