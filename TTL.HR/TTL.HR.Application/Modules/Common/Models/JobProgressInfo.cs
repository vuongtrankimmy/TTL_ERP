using System;

namespace TTL.HR.Application.Modules.Common.Models
{
    public class JobProgressInfo
    {
        public string JobId { get; set; } = string.Empty;
        public int Percentage { get; set; }
        public string? Status { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsFailed { get; set; }
        public string? Error { get; set; }
        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
    }
}
