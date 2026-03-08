using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Shared.Pages.Settings
{
    public partial class LocalizationSettings
    {
        [Inject] public ISettingsService SettingsService { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;

        private SystemSettingsModel Model = new();
        private bool _isLoading = true;
        private bool _isSaving = false;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            _isLoading = true;
            var data = await SettingsService.GetSettingsAsync();
            if (data != null)
            {
                Model = data;
            }
            _isLoading = false;
        }

        private async Task SaveSettings()
        {
            _isSaving = true;
            Model.ActiveTab = "localization";
            var result = await SettingsService.UpdateSettingsAsync(Model);
            _isSaving = false;

            if (result != null && result.Success)
            {
                await JS.InvokeVoidAsync("toastr.success", "Cập nhật cấu hình vùng và ngôn ngữ thành công!");
            }
            else
            {
                string errorMsg = "Có lỗi xảy ra khi cập nhật cấu hình.";
                if (result != null)
                {
                    if (result.Errors != null && result.Errors.Any())
                    {
                        errorMsg = string.Join("<br/>", result.Errors);
                    }
                    else if (!string.IsNullOrEmpty(result.Message))
                    {
                        errorMsg = result.Message;
                    }
                }
                await JS.InvokeVoidAsync("toastr.error", errorMsg);
            }
        }

        private async Task ResetToDefault()
        {
            // In a real app, this might fetch defaults from server or reset local model
            await LoadData();
            await JS.InvokeVoidAsync("toastr.info", "Đã khôi phục dữ liệu từ máy chủ.");
        }
    }
}
