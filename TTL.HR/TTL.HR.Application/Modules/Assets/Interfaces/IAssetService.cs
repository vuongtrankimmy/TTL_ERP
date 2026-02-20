using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Assets.Models;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Application.Modules.Assets.Interfaces
{
    public interface IAssetService
    {
        Task<IEnumerable<AssetModel>> GetAssetsAsync();
        Task<PagedResult<AssetAllocationDto>> GetAllocationsAsync(int page, int pageSize, string? status = null, string? searchTerm = null);
        Task<AssetModel?> GetAssetAsync(string id);
        Task<IEnumerable<AssetHistoryModel>> GetAssetHistoryAsync(string assetId);
        Task<bool> AssignAssetAsync(string assetId, string employeeId, string condition, string note);
        Task<bool> ReturnAssetAsync(string assetId, string condition, string note);
        Task<bool> RequestMaintenanceAsync(string assetId, string issue, string priority);
        Task<bool> CreateAssetAsync(AssetModel asset);
        Task<bool> UpdateAssetAsync(string id, AssetModel asset);
        Task<bool> DeleteAssetAsync(string id);
    }
}
