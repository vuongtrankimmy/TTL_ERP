using System.Collections.Generic;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.HumanResource.Models;
using TTL.HR.Application.Modules.HumanResource.Entities;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Application.Modules.HumanResource.Interfaces
{
    public interface IContractService
    {
        // Templates
        Task<PagedResult<ContractTemplateModel>> GetTemplatesAsync(int page = 1, int pageSize = 10, string? searchTerm = null, string? status = null, int? typeId = null, string? lang = null);
        Task<ContractTemplatesSummaryModel> GetTemplateSummaryAsync();
        Task<ContractTemplateModel?> GetTemplateAsync(string id, string? lang = null);
        Task<ContractTemplate?> CreateTemplateAsync(ContractTemplate template);
        Task<ContractTemplate?> UpdateTemplateAsync(string id, ContractTemplate template);
        Task<bool> DeleteTemplateAsync(string id);

        // Employee Contracts
        Task<PagedResult<EmployeeContractModel>> GetEmployeeContractsAsync(int page = 1, int pageSize = 10, string? searchTerm = null, string? status = null, int? typeId = null, string? employeeId = null, string? lang = null);
        Task<EmployeeContractModel?> GetEmployeeContractAsync(string id, string? lang = null);
        Task<EmployeeContract?> CreateEmployeeContractAsync(EmployeeContract contract);
        Task<EmployeeContract?> UpdateEmployeeContractAsync(string id, EmployeeContract contract);
        Task<bool> DeleteEmployeeContractAsync(string id);
    }
}
