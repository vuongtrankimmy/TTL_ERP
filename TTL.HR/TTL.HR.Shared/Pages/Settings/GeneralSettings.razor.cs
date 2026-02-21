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
        private List<CodeGeneratorConfigDto> CodeConfigs = new();
        private CodeGeneratorConfigDto? SelectedConfig;
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
            StateHasChanged();

            var dataTask = SettingsService.GetSettingsAsync();
            var codeConfigsTask = SettingsService.GetCodeGeneratorConfigsAsync();

            await Task.WhenAll(dataTask, codeConfigsTask);

            if (dataTask.Result != null)
            {
                Model = dataTask.Result;
            }
            if (codeConfigsTask.Result != null)
            {
                CodeConfigs = codeConfigsTask.Result;
                if(CodeConfigs.Any() && SelectedConfig == null) {
                    SelectedConfig = CodeConfigs.First();
                }
            }

            _isLoading = false;
        }

        private void SelectConfig(CodeGeneratorConfigDto config)
        {
            SelectedConfig = config;
        }

        private void AddNewConfig()
        {
            var newConfig = new CodeGeneratorConfigDto
            {
                Name = "Cấu hình mới",
                EntityType = "NewEntity",
                Prefix = "NEW",
                Length = 6,
                IsActive = true
            };
            CodeConfigs.Add(newConfig);
            SelectedConfig = newConfig;
        }

        private async Task DeleteConfig(CodeGeneratorConfigDto config)
        {
            // Simple remove from local list. 
            // The actual delete on DB will happen when we update the handler or we can call a specific delete API.
            // For now, let's remove from list and user must "Save" to persist (or we can add a Delete API).
            // To be safe, I will implement a confirmation later or just remove for now.
            CodeConfigs.Remove(config);
            if (SelectedConfig == config)
            {
                SelectedConfig = CodeConfigs.FirstOrDefault();
            }
        }

        private string GeneratePreview(CodeGeneratorConfigDto? config, int offset = 0)
        {
            if (config == null) return "---";

            var sb = new System.Text.StringBuilder();
            
            // Prefix
            sb.Append(config.Prefix);
            
            // Separator
            if (!string.IsNullOrEmpty(config.Separator))
                sb.Append(config.Separator);

            // Date
            if (config.IncludeDate && !string.IsNullOrEmpty(config.DateFormat))
            {
                sb.Append(DateTime.Now.ToString(config.DateFormat));
                if (!string.IsNullOrEmpty(config.Separator))
                    sb.Append(config.Separator);
            }

            // Sequence
            long seq = config.CurrentSequence + 1 + offset;
            sb.Append(seq.ToString().PadLeft(config.Length, '0'));

            // Suffix
            sb.Append(config.Suffix);

            return sb.ToString();
        }

        private async Task SaveSettings()
        {
            _isSaving = true;
            StateHasChanged();

            bool success = false;
            if (activeTab == "code_generator")
            {
                success = await SettingsService.UpdateCodeGeneratorConfigsAsync(CodeConfigs);
            }
            else
            {
                success = await SettingsService.UpdateSettingsAsync(Model);
            }

            _isSaving = false;
            StateHasChanged();

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
