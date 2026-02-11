using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Shared.Pages.Settings
{
    public partial class PayrollConfig
    {
        [Inject] public ISettingsService SettingsService { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;

        private SystemSettingsModel Model = new();
        private bool _isLoading = true;
        private bool _isSaving = false;
        private string activeTab = "insurance";

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
            var success = await SettingsService.UpdateSettingsAsync(Model);
            _isSaving = false;

            if (success)
            {
                await JS.InvokeVoidAsync("toastr.success", "Cập nhật tham số tính lương thành công!");
            }
            else
            {
                await JS.InvokeVoidAsync("toastr.error", "Có lỗi xảy ra khi cập nhật cấu hình.");
            }
        }

        private void SetActiveTab(string tab)
        {
            activeTab = tab;
        }
    }
}
