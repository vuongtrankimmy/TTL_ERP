using System;

namespace TTL.HR.Application.Modules.HumanResource.Models
{
    public class ContractTemplateModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? TypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string ContentHtml { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public int? StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public int UsageCount { get; set; }
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class EmployeeContractModel
    {
        public string Id { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public int? TypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string ContractNumber { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal AllowanceTotal { get; set; }
        public int? StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string SignedFileUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
    public class ContractTemplatesSummaryModel
    {
        public long TotalCount { get; set; }
        public long ActiveCount { get; set; }
        public long DraftCount { get; set; }
    }
}
