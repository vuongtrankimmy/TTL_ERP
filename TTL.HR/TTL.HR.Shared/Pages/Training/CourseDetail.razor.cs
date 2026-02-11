using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Training.Interfaces;
using TTL.HR.Application.Modules.Training.Models;

namespace TTL.HR.Shared.Pages.Training
{
    public partial class CourseDetail
    {
        [Parameter]
        public string CourseId { get; set; } = string.Empty;

        [Inject] public ITrainingService TrainingService { get; set; } = default!;
        [Inject] public NavigationManager Navigation { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;

        private CourseModel? Course { get; set; }
        private List<ParticipantModel> Participants { get; set; } = new();
        private bool _isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            _isLoading = true;
            try
            {
                Course = await TrainingService.GetCourseAsync(CourseId);
                if (Course == null)
                {
                    await JS.InvokeVoidAsync("toastr.error", "Không tìm thấy khóa đào tạo.");
                    Navigation.NavigateTo("/training");
                    return;
                }

                var participantsList = await TrainingService.GetParticipantsAsync(CourseId);
                Participants = participantsList.ToList();
            }
            catch (Exception)
            {
                await JS.InvokeVoidAsync("toastr.error", "Có lỗi xảy ra khi tải thông tin khóa học.");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task Enroll()
        {
            try
            {
                var success = await TrainingService.RegisterCourseAsync(CourseId);
                if (success)
                {
                    await JS.InvokeVoidAsync("toastr.success", "Đã đăng ký tham gia khóa học thành công!");
                    await LoadData();
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", "Có lỗi xảy ra khi đăng ký.");
                }
            }
            catch (Exception)
            {
                await JS.InvokeVoidAsync("toastr.error", "Có lỗi hệ thống.");
            }
        }
    }
}
