using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Payroll.Interfaces;
using TTL.HR.Application.Modules.Payroll.Models;

namespace TTL.HR.Shared.Pages.Benefits
{
    public partial class BenefitsList
    {
        [Inject] public IBenefitService BenefitService { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;

        private bool _showDetail = false;
        private bool _isLoading = true;
        private BenefitModel? _selectedBenefit;
        private List<BenefitModel> _benefits = new();
        private string _searchTerm = "";

        private BenefitEditModal _editModal = default!;
        private bool IsDeleteModalOpen = false;
        private BenefitModel? BenefitToDelete;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            _isLoading = true;
            try
            {
                var data = await BenefitService.GetBenefitsAsync();
                _benefits = data?.ToList() ?? new List<BenefitModel>();
            }
            catch (Exception)
            {
                await JS.InvokeVoidAsync("toastr.error", "Lỗi tải dữ liệu phúc lợi.");
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        private void CreateNewBenefit() => _editModal.OpenAsync();

        private void EditBenefit(BenefitModel item) => _editModal.OpenAsync(item.Id);

        private void PromptDeleteBenefit(BenefitModel item)
        {
            BenefitToDelete = item;
            IsDeleteModalOpen = true;
        }

        private void CloseDeleteModal()
        {
            IsDeleteModalOpen = false;
            BenefitToDelete = null;
        }

        private async Task ConfirmDelete()
        {
            if (BenefitToDelete != null && !string.IsNullOrEmpty(BenefitToDelete.Id))
            {
                var success = await BenefitService.DeleteBenefitAsync(BenefitToDelete.Id);
                if (success)
                {
                    await JS.InvokeVoidAsync("toastr.success", $"Đã xóa phúc lợi {BenefitToDelete.Name}.");
                    await LoadData();
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", "Xóa phúc lợi thất bại.");
                }
                CloseDeleteModal();
            }
        }

        private void openDetail(BenefitModel item)
        {
            _selectedBenefit = item;
            _showDetail = true;
        }

        private void closeDetail() => _showDetail = false;

        private List<BenefitModel> FilteredBenefits => string.IsNullOrWhiteSpace(_searchTerm)
            ? _benefits
            : _benefits.Where(x => x.Name.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) || 
                                   x.Code.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

        private string GetTypeBadgeClass(string type)
        {
            return type switch
            {
                "Cố định hàng tháng" or "Monthly" => "badge-light-primary",
                "Sự kiện" or "Event" or "Yearly" => "badge-light-info",
                "Một lần" or "One-time" or "OneTime" => "badge-light-success",
                "Số lượng" or "Quantity" => "badge-light-warning",
                _ => "badge-light-primary"
            };
        }

        private string TranslateType(string type)
        {
            return type switch
            {
                "Monthly" => "Cố định hàng tháng",
                "Event" or "Yearly" => "Định kỳ năm",
                "One-time" or "OneTime" => "Một lần",
                "Quantity" => "Số lượng",
                _ => type
            };
        }
        
        private string GetIconByBenefit(BenefitModel item)
        {
            if (!string.IsNullOrEmpty(item.Icon)) return item.Icon;
            
            return item.Category switch
            {
                "Allowance" => "ki-outline ki-wallet",
                "Insurance" => "ki-outline ki-shield-search",
                "Bonus" => "ki-outline ki-gift",
                _ => "ki-outline ki-briefcase"
            };
        }
    }
}
