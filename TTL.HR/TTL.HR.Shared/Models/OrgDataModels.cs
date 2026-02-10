namespace TTL.HR.Shared.Models
{
    public class DepartmentModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? ParentId { get; set; }
        public string? ManagerId { get; set; }
        public int EmployeeCount { get; set; }
    }

    public class PositionModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
    }
}
