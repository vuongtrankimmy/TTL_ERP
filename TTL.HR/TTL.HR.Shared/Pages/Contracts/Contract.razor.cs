using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.HumanResource.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Models;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.Common.Interfaces;

namespace TTL.HR.Shared.Pages.Contracts
{
    public partial class Contract
    {
        [Inject] public NavigationManager Navigation { get; set; } = default!;
        [Inject] public IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] public IContractService ContractService { get; set; } = default!;
        [Inject] public IMasterDataService MasterDataService { get; set; } = default!;

        private List<LookupModel> contractTypeLookups = new();
        private List<LookupModel> templateStatusLookups = new();
        private List<LookupModel> contractStatusLookups = new();

        private string FilterTypeId = "";
        private string FilterStatusId = "";
        private string SearchQuery = "";
        private ContractTemplateModel? selectedTemplate;
        private string deleteConfirmInput = "";

        private string ActiveTab = "Templates";
        private PagedResult<ContractTemplateModel> TemplateData = new();
        private PagedResult<EmployeeContractModel> ContractData = new();
        private int PageIndex = 1;
        private int PageSize = 10;
        private bool IsLoading = false;

        private long TotalTemplateCount = 0;
        private long ActiveTemplateCount = 0;
        private long DraftTemplateCount = 0;
        private long EmployeeContractCount = 0;

        protected override async Task OnInitializedAsync()
        {
            var currentLang = "vi-VN"; // Default to vi-VN
            contractTypeLookups = await MasterDataService.GetCachedLookupsAsync("ContractType", currentLang);
            templateStatusLookups = await MasterDataService.GetCachedLookupsAsync("TemplateStatus", currentLang);
            contractStatusLookups = await MasterDataService.GetCachedLookupsAsync("ContractStatus", currentLang);

            await LoadCounts();
            await LoadData();
        }

        private async Task LoadData()
        {
            IsLoading = true;
            try
            {
                var currentLang = "vi-VN"; // Default
                if (ActiveTab.StartsWith("Templates") || ActiveTab == "All" || ActiveTab == "Active" || ActiveTab == "Draft")
                {
                    var status = ActiveTab == "Templates" || ActiveTab == "All" ? FilterStatusId : ActiveTab;
                    TemplateData = await ContractService.GetTemplatesAsync(PageIndex, PageSize, SearchQuery, status, FilterTypeId, currentLang);
                }
                else if (ActiveTab == "EmployeeContracts")
                {
                    ContractData = await ContractService.GetEmployeeContractsAsync(PageIndex, PageSize, SearchQuery, FilterStatusId, FilterTypeId, null, currentLang);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
            IsLoading = false;
            StateHasChanged();
        }

        private async Task LoadCounts()
        {
            try
            {
                var summary = await ContractService.GetTemplateSummaryAsync();
                TotalTemplateCount = summary.TotalCount;
                ActiveTemplateCount = summary.ActiveCount;
                DraftTemplateCount = summary.DraftCount;

                // For Employee Contracts, we can fetch just 1 item to get the TotalCount
                var employees = await ContractService.GetEmployeeContractsAsync(1, 1);
                EmployeeContractCount = employees.TotalCount;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading counts: {ex.Message}");
            }
        }

        private async Task ChangePage(int page)
        {
            PageIndex = page;
            await LoadData();
        }

        private async Task ApplyFilters()
        {
            PageIndex = 1;
            await LoadData();
        }

        private async Task ResetFilters()
        {
            FilterTypeId = "";
            FilterStatusId = "";
            PageIndex = 1;
            await LoadData();
        }

        private async Task OnSearchChanged(ChangeEventArgs e)
        {
            SearchQuery = e.Value?.ToString() ?? "";
            PageIndex = 1;
            await LoadData();
        }



        private async Task SetActiveTab(string tab)
        {
            ActiveTab = tab;
            PageIndex = 1;
            SearchQuery = "";
            
            // Clear current data if switching to a different entity type to avoid UI flickers 
            // with mismatched data during loading
            if (tab == "EmployeeContracts")
            {
                TemplateData = new();
            }
            else
            {
                ContractData = new();
            }

            StateHasChanged(); // Show loading state immediately
            await LoadData();
        }

        private async Task ShowDetail(ContractTemplateModel template)
        {
            try
            {
                selectedTemplate = template;
                StateHasChanged();
                await JSRuntime.InvokeVoidAsync("openContractDetail");

                // Fetch full details (e.g. content) from API
                var fullDetails = await ContractService.GetTemplateAsync(template.Id);
                if (fullDetails != null)
                {
                    selectedTemplate = fullDetails;
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error showing detail: {ex.Message}");
                await JSRuntime.InvokeVoidAsync("Swal.fire", "Thông báo", "Không thể tải chi tiết mẫu hợp đồng.", "error");
            }
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
                await LoadCounts();
                await LoadData();

                
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
