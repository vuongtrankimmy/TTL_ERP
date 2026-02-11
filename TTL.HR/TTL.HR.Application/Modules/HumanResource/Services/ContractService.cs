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
        public async Task<List<ContractTemplateModel>> GetTemplatesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<ContractTemplateModel>>>(ApiEndpoints.Contracts.Templates);
            return response?.Data ?? new List<ContractTemplateModel>();
        }

        public async Task<ContractTemplateModel?> GetTemplateAsync(string id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<ContractTemplateModel>>($"{ApiEndpoints.Contracts.Templates}/{id}");
            return response?.Data;
        }

        public async Task<ContractTemplate?> CreateTemplateAsync(ContractTemplate template)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Contracts.Templates, template);
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ContractTemplate>>();
            return apiResponse?.Data;
        }

        public async Task<ContractTemplate?> UpdateTemplateAsync(string id, ContractTemplate template)
        {
            var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoints.Contracts.Templates}/{id}", template);
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ContractTemplate>>();
            return apiResponse?.Data;
        }

        public async Task<bool> DeleteTemplateAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiEndpoints.Contracts.Templates}/{id}");
            return response.IsSuccessStatusCode;
        }

        // Employee Contracts
        public async Task<List<EmployeeContractModel>> GetEmployeeContractsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<EmployeeContractModel>>>(ApiEndpoints.Contracts.Base);
            return response?.Data ?? new List<EmployeeContractModel>();
        }

        public async Task<EmployeeContractModel?> GetEmployeeContractAsync(string id)
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<EmployeeContractModel>>($"{ApiEndpoints.Contracts.Base}/{id}");
            return response?.Data;
        }

        public async Task<EmployeeContract?> CreateEmployeeContractAsync(EmployeeContract contract)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Contracts.Base, contract);
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<EmployeeContract>>();
            return apiResponse?.Data;
        }

        public async Task<EmployeeContract?> UpdateEmployeeContractAsync(string id, EmployeeContract contract)
        {
            var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoints.Contracts.Base}/{id}", contract);
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<EmployeeContract>>();
            return apiResponse?.Data;
        }

        public async Task<bool> DeleteEmployeeContractAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiEndpoints.Contracts.Base}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
