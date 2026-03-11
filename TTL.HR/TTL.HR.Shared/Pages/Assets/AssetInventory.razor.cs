using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Assets.Interfaces;
using TTL.HR.Application.Modules.Assets.Models;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Shared.Pages.Assets
{
    public partial class AssetInventory
    {
        [Inject] public NavigationManager Navigation { get; set; } = default!;
        [Inject] public IAssetService AssetService { get; set; } = default!;
        [Inject] public IMasterDataService MasterDataService { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;
        [Inject] public INavigationService NavigationService { get; set; } = default!;

        private List<AssetViewModel> AssetList = new();
        private List<LookupModel> StatusLookups = new();
        private bool IsDeleteModalOpen = false;
        private AssetViewModel? AssetToDelete;
        private bool _isLoading = true;
        private string _searchTerm = "";

        protected override async Task OnInitializedAsync()
        {
            if (!await NavigationService.UserHasPermissionAsync("Permissions.Assets.View"))
            {
                Navigation.NavigateTo("/access-denied");
                return;
            }
            await LoadData();
        }

        private async Task LoadData()
        {
            _isLoading = true;
            try
            {
                var assetsTask = AssetService.GetAssetsAsync(null, 100);
                var lookupsTask = MasterDataService.GetCachedLookupsAsync("AssetStatus");
                
                await Task.WhenAll(assetsTask, lookupsTask);

                var assets = await assetsTask;
                StatusLookups = await lookupsTask;
                if (assets != null)
                {
                    AssetList = assets.Select(a => new AssetViewModel
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Code = a.Code,
                        Type = a.Type,
                        Category = !string.IsNullOrEmpty(a.CategoryName) ? a.CategoryName : (!string.IsNullOrEmpty(a.Category) ? a.Category : "Của công ty"),
                        AssignedTo = a.AssignedToName ?? "Chưa bàn giao",
                        Status = TranslateStatus(a.Status, a.StatusId),
                        StatusKey = a.Status ?? "Available",
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

        private string TranslateStatus(string? status, int? statusId = null)
        {
            if (statusId.HasValue)
            {
                var lookup = StatusLookups.FirstOrDefault(l => l.LookupID == statusId.Value);
                if (lookup != null) return lookup.Name;
            }

            if (string.IsNullOrEmpty(status)) return "Sẵn dùng";

            var lByName = StatusLookups.FirstOrDefault(l => string.Equals(l.Code, status, StringComparison.OrdinalIgnoreCase));
            if (lByName != null) return lByName.Name;

            return status.ToLower().Trim() switch
            {
                "available" => "Sẵn dùng",
                "assigned" => "Đã bàn giao",
                "maintenance" => "Đang bảo trì",
                "broken" => "Đã hỏng",
                "lost" => "Đã mất",
                _ => status
            };
        }

        private string GetStatusColor(string? status)
        {
            if (string.IsNullOrEmpty(status)) return "success";

            return status.ToLower().Trim() switch
            {
                "available" => "success",
                "assigned" => "primary",
                "maintenance" => "warning",
                "broken" => "danger",
                "lost" => "danger",
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
