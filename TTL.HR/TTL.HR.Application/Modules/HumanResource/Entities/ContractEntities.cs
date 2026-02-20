using System;
using TTL.HR.Application.Modules.Common.Entities;

namespace TTL.HR.Application.Modules.HumanResource.Entities
{
    public class ContractTemplate : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TypeId { get; set; } = string.Empty;
        public string ContentHtml { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string StatusId { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }

    public class EmployeeContract : BaseEntity
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string ContractTemplateId { get; set; } = string.Empty;
        public string ContractNumber { get; set; } = string.Empty;
        public string TypeId { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal AllowanceTotal { get; set; }
        public string StatusId { get; set; } = string.Empty;
        public string SignedFileUrl { get; set; } = string.Empty;
    }
}
