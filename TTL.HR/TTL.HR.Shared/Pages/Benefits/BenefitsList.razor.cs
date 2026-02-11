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
                "Sự kiện" or "Event" => "badge-light-info",
                "Một lần" or "One-time" => "badge-light-success",
                "Số lượng" or "Quantity" => "badge-light-warning",
                _ => "badge-light-primary"
            };
        }

        private string TranslateType(string type)
        {
            return type switch
            {
                "Monthly" => "Cố định hàng tháng",
                "Event" => "Sự kiện",
                "One-time" => "Một lần",
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
