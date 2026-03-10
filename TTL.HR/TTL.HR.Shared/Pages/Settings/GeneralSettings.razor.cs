using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.HumanResource.Interfaces;

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
        [Inject] public IEmployeeService EmployeeService { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;
        [Inject] public IMasterDataService MasterDataService { get; set; } = default!;


        private string activeTab = "company_profile";
        private string activeTranslationTab = "vi-VN";
        private SystemSettingsModel Model = new();
        private List<CodeGeneratorConfigDto> CodeConfigs = new();
        private CodeGeneratorConfigDto? SelectedConfig;
        
        private List<LookupModel> TimeZoneLookups = new();
        private List<LookupModel> LanguageLookups = new();
        private List<LookupModel> CurrencyLookups = new();
        private List<LookupModel> ThousandSeparatorLookups = new();
        private List<LookupModel> DecimalSeparatorLookups = new();
        private List<LookupModel> CodeSeparatorLookups = new();
        private List<LookupModel> CodeDateFormatLookups = new();
        private bool _isLoading = true;
        private bool _isSaving = false;
        private bool _isAccruing = false;
        public int SelectedYear { get; set; } = DateTime.Now.Year;

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
            var tzTask = MasterDataService.GetLookupsAsync("TimeZone");
            var langTask = MasterDataService.GetLookupsAsync("Language");
            var curTask = MasterDataService.GetLookupsAsync("Currency");
            var thouTask = MasterDataService.GetLookupsAsync("ThousandSeparator");
            var decTask = MasterDataService.GetLookupsAsync("DecimalSeparator");
            var codeSepTask = MasterDataService.GetLookupsAsync("CodeSeparator");
            var codeDateTask = MasterDataService.GetLookupsAsync("CodeDateFormat");

            await Task.WhenAll(dataTask, codeConfigsTask, tzTask, langTask, curTask, thouTask, decTask, codeSepTask, codeDateTask);

            if (dataTask.Result != null)
            {
                Model = dataTask.Result;
                if (Model.WorkDays == null) Model.WorkDays = new List<string>();
                if (Model.Holidays == null) Model.Holidays = new List<HolidayConfigModel>();
            }
            if (codeConfigsTask.Result != null)
            {
                CodeConfigs = codeConfigsTask.Result;
                if(CodeConfigs.Any() && SelectedConfig == null) {
                    SelectedConfig = CodeConfigs.First();
                }
            }

            TimeZoneLookups = tzTask.Result ?? new();
            LanguageLookups = langTask.Result ?? new();
            CurrencyLookups = curTask.Result ?? new();
            ThousandSeparatorLookups = thouTask.Result ?? new();
            DecimalSeparatorLookups = decTask.Result ?? new();
            CodeSeparatorLookups = codeSepTask.Result ?? new();
            CodeDateFormatLookups = codeDateTask.Result ?? new();

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

        private void AddHoliday()
        {
            if (Model.Holidays == null) Model.Holidays = new List<HolidayConfigModel>();
            Model.Holidays.Add(new HolidayConfigModel
            {
                Name = "Nghỉ lễ mới",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today,
                IsRecurring = false
            });
        }

        private void RemoveHoliday(HolidayConfigModel holiday)
        {
            if (Model.Holidays != null && Model.Holidays.Contains(holiday))
            {
                Model.Holidays.Remove(holiday);
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

            TTL.HR.Application.Modules.Common.Models.ApiResponse<bool> result = null;
            if (activeTab == "code_generator")
            {
                result = await SettingsService.UpdateCodeGeneratorConfigsAsync(CodeConfigs);
            }
            else
            {
                Model.ActiveTab = activeTab;
                result = await SettingsService.UpdateSettingsAsync(Model);
            }

            _isSaving = false;
            StateHasChanged();

            if (result != null && result.Success)
            {
                await JS.InvokeVoidAsync("toastr.success", "Cập nhật cấu hình thành công!");
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

        private async Task HandleAccrueLeave()
        {
            if (_isAccruing) return;
            
            _isAccruing = true;
            StateHasChanged();

            try
            {
                var error = await EmployeeService.AccrueLeaveAsync(DateTime.Now.Month, DateTime.Now.Year);
                if (string.IsNullOrEmpty(error))
                {
                    await JS.InvokeVoidAsync("toastr.success", $"Đã cộng phép tháng {DateTime.Now.Month} cho toàn bộ nhân viên thành công!");
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", $"Lỗi: {error}");
                }
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("toastr.error", $"Lỗi hệ thống: {ex.Message}");
            }
            finally
            {
                _isAccruing = false;
                StateHasChanged();
            }
        }

        private void AddTaxStep()
        {
            if (Model.PitSteps == null)
                Model.PitSteps = new List<TaxStepModel>();

            Model.PitSteps.Add(new TaxStepModel
            {
                Threshold = 0,
                Rate = 0,
                Deduction = 0
            });
        }

        private void RemoveTaxStep(TaxStepModel step)
        {
            if (Model.PitSteps != null && Model.PitSteps.Contains(step))
            {
                Model.PitSteps.Remove(step);
            }
        }

        private void AddNavigationItem()
        {
            if (Model.SidebarMenu == null) Model.SidebarMenu = new List<NavItem>();
            int nextId = GetNextNumericId();
            Model.SidebarMenu.Add(new NavItem
            {
                NumericId = nextId,
                Title = "Menu_NewItem",
                Icon = "ki-outline ki-abstract-26",
                Order = Model.SidebarMenu.Count + 1,
                IsActive = true
            });
        }

        private void AddSubNavigationItem(NavItem parent)
        {
            if (parent.SubItems == null) parent.SubItems = new List<NavItem>();
            int nextId = GetNextNumericId();
            parent.SubItems.Add(new NavItem
            {
                NumericId = nextId,
                Title = "Menu_NewSubItem",
                Icon = "ki-outline ki-right",
                Order = parent.SubItems.Count + 1,
                IsActive = true
            });
        }

        private void AddTranslation()
        {
            if (Model.Translations == null) Model.Translations = new List<LanguageTranslationModel>();
            Model.Translations.Add(new LanguageTranslationModel
            {
                NavigationID = 0,
                LanguageCode = activeTranslationTab,
                Value = "Tên mới"
            });
        }

        private int GetNextNumericId()
        {
            var allItems = new List<NavItem>();
            if (Model.SidebarMenu != null)
            {
                FlattenMenuForId(Model.SidebarMenu, allItems);
            }
            return allItems.Any() ? allItems.Max(i => i.NumericId) + 1 : 1;
        }

        private void FlattenMenuForId(List<NavItem> items, List<NavItem> result)
        {
            foreach (var item in items)
            {
                result.Add(item);
                if (item.HasSubItems) FlattenMenuForId(item.SubItems, result);
            }
        }

        private void RemoveNavigationItem(List<NavItem> list, NavItem item)
        {
            list.Remove(item);
        }
    }
}
