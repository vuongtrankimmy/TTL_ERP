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
    public partial class CourseList
    {
        [Inject] public ITrainingService TrainingService { get; set; } = default!;
        [Inject] public NavigationManager Navigation { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;

        private List<CourseModel> Courses = new();
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
                var result = await TrainingService.GetCoursesAsync();
                Courses = result.ToList();
            }
            catch (Exception)
            {
                await JS.InvokeVoidAsync("toastr.error", "Có lỗi xảy ra khi tải danh sách khóa học.");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void ViewDetail(string id)
        {
            Navigation.NavigateTo($"/training/course/{id}");
        }
    }
}
