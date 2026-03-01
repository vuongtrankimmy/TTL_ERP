using System.Collections.Generic;

namespace TTL.HR.Application.Modules.Common.Models
{
    public class NavItem
    {
        public string Title { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Href { get; set; } = string.Empty;
        public string? Permission { get; set; }
        public bool IsSection { get; set; }
        public List<NavItem> SubItems { get; set; } = new();
        public bool HasSubItems => SubItems != null && SubItems.Count > 0;
    }
}
