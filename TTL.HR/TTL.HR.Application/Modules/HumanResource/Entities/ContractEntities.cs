using System;
using TTL.HR.Application.Modules.Common.Entities;

namespace TTL.HR.Application.Modules.HumanResource.Entities
{
    public class ContractTemplate : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string ContentHtml { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }

    public class EmployeeContract : BaseEntity
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string TemplateId { get; set; } = string.Empty;
        public string ContractNumber { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal AllowanceTotal { get; set; }
        public string Status { get; set; } = "Draft";
        public string SignedFileUrl { get; set; } = string.Empty;
    }
}
