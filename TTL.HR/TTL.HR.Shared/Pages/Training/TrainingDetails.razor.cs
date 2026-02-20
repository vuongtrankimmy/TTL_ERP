using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Shared.Components.Training;
using TTL.HR.Application.Modules.Training.Interfaces;
using TTL.HR.Application.Modules.Training.Models;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Shared.Pages.Training
{
    public partial class TrainingDetails
    {
        [Parameter] public string Id { get; set; } = "";
        
        [Inject] public ITrainingService TrainingService { get; set; } = default!;
        [Inject] public NavigationManager Navigation { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;

        private bool _isLoading = true;
        private CourseModel? Course;
        private List<ParticipantModel> Attendees = new();
        private bool IsRegisterModalOpen = false;

        // Modal Delete Control
        private bool IsDeleteModalOpen = false;
        private ParticipantModel? ItemToRemoveAttendee;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            _isLoading = true;
            try
            {
                Course = await TrainingService.GetCourseAsync(Id);
                if (Course == null)
                {
                    await JS.InvokeVoidAsync("toastr.error", "Không tìm thấy khóa đào tạo.");
                    Navigation.NavigateTo("/training");
                    return;
                }

                Attendees = Course.EnrolledEmployees ?? new List<ParticipantModel>();
            }
            catch (Exception)
            {
                await JS.InvokeVoidAsync("toastr.error", "Lỗi tải dữ liệu.");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void HandleEdit()
        {
            Navigation.NavigateTo($"/training/add?id={Id}");
        }

        private void PromptRemoveAttendee(ParticipantModel attendee)
        {
            ItemToRemoveAttendee = attendee;
            IsDeleteModalOpen = true;
        }

        private void CloseRemoveModal()
        {
            IsDeleteModalOpen = false;
            ItemToRemoveAttendee = null;
        }

        private async Task ConfirmRemoveAttendee()
        {
            if (ItemToRemoveAttendee != null)
            {
                var success = await TrainingService.RemoveParticipantAsync(Id, ItemToRemoveAttendee.EmployeeId);
                if (success)
                {
                    await JS.InvokeVoidAsync("toastr.success", "Đã hủy đăng ký học viên thành công!");
                    await LoadData();
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", "Có lỗi xảy ra khi hủy đăng ký.");
                }
                CloseRemoveModal();
            }
        }

        private void HandleRegisterAttendee()
        {
            IsRegisterModalOpen = true;
        }

        private void CloseRegisterModal()
        {
            IsRegisterModalOpen = false;
        }

        private async Task ConfirmRegister(List<TrainingRegistrationPopup.EmployeeSelectViewModel> selectedEmployees)
        {
            if (selectedEmployees == null || !selectedEmployees.Any()) return;

            var employeeIds = selectedEmployees.Select(e => e.Id).ToList();
            var result = await TrainingService.RegisterParticipantsAsync(Id, employeeIds);
            
            if (result.Success)
            {
                await JS.InvokeVoidAsync("toastr.success", result.Message ?? "Đã đăng ký học viên thành công!");
                IsRegisterModalOpen = false;
                await LoadData();
            }
            else
            {
                await JS.InvokeVoidAsync("toastr.error", result.Message ?? "Có lỗi xảy ra khi đăng ký.");
            }
            
            CloseRegisterModal();
        }
    }
}
