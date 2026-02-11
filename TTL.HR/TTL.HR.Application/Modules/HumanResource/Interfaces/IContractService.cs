using System.Collections.Generic;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.HumanResource.Models;
using TTL.HR.Application.Modules.HumanResource.Entities;

namespace TTL.HR.Application.Modules.HumanResource.Interfaces
{
    public interface IContractService
    {
        // Templates
        Task<List<ContractTemplateModel>> GetTemplatesAsync();
        Task<ContractTemplateModel?> GetTemplateAsync(string id);
        Task<ContractTemplate?> CreateTemplateAsync(ContractTemplate template);
        Task<ContractTemplate?> UpdateTemplateAsync(string id, ContractTemplate template);
        Task<bool> DeleteTemplateAsync(string id);

        // Employee Contracts
        Task<List<EmployeeContractModel>> GetEmployeeContractsAsync();
        Task<EmployeeContractModel?> GetEmployeeContractAsync(string id);
        Task<EmployeeContract?> CreateEmployeeContractAsync(EmployeeContract contract);
        Task<EmployeeContract?> UpdateEmployeeContractAsync(string id, EmployeeContract contract);
        Task<bool> DeleteEmployeeContractAsync(string id);
    }
}
