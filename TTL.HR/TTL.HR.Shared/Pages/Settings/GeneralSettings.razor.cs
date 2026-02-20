using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Shared.Pages.Settings
{
    public partial class GeneralSettings
    {
        [Parameter]
        [SupplyParameterFromQuery]
        public string? Tab { get; set; }

        [Inject] public NavigationManager Navigation { get; set; } = default!;
        [Inject] public ISettingsService SettingsService { get; set; } = default!;
        [Inject] public IFileService FileService { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;


        private string activeTab = "company_profile";
        private SystemSettingsModel Model = new();
        private bool _isLoading = true;
        private bool _isSaving = false;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        protected override void OnParametersSet()
        {
            if (!string.IsNullOrEmpty(Tab))
            {
                activeTab = Tab;
            }
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
                await JS.InvokeVoidAsync("toastr.success", "Cập nhật cấu hình thành công!");
            }
            else
            {
                await JS.InvokeVoidAsync("toastr.error", "Có lỗi xảy ra khi cập nhật cấu hình.");
            }
        }

        private async Task HandleLogoUpload(InputFileChangeEventArgs e)
        {
            var file = e.File;
            if (file != null)
            {
                _isSaving = true;
                StateHasChanged();
                
                var url = await FileService.UploadFileAsync(file, "system");

                if (!string.IsNullOrEmpty(url))
                {
                    Model.LogoUrl = url;
                    await JS.InvokeVoidAsync("toastr.info", "Đã tải lên logo mới. Nhấn Lưu để hoàn tất.");
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", "Tải lên logo thất bại.");
                }
                
                _isSaving = false;
                StateHasChanged();
            }
        }

        private async Task HandleFaviconUpload(InputFileChangeEventArgs e)

        {
            var file = e.File;
            if (file != null)
            {
                _isSaving = true;
                StateHasChanged();
                
                var url = await FileService.UploadFileAsync(file, "system");

                if (!string.IsNullOrEmpty(url))
                {
                    Model.FaviconUrl = url;
                    await JS.InvokeVoidAsync("toastr.info", "Đã tải lên favicon mới. Nhấn Lưu để hoàn tất.");
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", "Tải lên favicon thất bại.");
                }
                
                _isSaving = false;
                StateHasChanged();
            }
        }


        private void SetActiveTab(string tab)
        {
            activeTab = tab;
            Navigation.NavigateTo($"/settings/general?Tab={tab}", false);
        }
    }
}
