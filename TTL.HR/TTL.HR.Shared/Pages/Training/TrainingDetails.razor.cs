using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using TTL.HR.Shared.Components.Training;

namespace TTL.HR.Shared.Pages.Training
{
    public partial class TrainingDetails
    {
        [Parameter] public int Id { get; set; }
        private bool _isLoading = true;
        private CourseDetailViewModel Course;
        private List<AttendeeViewModel> Attendees = new();
        private bool IsRegisterModalOpen = false;

        // Modal Delete Control
        private bool IsDeleteModalOpen = false;
        private AttendeeViewModel ItemToRemoveAttendee;

        [Inject] public NavigationManager Navigation { get; set; }

        private void HandleEdit()
        {
            Navigation.NavigateTo($"/training/add?id={Id}");
        }

        private void PromptRemoveAttendee(AttendeeViewModel attendee)
        {
            ItemToRemoveAttendee = attendee;
            IsDeleteModalOpen = true;
        }

        private void CloseRemoveModal()
        {
            IsDeleteModalOpen = false;
            ItemToRemoveAttendee = null;
        }

        private void ConfirmRemoveAttendee()
        {
            if (ItemToRemoveAttendee != null)
            {
                Attendees.Remove(ItemToRemoveAttendee);
                Course.ParticipantsCount--;
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

        private void ConfirmRegister(List<TrainingRegistrationPopup.EmployeeSelectViewModel> selectedEmployees)
        {
            foreach (var emp in selectedEmployees)
            {
                // Simulate adding attendees
                Attendees.Add(new AttendeeViewModel { 
                    Name = emp.FullName, 
                    Department = emp.Department, 
                    Email = $"{emp.Code.ToLower()}@company.com", 
                    Avatar = "assets/media/avatars/300-1.jpg", 
                    Status = "Joined", 
                    StatusBadge = "badge-light-success", 
                    Score = "--" 
                });
                Course.ParticipantsCount++;
            }
            CloseRegisterModal();
        }

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            await System.Threading.Tasks.Task.Delay(800);
            
            // Mock fetching detail based on Id (Normally calling a Service)
            Course = new CourseDetailViewModel
            {
                Id = Id,
                Title = "Hội Nhập Văn Hóa Doanh Nghiệp (Onboarding)",
                Code = "TRN-ONB",
                Description = "Khóa học bắt buộc cho nhân viên mới về quy định chung, tầm nhìn, sứ mệnh và văn hóa ứng xử tại công ty.",
                Trainer = "Nguyễn Thị Mai (HR)",
                Location = "Phòng đào tạo tầng 4 / Zoom Online",
                StartDate = new DateTime(2026, 3, 10),
                Duration = "4 giờ",
                ParticipantsCount = 15,
                MaxParticipants = 20,
                Status = "Active",
                StatusClass = "badge-light-success",
                IsMandatory = true
            };

            Attendees = new List<AttendeeViewModel>
            {
                new() { Name = "Phan Thanh Tùng", Department = "Phòng Kỹ thuật", Email = "tung.pt@company.com", Avatar = "assets/media/avatars/300-1.jpg", Status = "Joined", StatusBadge = "badge-light-success", Score = "8.5" },
                new() { Name = "Nguyễn Văn Lộc", Department = "Kinh doanh", Email = "loc.nv@company.com", Avatar = "assets/media/avatars/300-2.jpg", Status = "Pending", StatusBadge = "badge-light-warning", Score = "--" },
                new() { Name = "Lê Thị Hồng", Department = "Nhân sự", Email = "hong.lt@company.com", Avatar = "assets/media/avatars/300-3.jpg", Status = "Joined", StatusBadge = "badge-light-success", Score = "9.0" }
            };

            _isLoading = false;
        }

        public class CourseDetailViewModel
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Code { get; set; }
            public string Description { get; set; }
            public string Trainer { get; set; }
            public string Location { get; set; }
            public DateTime StartDate { get; set; }
            public string Duration { get; set; }
            public int ParticipantsCount { get; set; }
            public int MaxParticipants { get; set; }
            public string Status { get; set; }
            public string StatusClass { get; set; }
            public bool IsMandatory { get; set; }
        }

        public class AttendeeViewModel
        {
            public string Name { get; set; }
            public string Department { get; set; }
            public string Email { get; set; }
            public string Avatar { get; set; }
            public string Status { get; set; }
            public string StatusBadge { get; set; }
            public string Score { get; set; }
        }
    }
}
