namespace TTL.HR.Shared.Models
{
    public class LookupModel
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
    }
}
