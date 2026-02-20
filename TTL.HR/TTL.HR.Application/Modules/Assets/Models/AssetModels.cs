namespace TTL.HR.Application.Modules.Assets.Models
{
    public class AssetModel
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Code { get; set; } = "";
        public string Category { get; set; } = "";
        public string Status { get; set; } = "";
        public string Type { get; set; } = "";
        public decimal PurchasePrice { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string SerialNumber { get; set; } = "";
        public string? Specification { get; set; }
        public string? WarrantyPeriod { get; set; }
        public string? EmployeeName { get; set; }
        public string? AssignedToName { get; set; }
    }
    public class AssetHistoryModel
    {
        public DateTime Date { get; set; }
        public string Action { get; set; } = "";
        public string Description { get; set; } = "";
        public string Actor { get; set; } = "";
        public string? EmployeeName { get; set; }
    }

    public class AssetAllocationDto
    {
        public string Id { get; set; } = string.Empty;
        public string AssetName { get; set; } = string.Empty;
        public string AssetCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime AllocatedDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Note { get; set; }
        public string? ActorName { get; set; }
        public string AssetId { get; set; } = string.Empty;
    }
}
