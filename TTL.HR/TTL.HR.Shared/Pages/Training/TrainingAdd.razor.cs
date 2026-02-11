using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using TTL.HR.Application.Modules.Training.Interfaces;
using TTL.HR.Application.Modules.Training.Models;
using Microsoft.JSInterop;

namespace TTL.HR.Shared.Pages.Training
{
    public partial class TrainingAdd
    {
        [Inject] private ITrainingService TrainingService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        [Parameter] [SupplyParameterFromQuery] public string? Id { get; set; }

        private CourseModel _course = new();
        private bool _isEdit = false;
        private bool _isSaving = false;

        protected override async Task OnInitializedAsync()
        {
            if (!string.IsNullOrEmpty(Id))
            {
                _isEdit = true;
                var result = await TrainingService.GetCourseAsync(Id);
                if (result != null)
                {
                    _course = result;
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", "Không tìm thấy khóa học.");
                    Navigation.NavigateTo("/training");
                }
            }
        }

        private async Task SaveCourse()
        {
            _isSaving = true;
            try
            {
                bool success;
                if (_isEdit)
                {
                    success = await TrainingService.UpdateCourseAsync(Id!, _course);
                }
                else
                {
                    success = await TrainingService.CreateCourseAsync(_course);
                }

                if (success)
                {
                    await JS.InvokeVoidAsync("toastr.success", "Đã lưu khóa học thành công!");
                    Navigation.NavigateTo("/training");
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", "Có lỗi xảy ra khi lưu khóa học.");
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
