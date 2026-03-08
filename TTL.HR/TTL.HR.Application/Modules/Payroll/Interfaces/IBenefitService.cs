using System.Collections.Generic;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Payroll.Models;

namespace TTL.HR.Application.Modules.Payroll.Interfaces
{
    public interface IBenefitService
    {
        Task<IEnumerable<BenefitModel>> GetBenefitsAsync();
        Task<BenefitModel?> GetBenefitByIdAsync(string id);
        Task<bool> CreateBenefitAsync(BenefitModel benefit);
        Task<bool> UpdateBenefitAsync(string id, BenefitModel benefit);
        Task<bool> DeleteBenefitAsync(string id);
        Task<bool> AssignBenefitAsync(BenefitAssignRequest request);
        Task<IEnumerable<EmployeeBenefitModel>> GetEmployeeBenefitsAsync(string employeeId);
        Task<IEnumerable<EmployeeBenefitModel>> GetBenefitAllocationsAsync(string benefitId);
        Task<bool> RemoveEmployeeBenefitAsync(string allocationId);
        Task<bool> BatchAssignBenefitsAsync(BatchAssignRequest request);
        Task<byte[]?> ExportBenefitsAsync(string? searchTerm = null, string? category = null);
    }
}
