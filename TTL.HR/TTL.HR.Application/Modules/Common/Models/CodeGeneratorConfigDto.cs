using System;

namespace TTL.HR.Application.Modules.Common.Models
{
    public class CodeGeneratorConfigDto
    {
        public string Id { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Prefix { get; set; } = string.Empty;
        public string Suffix { get; set; } = string.Empty;
        public string Separator { get; set; } = string.Empty;
        public string GenerationType { get; set; } = string.Empty;
        public bool IncludeDate { get; set; }
        public string DateFormat { get; set; } = string.Empty;
        public int Length { get; set; }
        public long CurrentSequence { get; set; }
        public string ResetCondition { get; set; } = "None";
        public bool IsActive { get; set; } = true;
        public DateTime? LastGeneratedAt { get; set; }
    }
}
