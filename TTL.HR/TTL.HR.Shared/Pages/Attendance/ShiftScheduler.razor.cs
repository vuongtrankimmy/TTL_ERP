using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Attendance.Interfaces;
using TTL.HR.Application.Modules.Attendance.Models;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.Organization.Interfaces;
using TTL.HR.Application.Modules.Organization.Models;
using TTL.HR.Application.Modules.HumanResource.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Models;

namespace TTL.HR.Shared.Pages.Attendance
{
    public partial class ShiftScheduler
    {
        [Inject] private IAttendanceService AttendanceService { get; set; } = default!;
        [Inject] private IEmployeeService EmployeeService { get; set; } = default!;
        [Inject] private IDepartmentService DepartmentService { get; set; } = default!;
        [Inject] private IMasterDataService MasterDataService { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        
        [Parameter]
        [SupplyParameterFromQuery]
        public string? ShiftId { get; set; }

        private bool _isLoading = true;
        
        private DateTime _currentDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        private int _daysInMonth => DateTime.DaysInMonth(_currentDate.Year, _currentDate.Month);
        
        private List<EmployeeScheduleDto> _employees = new();
        private List<DepartmentModel> _departments = new();
        private List<WorkShiftModel> _shifts = new();
        
        // Shift Management States
        private bool _showShiftCatalog = false;
        private bool _showShiftDialog = false;
        private bool _isSavingShift = false;
        
        // Delete Modal State
        private bool _isDeleteModalOpen = false;
        private WorkShiftModel? _shiftToDelete;
        private WorkShiftModel _editingShift = new();
        private string _shiftSearchTerm = "";
        private List<WorkShiftModel> _filteredShiftsList => _shifts
            .Where(s => string.IsNullOrEmpty(_shiftSearchTerm) || 
                        s.Name.Contains(_shiftSearchTerm, StringComparison.OrdinalIgnoreCase) || 
                        s.Code.Contains(_shiftSearchTerm, StringComparison.OrdinalIgnoreCase))
            .ToList();
        
        private string _selectedDepartmentId = "";
        private bool _isShiftView = true; // Default to Shift View as per request
        private int _pageIndex = 1;
        private int _pageSize = 20;
        private long _totalCount = 0;
        private int _totalPages => (int)Math.Ceiling(_totalCount / (double)_pageSize);
        
        private List<string> _predefinedColors = new() { 
            "primary", "success", "info", "warning", "danger", "dark",
            "#F1416C", "#7239EA", "#50CD89", "#FFC700", "#009EF7", "#3F4254",
            "#FF5733", "#C70039", "#900C3F", "#581845", "#FFC300", "#DAF7A6",
            "#1ABC9C", "#2ECC71", "#3498DB", "#9B59B6", "#34495E", "#E67E22"
        };

        private string GetShiftClass(string? color, string prefix = "bg-")
        {
            if (string.IsNullOrEmpty(color)) return $"{prefix}secondary";
            if (color.StartsWith("#")) return "";
            
            // Handle metronic light classes (e.g., bg-light-primary)
            if (prefix == "bg-light-") return $"bg-light-{color}";
            
            return $"{prefix}{color}";
        }

        private string GetShiftStyle(string? color, string variant = "solid")
        {
            if (string.IsNullOrEmpty(color) || !color.StartsWith("#")) return "";
            
            return variant switch
            {
                "solid" => $"background-color: {color} !important; border-color: {color} !important; color: white !important;",
                "light" => $"background-color: {color}20 !important; border-color: {color}40 !important;", // 20 (~12% opacity), 40 (~25%)
                "text" => $"color: {color} !important;",
                _ => ""
            };
        }

        protected override async Task OnInitializedAsync()
        {
            var uri = new Uri(NavigationManager.Uri);
            var query = uri.Query;
            if (!string.IsNullOrEmpty(query))
            {
                var queryParams = query.TrimStart('?').Split('&')
                    .Select(p => p.Split('='))
                    .ToDictionary(p => p[0], p => p.Length > 1 ? Uri.UnescapeDataString(p[1]) : "");

                if (queryParams.TryGetValue("month", out var mStr) && int.TryParse(mStr, out var month) && 
                    queryParams.TryGetValue("year", out var yStr) && int.TryParse(yStr, out var year))
                {
                    try { _currentDate = new DateTime(year, month, 1); } catch { }
                }

                if (queryParams.TryGetValue("shiftId", out var sId))
                {
                    ShiftId = sId;
                }
            }

            await LoadLookups();
            await LoadData();
        }

        private async Task LoadLookups()
        {
            _departments = await DepartmentService.GetDepartmentsAsync();
            _shifts = (await AttendanceService.GetWorkShiftsAsync()).ToList();
            
            // Note: ShiftId from URL is still available in the property and can be used for UI focus/highlighting
        }

        private async Task LoadData()
        {
            _isLoading = true;
            try
            {
                var startDate = new DateTime(_currentDate.Year, _currentDate.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                
                // 1. Fetch all employees (within department if selected)
                var empResult = await EmployeeService.GetEmployeesPaginatedAsync(_pageIndex, _pageSize, null, _selectedDepartmentId);
                if (empResult == null || empResult.Items == null)
                {
                    _employees = new();
                    _totalCount = 0;
                    return;
                }

                var allEmployees = empResult.Items.ToList();
                _totalCount = empResult.TotalCount;

                // 2. Fetch existing schedules
                var schedulesResult = await AttendanceService.GetMonthlyWorkSchedulesAsync(startDate, endDate);
                var allSchedules = (schedulesResult ?? Enumerable.Empty<EmployeeScheduleDto>()).ToList();

                // 3. Merge: Every employee in our list should exist in the view, even with empty schedules
                _employees = allEmployees.Select(e => 
                {
                    var existingSched = allSchedules?.FirstOrDefault(s => s.EmployeeId == e.Id);
                    
                    // Correct mapping based on EmployeeDto properties
                    string name = !string.IsNullOrWhiteSpace(e.FullName) ? e.FullName : "N/A";
                    string code = !string.IsNullOrWhiteSpace(e.Code) ? e.Code : "N/A";
                    string dept = !string.IsNullOrWhiteSpace(e.DepartmentName) ? e.DepartmentName : "N/A";

                    return new EmployeeScheduleDto
                    {
                        EmployeeId = e.Id,
                        EmployeeName = name,
                        EmployeeCode = code,
                        Department = dept,
                        AvatarUrl = e.AvatarUrl ?? "",
                        Schedules = existingSched?.Schedules ?? new List<ScheduleDayDto>()
                    };
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading scheduler data: {ex}");
                try 
                { 
                    await JS.InvokeVoidAsync("toastr.error", $"Lỗi tải dữ liệu: {ex.Message}"); 
                } 
                catch (InvalidOperationException) 
                {
                    // Ignore if JS not available during prerendering
                }
            }
            finally
            {
                _isLoading = false;
            }
        }


        private string GetDayName(DayOfWeek day)
        {
            return day switch
            {
                DayOfWeek.Monday => "Hai",
                DayOfWeek.Tuesday => "Ba",
                DayOfWeek.Wednesday => "Tư",
                DayOfWeek.Thursday => "Năm",
                DayOfWeek.Friday => "Sáu",
                DayOfWeek.Saturday => "Bảy",
                DayOfWeek.Sunday => "CN",
                _ => ""
            };
        }


        private async Task ChangeMonth(int delta)
        {
            _currentDate = _currentDate.AddMonths(delta);
            await LoadData();
        }

        private async Task OnDepartmentChanged(ChangeEventArgs e)
        {
            _selectedDepartmentId = e.Value?.ToString() ?? "";
            _pageIndex = 1; // Reset to page 1 on filter change
            await LoadData();
        }

        private async Task ChangePage(int newPage)
        {
            if (newPage >= 1 && newPage <= _totalPages)
            {
                _pageIndex = newPage;
                await LoadData();
            }
        }

        private void OpenBulkAssign()
        {
            NavigationManager.NavigateTo($"/attendance/allocation?month={_currentDate.Month}&year={_currentDate.Year}");
        }

        // Shift Management Actions
        private void ToggleShiftCatalog() => _showShiftCatalog = !_showShiftCatalog;

        private void OpenShiftDialog(WorkShiftModel? shift = null)
        {
            if (shift != null)
            {
                _editingShift = new WorkShiftModel
                {
                    Id = shift.Id,
                    Name = shift.Name,
                    Code = shift.Code,
                    StartTime = shift.StartTime,
                    EndTime = shift.EndTime,
                    Color = shift.Color
                };
            }
            else
            {
                _editingShift = new WorkShiftModel
                {
                    Color = "primary",
                    StartTime = "08:00",
                    EndTime = "17:00"
                };
            }
            _showShiftDialog = true;
        }

        private async Task SaveShiftAsync()
        {
            if (string.IsNullOrWhiteSpace(_editingShift.Name) || string.IsNullOrWhiteSpace(_editingShift.Code))
            {
                await JS.InvokeVoidAsync("toastr.warning", "Vui lòng nhập đầy đủ Tên và Mã ca");
                return;
            }

            _isSavingShift = true;
            try
            {
                bool success;
                string? message;

                if (string.IsNullOrEmpty(_editingShift.Id))
                {
                    var result = await AttendanceService.CreateWorkShiftAsync(_editingShift);
                    success = result.Success;
                    message = result.Message;
                }
                else
                {
                    var result = await AttendanceService.UpdateWorkShiftAsync(_editingShift.Id, _editingShift);
                    success = result.Success;
                    message = result.Message;
                }

                if (success)
                {
                    await JS.InvokeVoidAsync("toastr.success", "Lưu thông tin ca thành công");
                    _showShiftDialog = false;
                    _shifts = (await AttendanceService.GetWorkShiftsAsync()).ToList();
                    StateHasChanged();
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", message ?? "Lỗi khi lưu thông tin");
                }
            }
            finally
            {
                _isSavingShift = false;
            }
        }

        // --- DELETE MODAL LOGIC ---
        private void PromptDelete(WorkShiftModel shift)
        {
            _shiftToDelete = shift;
            _isDeleteModalOpen = true;
        }

        private void CloseDeleteModal()
        {
            _isDeleteModalOpen = false;
            _shiftToDelete = null;
        }

        private async Task ConfirmDeleteShift()
        {
            if (_shiftToDelete != null)
            {
                var result = await AttendanceService.DeleteWorkShiftAsync(_shiftToDelete.Id);
                if (result.Success)
                {
                    await JS.InvokeVoidAsync("toastr.success", "Xóa ca thành công");
                    _shifts = (await AttendanceService.GetWorkShiftsAsync()).ToList();
                    CloseDeleteModal();
                    StateHasChanged();
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", result.Message ?? "Lỗi khi xóa ca");
                }
            }
        }

        private async Task GoBack()
        {
            await JS.InvokeVoidAsync("history.back");
        }
    }
}
