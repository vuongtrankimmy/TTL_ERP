using System.Collections.Generic;

namespace TTL.HR.Application.Modules.Common.Models
{
    public class NavItem
    {
        public string Id { get; set; } = string.Empty;
        public int NumericId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Href { get; set; } = string.Empty;
        public string? Permission { get; set; }
        public bool IsSection { get; set; }
        public bool IsActive { get; set; } = true;
        public int Order { get; set; }
        public List<NavItem> SubItems { get; set; } = new();
        public bool HasSubItems => SubItems != null && SubItems.Count > 0;
    }

    public class NotificationModel
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Type { get; set; } = "Notify"; // Notify, Email, SMS, System
        public bool IsRead { get; set; }
        public string? Icon { get; set; }
        public string? Link { get; set; }
        public string? ActionUrl { get; set; }
    }

    public class RecentMenuModel
    {
        public string Title { get; set; } = string.Empty;
        public string Href { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public DateTime LastAccessed { get; set; }
    }
}
