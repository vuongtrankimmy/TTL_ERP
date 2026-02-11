using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using TTL.HR.Application.Modules.Recruitment;
using TTL.HR.Application.Modules.Recruitment.Models;
using Microsoft.JSInterop;

namespace TTL.HR.Shared.Pages.Recruitment
{
    public partial class RecruitmentAdd
    {
        [Inject] public IRecruitmentApplication RecruitmentApp { get; set; } = default!;
        [Inject] public NavigationManager Navigation { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;

        [Parameter, SupplyParameterFromQuery]
        public string? Id { get; set; }

        private JobDetail Model = new();
        private bool _isEdit => !string.IsNullOrEmpty(Id);
        private bool _isLoading = false;
        private bool _isSaving = false;

        protected override async Task OnInitializedAsync()
        {
            if (_isEdit)
            {
                _isLoading = true;
                var result = await RecruitmentApp.GetJobDetailsAsync(Id!);
                if (result != null)
                {
                    Model = result;
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", "Không tìm thấy tin tuyển dụng.");
                    Navigation.NavigateTo("/recruitment");
                }
                _isLoading = false;
            }
            else
            {
                Model = new JobDetail { EndDate = DateTime.Today.AddMonths(1) };
            }
        }

        private async Task SaveRecruitment()
        {
            _isSaving = true;
            try 
            {
                bool success;
                if (_isEdit)
                {
                    success = await RecruitmentApp.UpdateJobPostingAsync(Id!, Model);
                }
                else
                {
                    success = await RecruitmentApp.PostNewJobAsync(Model);
                }

                if (success)
                {
                    await JS.InvokeVoidAsync("toastr.success", "Đã lưu tin tuyển dụng thành công!");
                    Navigation.NavigateTo("/recruitment");
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", "Có lỗi xảy ra khi lưu tin tuyển dụng.");
                }
            }
            catch (Exception)
            {
                await JS.InvokeVoidAsync("toastr.error", "Có lỗi hệ thống.");
            }
            finally
            {
                _isSaving = false;
            }
        }
    }
}
