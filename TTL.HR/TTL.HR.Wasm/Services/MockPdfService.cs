using System.Collections.Generic;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Models;
using TTL.HR.Application.Modules.Payroll.Models;

namespace TTL.HR.Wasm.Services
{
    public class MockPdfService : IPdfService
    {
        public Task<byte[]> GeneratePayslipPdfAsync(PayrollModel payroll)
        {
            return Task.FromResult(System.Array.Empty<byte>());
        }

        public Task<byte[]> GenerateBatchPayslipsPdfAsync(IEnumerable<PayrollModel> payrolls)
        {
            return Task.FromResult(System.Array.Empty<byte>());
        }

        public Task<byte[]> GenerateEmployeeProfilePdfAsync(EmployeeModel employee)
        {
            return Task.FromResult(System.Array.Empty<byte>());
        }
    }
}
