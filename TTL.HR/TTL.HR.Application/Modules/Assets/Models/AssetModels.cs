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
        public string? EmployeeName { get; set; }
        public string? AssignedToName { get; set; }
    }
}
