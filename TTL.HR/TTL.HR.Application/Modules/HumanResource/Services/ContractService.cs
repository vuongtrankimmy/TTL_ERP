using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.HumanResource.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Models;
using TTL.HR.Application.Modules.HumanResource.Entities;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.Common.Constants;

namespace TTL.HR.Application.Modules.HumanResource.Services
{
    public class ContractService : IContractService
    {
        private readonly HttpClient _httpClient;

        public ContractService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Templates
        public async Task<PagedResult<ContractTemplateModel>> GetTemplatesAsync(int page = 1, int pageSize = 10, string? searchTerm = null, string? status = null, int? typeId = null, string? lang = null)
        {
            var query = $"{ApiEndpoints.Contracts.Templates}?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(searchTerm)) query += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";
            if (!string.IsNullOrEmpty(status) && status != "All") query += $"&status={status}";
            if (typeId.HasValue) query += $"&typeId={typeId}";
            if (!string.IsNullOrEmpty(lang)) query += $"&LanguageCode={lang}";

            var response = await _httpClient.GetFromJsonAsync<ApiResponse<PagedResult<ContractTemplateModel>>>(query);
            return response?.Data ?? new PagedResult<ContractTemplateModel>();
        }

        public async Task<ContractTemplatesSummaryModel> GetTemplateSummaryAsync()
        {
            var url = $"{ApiEndpoints.Contracts.Templates}/summary";
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<ContractTemplatesSummaryModel>>(url);
            return response?.Data ?? new ContractTemplatesSummaryModel();
        }

        public async Task<ContractTemplateModel?> GetTemplateAsync(string id, string? lang = null)
        {
            var url = $"{ApiEndpoints.Contracts.Templates}/{id}";
            if (!string.IsNullOrEmpty(lang)) url += $"?LanguageCode={lang}";
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<ContractTemplateModel>>(url);
            return response?.Data;
        }

        public async Task<ContractTemplate?> CreateTemplateAsync(ContractTemplate template)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Contracts.Templates, template);
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
            if (apiResponse?.Success == true)
            {
                template.Id = apiResponse.Data ?? string.Empty;
                return template;
            }
            return null;
        }

        public async Task<ContractTemplate?> UpdateTemplateAsync(string id, ContractTemplate template)
        {
            var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoints.Contracts.Templates}/{id}", template);
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
            if (apiResponse?.Success == true)
            {
                return template;
            }
            return null;
        }

        public async Task<bool> DeleteTemplateAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiEndpoints.Contracts.Templates}/{id}");
            return response.IsSuccessStatusCode;
        }

        // Employee Contracts
        public async Task<PagedResult<EmployeeContractModel>> GetEmployeeContractsAsync(int page = 1, int pageSize = 10, string? searchTerm = null, string? status = null, int? typeId = null, string? employeeId = null, string? lang = null)
        {
            var query = $"{ApiEndpoints.Contracts.Base}?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(searchTerm)) query += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";
            if (!string.IsNullOrEmpty(status) && status != "All") query += $"&status={status}";
            if (typeId.HasValue) query += $"&typeId={typeId}";
            if (!string.IsNullOrEmpty(employeeId)) query += $"&employeeId={employeeId}";
            if (!string.IsNullOrEmpty(lang)) query += $"&LanguageCode={lang}";

            var response = await _httpClient.GetFromJsonAsync<ApiResponse<PagedResult<EmployeeContractModel>>>(query);
            return response?.Data ?? new PagedResult<EmployeeContractModel>();
        }

        public async Task<EmployeeContractModel?> GetEmployeeContractAsync(string id, string? lang = null)
        {
            var url = $"{ApiEndpoints.Contracts.Base}/{id}";
            if (!string.IsNullOrEmpty(lang)) url += $"?LanguageCode={lang}";
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<EmployeeContractModel>>(url);
            return response?.Data;
        }

        public async Task<EmployeeContract?> CreateEmployeeContractAsync(EmployeeContract contract)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Contracts.Base, contract);
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
            if (apiResponse?.Success == true)
            {
                contract.Id = apiResponse.Data ?? string.Empty;
                return contract;
            }
            return null;
        }

        public async Task<EmployeeContract?> UpdateEmployeeContractAsync(string id, EmployeeContract contract)
        {
            var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoints.Contracts.Base}/{id}", contract);
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
            if (apiResponse?.Success == true)
            {
                return contract;
            }
            return null;
        }

        public async Task<bool> DeleteEmployeeContractAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiEndpoints.Contracts.Base}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
