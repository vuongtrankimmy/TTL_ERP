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

        // Modal states
        private bool IsRevokeModalVisible = false;
        private bool IsMaintenanceModalVisible = false;

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
                    var history = await AssetService.GetAssetHistoryAsync(Id);
                    AllocationHistory = history.Select(h => new HistoryViewModel 
                    { 
                        Date = h.Date, 
                        Action = h.Action, 
                        Description = h.Description, 
                        Actor = h.Actor 
                    }).ToList();
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

        private void ShowRevokeModal()
        {
            IsRevokeModalVisible = true;
        }

        private void ShowMaintenanceModal()
        {
            IsMaintenanceModalVisible = true;
        }

        private async Task HandleRevokeConfirmed(string reason)
        {
            IsRevokeModalVisible = false;
            if (Asset == null) return;

            var success = await AssetService.ReturnAssetAsync(Id, "Bình thường", string.IsNullOrWhiteSpace(reason) ? "Thu hồi định kỳ" : reason);
            if (success)
            {
                await JS.InvokeVoidAsync("toastr.success", "Đã thu hồi tài sản thành công!");
                await LoadData();
            }
            else
            {
                await JS.InvokeVoidAsync("toastr.error", "Có lỗi xảy ra khi thu hồi tài sản.");
            }
        }

        private async Task HandleMaintenanceConfirmed(string issue)
        {
            IsMaintenanceModalVisible = false;
            if (Asset == null) return;

            var success = await AssetService.RequestMaintenanceAsync(Id, string.IsNullOrWhiteSpace(issue) ? "Yêu cầu bảo trì từ người dùng" : issue, "Urgent");
            if (success)
            {
                await JS.InvokeVoidAsync("toastr.success", "Đã gửi yêu cầu bảo trì thành công!");
                await LoadData();
            }
            else
            {
                await JS.InvokeVoidAsync("toastr.error", "Có lỗi xảy ra khi gửi yêu cầu bảo trì.");
            }
        }

        private async Task RevokeAsset() // Keeping this for backward compatibility if needed, but we'll use ShowRevokeModal
        {
            ShowRevokeModal();
        }

        private async Task RequestMaintenance()
        {
            ShowMaintenanceModal();
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

        private string GetAssetIcon(string? type) => type?.ToLower() switch
        {
            "laptop" or "máy tính" => "ki-outline ki-laptop",
            "mobile" or "điện thoại" => "ki-outline ki-phone",
            "vehicle" or "xe" => "ki-outline ki-car",
            "furniture" or "nội thất" => "ki-outline ki-home",
            "tablet" => "ki-outline ki-tablet",
            _ => "ki-outline ki-briefcase"
        };

        public class HistoryViewModel
        {
            public DateTime Date { get; set; }
            public string Action { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Actor { get; set; } = string.Empty;
        }
    }
}
