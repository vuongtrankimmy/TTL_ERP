using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.HumanResource.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Models;

namespace TTL.HR.Shared.Pages.Contracts
{
    public partial class Contract
    {
        [Inject] public NavigationManager Navigation { get; set; } = default!;
        [Inject] public IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] public IContractService ContractService { get; set; } = default!;

        private string ActiveTab = "Templates";
        private string SearchQuery = "";
        private ContractTemplateModel? selectedTemplate;
        private string deleteConfirmInput = "";

        private List<ContractTemplateModel> AllTemplates = new();
        private List<EmployeeContractModel> AllEmployeeContracts = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            AllTemplates = await ContractService.GetTemplatesAsync();
            AllEmployeeContracts = await ContractService.GetEmployeeContractsAsync();
            StateHasChanged();
        }

        private IEnumerable<ContractTemplateModel> FilteredTemplates
        {
            get
            {
                var templates = ActiveTab switch
                {
                    "Active" => AllTemplates.Where(t => t.Status == "Active"),
                    "Draft" => AllTemplates.Where(t => t.Status == "Draft"),
                    _ => AllTemplates
                };

                if (!string.IsNullOrWhiteSpace(SearchQuery))
                {
                    templates = templates.Where(t =>
                        t.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                        t.Code.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));
                }

                return templates;
            }
        }

        private IEnumerable<EmployeeContractModel> FilteredEmployeeContracts
        {
            get
            {
                var result = AllEmployeeContracts;
                if (!string.IsNullOrWhiteSpace(SearchQuery))
                {
                    result = result.Where(c =>
                        c.EmployeeName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                        c.ContractNumber.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                return result;
            }
        }

        private void SetActiveTab(string tab)
        {
            ActiveTab = tab;
        }

        private void ShowDetail(ContractTemplateModel template)
        {
            selectedTemplate = template;
            StateHasChanged();
            JSRuntime.InvokeVoidAsync("openContractDetail");
        }

        private bool IsDeleteButtonEnabled => deleteConfirmInput.ToUpper() == "XÓA";

        private async Task DeleteTemplate(ContractTemplateModel? template)
        {
            if (template == null) return;
            selectedTemplate = template;
            deleteConfirmInput = ""; 
            StateHasChanged();
            await JSRuntime.InvokeVoidAsync("showModal", "#kt_modal_delete_confirm");
        }

        private async Task ConfirmDeleteExecution()
        {
            if (selectedTemplate == null || !IsDeleteButtonEnabled) return;

            var name = selectedTemplate.Name;
            var success = await ContractService.DeleteTemplateAsync(selectedTemplate.Id);
            
            if (success)
            {
                AllTemplates.Remove(selectedTemplate);
                
                // Đóng các UI
                await JSRuntime.InvokeVoidAsync("hideAllContractUI");

                await JSRuntime.InvokeVoidAsync("Swal.fire", new
                {
                    title = "Đã xóa thành công!",
                    text = $"Mẫu hợp đồng {name} đã được loại bỏ.",
                    icon = "success",
                    buttonsStyling = false,
                    confirmButtonText = "Hoàn tất",
                    customClass = new { confirmButton = "btn fw-bold btn-primary" }
                });

                selectedTemplate = null;
                StateHasChanged();
            }
            else 
            {
                await JSRuntime.InvokeVoidAsync("Swal.fire", "Lỗi", "Không thể xóa mẫu hợp đồng. Vui lòng thử lại.", "error");
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JSRuntime.InvokeVoidAsync("eval", "KTMenu.createInstances(); KTDrawer.createInstances();");
        }
    }
}
