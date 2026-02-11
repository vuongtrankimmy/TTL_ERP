using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Assets.Interfaces;
using TTL.HR.Application.Modules.Assets.Models;

namespace TTL.HR.Shared.Pages.Assets
{
    public partial class AssetCreate
    {
        [Parameter] public string? Id { get; set; }

        [Inject] public NavigationManager Navigation { get; set; } = default!;
        [Inject] public IAssetService AssetService { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;

        private AssetModel Asset = new();
        private bool IsEdit => !string.IsNullOrEmpty(Id);
        private bool _isSaving = false;
        private bool _isLoading = false;

        protected override async Task OnInitializedAsync()
        {
            if (IsEdit && Id != null)
            {
                _isLoading = true;
                try
                {
                    var data = await AssetService.GetAssetAsync(Id);
                    if (data != null)
                    {
                        Asset = data;
                    }
                }
                catch (Exception)
                {
                    await JS.InvokeVoidAsync("toastr.error", "Lỗi tải thông tin tài sản.");
                }
                finally
                {
                    _isLoading = false;
                }
            }
            else
            {
                Asset.PurchaseDate = DateTime.Today;
                Asset.Status = "Available";
                Asset.Category = "Laptop"; // Default
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
