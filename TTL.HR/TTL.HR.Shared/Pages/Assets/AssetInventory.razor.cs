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
    public partial class AssetInventory
    {
        [Inject] public NavigationManager Navigation { get; set; } = default!;
        [Inject] public IAssetService AssetService { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;

        private List<AssetViewModel> AssetList = new();
        private bool IsDeleteModalOpen = false;
        private AssetViewModel? AssetToDelete;
        private bool _isLoading = true;
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
                var assets = await AssetService.GetAssetsAsync();
                if (assets != null)
                {
                    AssetList = assets.Select(a => new AssetViewModel
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Code = a.Code,
                        Type = a.Type,
                        Category = a.Category,
                        AssignedTo = a.AssignedToName ?? "Chưa bàn giao",
                        Status = TranslateStatus(a.Status),
                        StatusKey = a.Status,
                        PurchaseDate = a.PurchaseDate
                    }).ToList();
                }
            }
            catch (Exception)
            {
                await JS.InvokeVoidAsync("toastr.error", "Lỗi tải kho tài sản.");
            }
            finally
            {
                _isLoading = false;
            }
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
        
        private void CreateNewAsset() 
        { 
            Navigation.NavigateTo("/assets/add");
        }

        private void ViewDetail(string id)
        {
            Navigation.NavigateTo($"/assets/detail/{id}");
        }

        private void EditAsset(string id)
        {
            Navigation.NavigateTo($"/assets/edit/{id}");
        }

        private void PromptDeleteAsset(AssetViewModel asset)
        {
            AssetToDelete = asset;
            IsDeleteModalOpen = true;
        }

        private void CloseDeleteModal()
        {
            IsDeleteModalOpen = false;
            AssetToDelete = null;
        }

        private async Task ConfirmDelete()
        {
            if (AssetToDelete != null)
            {
                var success = await AssetService.DeleteAssetAsync(AssetToDelete.Id);
                if (success)
                {
                    await JS.InvokeVoidAsync("toastr.success", $"Đã xóa tài sản {AssetToDelete.Code}.");
                    await LoadData();
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", "Xóa tài sản thất bại.");
                }
                CloseDeleteModal();
            }
        }

        private IEnumerable<AssetViewModel> FilteredAssets =>
            string.IsNullOrWhiteSpace(_searchTerm)
            ? AssetList
            : AssetList.Where(a => a.Name.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) || a.Code.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase));

        public class AssetViewModel
        {
            public string Id { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Code { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public string AssignedTo { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string StatusKey { get; set; } = string.Empty;
            public DateTime? PurchaseDate { get; set; }
        }
    }
}
