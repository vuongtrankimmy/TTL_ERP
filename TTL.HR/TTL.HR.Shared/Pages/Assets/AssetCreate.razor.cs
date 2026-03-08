using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Assets.Interfaces;
using TTL.HR.Application.Modules.Assets.Models;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;
using System.Linq;

namespace TTL.HR.Shared.Pages.Assets
{
    public partial class AssetCreate
    {
        [Parameter] public string? Id { get; set; }

        [Inject] public NavigationManager Navigation { get; set; } = default!;
        [Inject] public IAssetService AssetService { get; set; } = default!;
        [Inject] public IMasterDataService MasterDataService { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;

        private AssetModel Asset = new();
        private List<AssetCategoryDto> Categories = new();
        private List<LookupModel> Statuses = new();

        private bool IsEdit => !string.IsNullOrEmpty(Id);
        private bool _isSaving = false;
        private bool _isLoading = false;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;
            try 
            {
                // Parallel load metadata
                var categoriesTask = AssetService.GetCategoriesAsync();
                var statusTask = MasterDataService.GetCachedLookupsAsync("AssetStatus");
                
                await Task.WhenAll(categoriesTask, statusTask);
                Categories = (await categoriesTask).ToList();
                Statuses = await statusTask;

                if (IsEdit && Id != null)
                {
                    var data = await AssetService.GetAssetAsync(Id);
                    if (data != null)
                    {
                        Asset = data;
                        // For editing, ensure StatusId is synced if it's missing but we have the Status (string)
                        if (!Asset.StatusId.HasValue && !string.IsNullOrEmpty(Asset.Status))
                        {
                            Asset.StatusId = Statuses.FirstOrDefault(s => s.Code == Asset.Status)?.LookupID;
                        }
                    }
                }
                else
                {
                    Asset.PurchaseDate = DateTime.Today;
                    Asset.StatusId = Statuses.FirstOrDefault(s => s.Code == "Available")?.LookupID ?? Statuses.FirstOrDefault()?.LookupID;
                    Asset.Status = "Available";
                    Asset.CategoryId = Categories.FirstOrDefault()?.Id ?? "";
                }
            }
            catch (Exception)
            {
                await JS.InvokeVoidAsync("toastr.error", "Lỗi tải thông tin khởi tạo.");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task HandleValidSubmit()
        {
            _isSaving = true;
            try
            {
                bool success;
                if (IsEdit && Asset.Id != null)
                {
                    success = await AssetService.UpdateAssetAsync(Asset.Id, Asset);
                }
                else
                {
                    // Generate code if empty
                    if (string.IsNullOrEmpty(Asset.Code))
                    {
                        Asset.Code = $"ASSET-{DateTime.Now.Ticks.ToString().Substring(10)}";
                    }
                    success = await AssetService.CreateAssetAsync(Asset);
                }

                if (success)
                {
                    await JS.InvokeVoidAsync("toastr.success", IsEdit ? "Cập nhật tài sản thành công." : "Thêm tài sản mới thành công.");
                    Navigation.NavigateTo("/assets/inventory");
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", "Lưu thông tin thất bại.");
                }
            }
            catch (Exception)
            {
                await JS.InvokeVoidAsync("toastr.error", "Đã xảy ra lỗi khi lưu thông tin.");
            }
            finally
            {
                _isSaving = false;
            }
        }

        private void Cancel()
        {
            Navigation.NavigateTo("/assets/inventory");
        }
    }
}
