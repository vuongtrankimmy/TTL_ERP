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
    }
}
