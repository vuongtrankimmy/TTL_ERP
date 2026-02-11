using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Assets.Models;

namespace TTL.HR.Application.Modules.Assets.Interfaces
{
    public interface IAssetService
    {
        Task<IEnumerable<AssetModel>> GetAssetsAsync();
        Task<AssetModel?> GetAssetAsync(string id);
        Task<bool> AssignAssetAsync(string assetId, string employeeId);
        Task<bool> ReturnAssetAsync(string assetId, string condition, string note);
        Task<bool> CreateAssetAsync(AssetModel asset);
        Task<bool> UpdateAssetAsync(string id, AssetModel asset);
        Task<bool> DeleteAssetAsync(string id);
    }
}
