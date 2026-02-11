using System;

namespace TTL.HR.Application.Modules.Common.Entities
{
    public abstract class BaseEntity
    {
        public string Id { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
