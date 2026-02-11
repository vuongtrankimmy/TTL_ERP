using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Assets.Interfaces;
using TTL.HR.Application.Modules.Assets.Models;

namespace TTL.HR.Shared.Pages.Assets
{
    public partial class AssetDetail
    {
        [Parameter] public string Id { get; set; } = string.Empty;

        [Inject] public NavigationManager Navigation { get; set; } = default!;
        [Inject] public IAssetService AssetService { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;

        private AssetModel? Asset;
        private List<HistoryViewModel> AllocationHistory = new();
        private bool _isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            _isLoading = true;
            try
            {
                Asset = await AssetService.GetAssetAsync(Id);
                if (Asset != null)
                {
                    // Mock history for now since API might not have it yet
                    AllocationHistory = new List<HistoryViewModel>
                    {
                        new HistoryViewModel { Date = Asset.PurchaseDate ?? DateTime.Today.AddYears(-1), Action = "Nhập kho", Description = "Mua mới từ nhà cung cấp", Actor = "Hệ thống" },
                        new HistoryViewModel { Date = DateTime.Today.AddMonths(-6), Action = "Bàn giao", Description = $"Bàn giao cho {Asset.AssignedToName ?? "nhân viên"}", Actor = "Admin" }
                    };
                }
            }
            catch (Exception)
            {
                await JS.InvokeVoidAsync("toastr.error", "Lỗi tải chi tiết tài sản.");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void EditAsset()
        {
            Navigation.NavigateTo($"/assets/edit/{Id}");
        }

        private string GetStatusColor(string status)
        {
            return status switch
            {
                "Available" => "success",
                "Assigned" => "primary",
                "Maintenance" => "warning",
                "Broken" => "danger",
                "Lost" => "danger",
                _ => "secondary"
            };
        }

        private string TranslateStatus(string status)
        {
            return status switch
            {
                "Available" => "Sẵn dùng",
                "Assigned" => "Đã bàn giao",
                "Maintenance" => "Đang bảo trì",
                "Broken" => "Đã hỏng",
                "Lost" => "Đã mất",
                _ => status
            };
        }

        public class HistoryViewModel
        {
            public DateTime Date { get; set; }
            public string Action { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Actor { get; set; } = string.Empty;
        }
    }
}
