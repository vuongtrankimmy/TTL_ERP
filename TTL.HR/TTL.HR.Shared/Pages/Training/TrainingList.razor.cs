using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using TTL.HR.Shared.Components.Training;
using TTL.HR.Shared.Interfaces;
using TTL.HR.Shared.Models;

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

        [Microsoft.AspNetCore.Components.Inject]
        public ITrainingService TrainingService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            _isLoading = true;
            try
            {
                var courses = await TrainingService.GetCoursesAsync();
                if (courses != null)
                {
                    _trainings = courses.Select(c => new TrainingItemViewModel
                    {
                        Id = int.TryParse(c.Id, out var id) ? id : 0,
                        Title = c.Title,
                        Code = $"TRN-{c.Id}",
                        Category = "Technical", // Default
                        Trainer = c.TrainerName,
                        Duration = $"{c.DurationHours}h",
                        Type = "Internal",
                        IsMandatory = true,
                        Status = c.Status,
                        Participants = 0,
                        MaxParticipants = 20
                    }).ToList();
                }
            }
            catch (Exception)
            {
                // Error handling
            }
            finally
            {
                _isLoading = false;
            }
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

        private List<TrainingItemViewModel> _trainings = new();

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
