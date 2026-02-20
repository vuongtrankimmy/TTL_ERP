using System;

namespace TTL.HR.Application.Modules.Common.Models
{
    public class AuditLogModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? IpAddress { get; set; }
        public DateTime CreatedAt { get; set; }

        // UI Helpers
        public string Module => EntityName;
        public DateTime Timestamp => CreatedAt;
        public string Description => $"{Action} {EntityName} ({EntityId})";
        public string Status => "Success"; // Default for now as backend doesn't seem to store failed attempts yet
    }
}
