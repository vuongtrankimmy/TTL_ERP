using System;

namespace TTL.HR.Application.Modules.HumanResource.Models
{
    public class ContractTemplateModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string ContentHtml { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class EmployeeContractModel
    {
        public string Id { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string TemplateId { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;
        public string ContractNumber { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal AllowanceTotal { get; set; }
        public string Status { get; set; } = string.Empty;
        public string SignedFileUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
