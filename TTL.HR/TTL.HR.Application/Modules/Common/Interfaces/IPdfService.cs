using System.Threading.Tasks;
using TTL.HR.Application.Modules.Payroll.Models;
using TTL.HR.Application.Modules.HumanResource.Models;

namespace TTL.HR.Application.Modules.Common.Interfaces
{
    public interface IPdfService
    {
        Task<byte[]> GeneratePayslipPdfAsync(PayrollModel payroll);
        Task<byte[]> GenerateEmployeeProfilePdfAsync(EmployeeModel employee);
    }
}
