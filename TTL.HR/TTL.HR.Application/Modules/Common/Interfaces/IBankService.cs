using System.Collections.Generic;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Application.Modules.Common.Interfaces
{
    public interface IBankService
    {
        Task<PagedResult<BankDto>> GetBanksAsync(GetBanksRequest request);
        Task<BankDto?> GetBankByIdAsync(string id);
        Task<BankDto?> CreateBankAsync(CreateBankRequest request);
        Task<bool> UpdateBankAsync(UpdateBankRequest request);
        Task<bool> DeleteBankAsync(string id);
    }
}
