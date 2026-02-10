using System.Collections.Generic;
using System.Threading.Tasks;
using TTL.HR.Shared.Models;

namespace TTL.HR.Shared.Interfaces
{
    public interface IMasterDataService
    {
        Task<List<LookupModel>> GetLookupsAsync(string? type = null);
        Task<List<LookupModel>> GetCachedLookupsAsync(string type);
        Task ClearCacheAsync();
    }
}
