using System;
using TTL.HR.Application.Modules.Common.Entities;

namespace TTL.HR.Application.Modules.Assets.Entities
{
    public class CompanyAsset : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public AssetType Type { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal DepreciationPerYear { get; set; }
        public AssetStatus Status { get; set; } = AssetStatus.Available;
        public string CurrentlyAssignedTo { get; set; } = string.Empty;
    }

    public class AssetAllocation : BaseEntity
    {
        public string AssetId { get; set; } = string.Empty;

        public string EmployeeId { get; set; } = string.Empty;

        public DateTime HandoverDate { get; set; }
        public string HandoverCondition { get; set; } = string.Empty;

        public DateTime? ReturnDate { get; set; }
        public string ReturnCondition { get; set; } = string.Empty;

        public string Remarks { get; set; } = string.Empty;
    }

    public enum AssetType { Computer, Accessory, Furniture, Vehicle, Uniform }
    public enum AssetStatus { Available, Assigned, Repairing, Dispose, Lost }
}
