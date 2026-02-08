using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using TTL.HR.Shared.Components.Training;

namespace TTL.HR.Shared.Pages.Training
{
    public partial class TrainingList
    {
        private bool _isLoading = true;
        private string SearchTerm = "";
        private string SelectedType = "Tất cả";
        private bool _isGridView = false;
        
        // Modal Registration Control
        private bool IsRegisterModalOpen = false;
        private TrainingItemViewModel ItemToRegister;

        // Modal Delete Control
        private bool IsDeleteModalOpen = false;
        private TrainingItemViewModel ItemToDelete;

        [Microsoft.AspNetCore.Components.Inject]
        public Microsoft.AspNetCore.Components.NavigationManager Navigation { get; set; }

        [Microsoft.AspNetCore.Components.Inject]
        public IJSRuntime JS { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await Task.Delay(800);
            _isLoading = false;
        }

        private void SetViewMode(bool isGrid)
        {
            _isGridView = isGrid;
        }

        private void HandleEdit(int id)
        {
            Navigation.NavigateTo($"/training/add?id={id}");
        }

        private void PromptDelete(TrainingItemViewModel course)
        {
            ItemToDelete = course;
            IsDeleteModalOpen = true;
        }

        private void CloseDeleteModal()
        {
            IsDeleteModalOpen = false;
            ItemToDelete = null;
        }

        private async Task ConfirmDelete()
        {
            if (ItemToDelete != null)
            {
                _trainings.Remove(ItemToDelete);
                CloseDeleteModal();
                await JS.InvokeVoidAsync("toastr.success", "Đã xóa khóa đào tạo thành công!");
            }
        }

        private void HandleDelete(TrainingItemViewModel course)
        {
            PromptDelete(course);
        }

        private void HandleRegister(TrainingItemViewModel course)
        {
            if (course == null || (course.MaxParticipants > 0 && course.Participants >= course.MaxParticipants))
            {
                return;
            }
            ItemToRegister = course;
            IsRegisterModalOpen = true;
            StateHasChanged();
        }

        private void CloseRegisterModal()
        {
            IsRegisterModalOpen = false;
            ItemToRegister = null;
        }

        private async Task ConfirmRegister(List<TrainingRegistrationPopup.EmployeeSelectViewModel> selectedEmployees)
        {
            if (ItemToRegister != null)
            {
                int available = ItemToRegister.MaxParticipants - ItemToRegister.Participants;
                int toAdd = Math.Min(selectedEmployees.Count, available);
                
                ItemToRegister.Participants += toAdd;
                CloseRegisterModal();
                await JS.InvokeVoidAsync("toastr.success", $"Đã đăng ký thành công {toAdd} học viên vào khóa học!");
            }
        }

        private List<TrainingItemViewModel> _trainings = new()
        {
            new() { Id = 1, Title = "Hội Nhập Văn Hóa Doanh Nghiệp", Code = "TRN-ONB", Category = "Onboarding", Trainer = "Nguyễn Thị Mai (HR)", Duration = "4h", Type = "Internal", IsMandatory = true, Status = "Active", Participants = 15, MaxParticipants = 20 },
            new() { Id = 2, Title = "Kỹ Năng Giao Tiếp Hiệu Quả", Code = "SOFT-COMM", Category = "Soft Skills", Trainer = "Dr. Lê Thẩm Dương", Duration = "8h", Type = "External", IsMandatory = false, Status = "Active", Participants = 45, MaxParticipants = 50 },
            new() { Id = 3, Title = "Microsoft Excel Nâng Cao", Code = "TECH-EXCEL", Category = "Technical", Trainer = "TT Tin học KHTN", Duration = "16h", Type = "External", IsMandatory = true, Status = "Active", Participants = 12, MaxParticipants = 15 },
            new() { Id = 4, Title = "Lãnh Đạo Đội Nhóm (Leadership)", Code = "MGT-LEAD", Category = "Management", Trainer = "Dale Carnegie VN", Duration = "24h", Type = "External", IsMandatory = true, Status = "Active", Participants = 8, MaxParticipants = 10 },
            new() { Id = 5, Title = ".NET Core & Microservices", Code = "TECH-NET", Category = "Technical", Trainer = "Tech Lead (IT Dept)", Duration = "32h", Type = "Internal", IsMandatory = true, Status = "Active", Participants = 20, MaxParticipants = 25 },
            new() { Id = 6, Title = "Digital Marketing Masterclass", Code = "MKT-DIGI", Category = "Marketing", Trainer = "Google Expert", Duration = "20h", Type = "External", IsMandatory = false, Status = "Draft", Participants = 0, MaxParticipants = 30 },
            new() { Id = 7, Title = "An Toàn Lao Động & PCCC", Code = "SAFE-FIRE", Category = "Technical", Trainer = "PCCC Quận 1", Duration = "8h", Type = "External", IsMandatory = true, Status = "Active", Participants = 100, MaxParticipants = 150 },
            new() { Id = 8, Title = "Kỹ Năng Giải Quyết Vấn Đề", Code = "SOFT-PROB", Category = "Soft Skills", Trainer = "Phan Thanh Tùng", Duration = "12h", Type = "Internal", IsMandatory = false, Status = "Active", Participants = 18, MaxParticipants = 25 },
            new() { Id = 9, Title = "Quản Trị Dự Án Agile/Scrum", Code = "MGT-SCRUM", Category = "Management", Trainer = "Scrum Alliance", Duration = "16h", Type = "External", IsMandatory = true, Status = "Active", Participants = 12, MaxParticipants = 12 },
            new() { Id = 10, Title = "Tiếng Anh Giao Tiếp Công Sở", Code = "SOFT-ENG", Category = "Soft Skills", Trainer = "British Council", Duration = "48h", Type = "External", IsMandatory = false, Status = "Active", Participants = 25, MaxParticipants = 30 },
            new() { Id = 11, Title = "Bảo Mật Thông Tin & Dữ Liệu", Code = "TECH-SEC", Category = "Technical", Trainer = "Security Team", Duration = "6h", Type = "Internal", IsMandatory = true, Status = "Active", Participants = 150, MaxParticipants = 200 },
            new() { Id = 12, Title = "Kỹ Năng Thuyết Trình Ấn Tượng", Code = "SOFT-PRES", Category = "Soft Skills", Trainer = "VTV Academy", Duration = "16h", Type = "External", IsMandatory = false, Status = "Completed", Participants = 20, MaxParticipants = 20 }
        };

        private IEnumerable<TrainingItemViewModel> FilteredTrainings => _trainings
            .Where(t => string.IsNullOrWhiteSpace(SearchTerm) || t.Title.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) || t.Code.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            .Where(t => SelectedType == "Tất cả" || t.Category == SelectedType);

        public class TrainingItemViewModel
        {
            public int Id { get; set; }
            public string Title { get; set; } = "";
            public string Code { get; set; } = "";
            public string Category { get; set; } = "";
            public string Trainer { get; set; } = "";
            public string Duration { get; set; } = "";
            public string Type { get; set; } = "";
            public bool IsMandatory { get; set; }
            public string Status { get; set; } = "";
            public int Participants { get; set; }
            public int MaxParticipants { get; set; }

            public string StatusBadge => Status switch
            {
                "Active" => "badge-light-success",
                "Draft" => "badge-light-warning",
                "Completed" => "badge-light-primary",
                "Closed" => "badge-light-danger",
                _ => "badge-light-secondary"
            };
        }
    }
}
