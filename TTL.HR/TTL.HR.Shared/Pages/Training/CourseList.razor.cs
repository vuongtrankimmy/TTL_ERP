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

        private bool _loadFailed = false;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && _loadFailed)
            {
                try {
                    await JS.InvokeVoidAsync("toastr.error", "Có lỗi xảy ra khi tải danh sách khóa học.");
                } catch { }
            }
        }

        private async Task LoadData()
        {
            _isLoading = true;
            _loadFailed = false;
            try
            {
                var result = await TrainingService.GetCoursesAsync();
                Courses = result.ToList();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"CourseList Error: {ex.Message}");
                _loadFailed = true;
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
