using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using TTL.HR.Application.Modules.Training.Interfaces;
using TTL.HR.Application.Modules.Training.Models;
using Microsoft.JSInterop;
using System.Linq;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;
using System.Collections.Generic;
using TTL.HR.Application.Modules.HumanResource.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Models;

namespace TTL.HR.Shared.Pages.Training
{
    public partial class TrainingAdd
    {
        [Inject] private ITrainingService TrainingService { get; set; } = default!;
        [Inject] private IMasterDataService MasterDataService { get; set; } = default!; // Injected IMasterDataService
        [Inject] private IEmployeeService EmployeeService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        [Parameter] [SupplyParameterFromQuery] public string? Id { get; set; }

        private CourseModel _course = new();
        private bool _isEdit = false;
        private bool _isSaving = false;
        private TimeOnly? _newSyllabusTime;
        private string _newSyllabusItem = "";
        private int? _editingSyllabusIndex = null;
        private System.Collections.Generic.Dictionary<string, string> _validationErrors = new();
        
        private List<LookupModel> _categories = new();
        private List<LookupModel> _levels = new();
        private List<LookupModel> _statuses = new();
        private List<EmployeeDto> _trainers = new();
        private bool _isInternalTrainer = true;

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
            try
            {
                // Load lookups
                _categories = await MasterDataService.GetCachedLookupsAsync("TrainingCategory");
                _levels = await MasterDataService.GetCachedLookupsAsync("TrainingLevel");
                _statuses = await MasterDataService.GetCachedLookupsAsync("CourseStatus");

                // Load trainers (Employees with IsTrainer == true)
                var allEmps = await EmployeeService.GetEmployeesAsync();
                _trainers = allEmps.Where(e => e.IsTrainer).ToList();
                
                StateHasChanged();

                if (!string.IsNullOrEmpty(Id))
                {
                    _isEdit = true;
                    var result = await TrainingService.GetCourseAsync(Id);
                    if (result != null)
                    {
                        _course = result;
                        _isInternalTrainer = !string.IsNullOrEmpty(_course.TrainerId);
                    }
                    else
                    {
                        await JS.InvokeVoidAsync("toastr.error", "Không tìm thấy khóa học.");
                        Navigation.NavigateTo("/training");
                    }
                }
                else
                {
                    // Default values for new course
                    _course = new CourseModel
                    {
                        StartDate = DateTime.Now,
                        Status = "Active",
                        Category = "General",
                        Level = "Beginner"
                    };
                    StateHasChanged();
                }
            }
            catch (Exception)
            {
                await JS.InvokeVoidAsync("toastr.error", "Không thể tải dữ liệu.");
            }
        }

        private void ToggleTrainerMode(bool isInternal)
        {
            _isInternalTrainer = isInternal;
            if (!_isInternalTrainer)
            {
                _course.TrainerId = null;
            }
            StateHasChanged();
        }

        private async Task SaveCourse()
        {
            _isSaving = true;
            _validationErrors.Clear();
            StateHasChanged();

            try
            {
                // Map TrainerName to TrainerId based on selected mode
                if (_isInternalTrainer)
                {
                    if (!string.IsNullOrWhiteSpace(_course.TrainerName))
                    {
                        var internalTrainer = _trainers.FirstOrDefault(t => 
                            t.FullName.Equals(_course.TrainerName, StringComparison.OrdinalIgnoreCase) || 
                            $"{t.Code} - {t.FullName}".Equals(_course.TrainerName, StringComparison.OrdinalIgnoreCase));
                            
                        _course.TrainerId = internalTrainer?.Id;
                    }
                    else
                    {
                        _course.TrainerId = null;
                    }
                }
                else
                {
                    _course.TrainerId = null; // Always null for external trainers
                }

                ApiResponse<string> createResult = null!;
                ApiResponse<bool> updateResult = null!;
                bool isSuccess = false;
                string message = "";
                System.Collections.Generic.List<string>? errors = null;
                string? stackTrace = null;

                if (_isEdit)
                {
                    updateResult = await TrainingService.UpdateCourseAsync(Id!, _course);
                    isSuccess = updateResult.Success;
                    message = updateResult.Message;
                    errors = updateResult.Errors;
                    stackTrace = updateResult.StackTrace;
                }
                else
                {
                    createResult = await TrainingService.CreateCourseAsync(_course);
                    isSuccess = createResult.Success;
                    message = createResult.Message;
                    errors = createResult.Errors;
                    stackTrace = createResult.StackTrace;
                }

                if (isSuccess)
                {
                    await JS.InvokeVoidAsync("toastr.success", "Đã lưu khóa học thành công!");
                    Navigation.NavigateTo("/training");
                }
                else
                {
                    if (!string.IsNullOrEmpty(stackTrace)) 
                    {
                        _validationErrors["DebugInfo"] = stackTrace;
                    }

                    // Map errors to fields if possible
                    if (errors != null && errors.Any())
                    {
                        foreach (var error in errors)
                        {
                            if (error.Contains(":"))
                            {
                                var parts = error.Split(":", 2);
                                var field = parts[0].Trim();
                                var msg = parts[1].Trim();
                                _validationErrors[field] = msg;
                            }
                            else
                            {
                                _validationErrors["General"] = error;
                            }
                        }
                        
                        await JS.InvokeVoidAsync("toastr.error", "Vui lòng kiểm tra lại các trường thông tin bị lỗi.");
                        await FocusFirstError();
                    }
                    else
                    {
                        await JS.InvokeVoidAsync("toastr.error", string.IsNullOrEmpty(message) ? "Có lỗi xảy ra khi lưu khóa học." : message);
                    }
                }
            }
            catch (Exception ex)
            {
                _validationErrors["General"] = $"Có lỗi hệ thống: {ex.Message}";
                _validationErrors["DebugInfo"] = ex.StackTrace ?? "";
                await JS.InvokeVoidAsync("toastr.error", $"Có lỗi hệ thống: {ex.Message}");
            }
            finally
            {
                _isSaving = false;
                StateHasChanged();
            }
        }

        private async Task FocusFirstError()
        {
            try
            {
                if (_validationErrors.Any())
                {
                    var firstField = _validationErrors.Keys.FirstOrDefault(k => k != "General");
                    if (!string.IsNullOrEmpty(firstField))
                    {
                        // Match field name to ID
                        var elementId = firstField.ToLower() switch
                        {
                            "title" => "course_title",
                            "code" => "course_code",
                            "trainername" => "course_trainer",
                            "category" => "course_category",
                            "status" => "course_status",
                            "level" => "course_level",
                            "durationhours" => "course_duration",
                            "maxparticipants" => "course_max",
                            "startdate" => "course_start",
                            "description" => "course_description",
                            _ => firstField
                        };
                        
                        await JS.InvokeVoidAsync("eval", $"document.getElementById('{elementId}')?.focus()");
                    }
                }
            }
            catch { }
        }
    }
}
