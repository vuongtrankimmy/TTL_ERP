using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using TTL.HR.Application.Modules.Training.Interfaces;
using TTL.HR.Application.Modules.Training.Models;
using Microsoft.JSInterop;
using System.Linq;

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
        private TimeOnly? _newSyllabusTime;
        private string _newSyllabusItem = "";
        private int? _editingSyllabusIndex = null;

        private void AddSyllabusItem()
        {
            if (!string.IsNullOrWhiteSpace(_newSyllabusItem))
            {
                var content = !_newSyllabusTime.HasValue ? _newSyllabusItem.Trim() : $"{_newSyllabusTime.Value.ToString("HH:mm")} | {_newSyllabusItem.Trim()}";
                
                _course.Syllabus ??= new();
                
                if (_editingSyllabusIndex.HasValue)
                {
                    _course.Syllabus[_editingSyllabusIndex.Value] = content;
                    _editingSyllabusIndex = null;
                }
                else
                {
                    _course.Syllabus.Add(content);
                }
                
                _newSyllabusItem = "";
                _newSyllabusTime = null;
                StateHasChanged();
            }
        }

        private void EditSyllabusItem(int index)
        {
            if (_course.Syllabus != null && index >= 0 && index < _course.Syllabus.Count)
            {
                var item = _course.Syllabus[index];
                if (item.Contains(" | "))
                {
                    var parts = item.Split(" | ", 2);
                    _newSyllabusTime = TimeOnly.TryParse(parts[0], out var time) ? time : null;
                    _newSyllabusItem = parts[1];
                }
                else
                {
                    _newSyllabusTime = null;
                    _newSyllabusItem = item;
                }
                _editingSyllabusIndex = index;
                StateHasChanged();
            }
        }

        private void CancelEditSyllabus()
        {
            _editingSyllabusIndex = null;
            _newSyllabusItem = "";
            _newSyllabusTime = null;
            StateHasChanged();
        }

        private void RemoveSyllabusItem(int index)
        {
            if (_course.Syllabus != null && index >= 0 && index < _course.Syllabus.Count)
            {
                _course.Syllabus.RemoveAt(index);
                if (_editingSyllabusIndex == index) _editingSyllabusIndex = null;
                else if (_editingSyllabusIndex > index) _editingSyllabusIndex--;
                StateHasChanged();
            }
        }

        private void HandleSyllabusKeyUp(Microsoft.AspNetCore.Components.Web.KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                AddSyllabusItem();
            }
        }

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
