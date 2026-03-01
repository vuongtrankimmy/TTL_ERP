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
using TTL.HR.Application.Modules.HumanResource.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Models;
using TTL.HR.Application.Modules.Organization.Interfaces;
using TTL.HR.Application.Modules.Organization.Models;

namespace TTL.HR.Shared.Pages.Attendance
{
    public partial class ShiftAllocation
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

        [Parameter]
        [SupplyParameterFromQuery]
        public string? Mode { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public int? Month { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public int? Year { get; set; }

        private bool _isLoading = true;
        private bool _isSaving = false;
        private bool _showPreview = false;
        
        // Data sources
        private List<DepartmentModel> _departments = new();
        private List<WorkShiftModel> _shifts = new();
        
        // State
        private int _currentStep = 1; // 1: Setup, 2: Selection, 3: Review
        private AssignWorkScheduleModel _assignModel = new();
        
        // Step 2: Selection State
        private string _selectionMode = "Department"; // "Employee" or "Department"
        private List<EmployeeDto> _employeeList = new();
        private int _pageIndex = 1;
        private int _pageSize = 10;
        private long _totalCount = 0;
        private int _totalPages => (int)Math.Ceiling(_totalCount / (double)_pageSize);
        private HashSet<string> _selectedIds = new();
        private HashSet<string> _initialSelectedIds = new();
        private HashSet<string> _selectedDeptIds = new();
        private HashSet<string> _initialSelectedDeptIds = new();
        private bool _isAllSelected = false;
        private string _searchTerm = "";
        private string _selectedDeptIdInner = "";
        private string _selectedStatusId = "";
        private Dictionary<string, EmployeeDto> _employeeCache = new();
        private bool _isFilteredByShift = false;

        private List<string> _predefinedColors = new() { 
            "primary", "success", "info", "warning", "danger", "dark",
            "#F1416C", "#7239EA", "#50CD89", "#FFC700", "#009EF7", "#3F4254",
            "#FF5733", "#C70039", "#900C3F", "#581845", "#FFC300", "#DAF7A6",
            "#1ABC9C", "#2ECC71", "#3498DB", "#9B59B6", "#34495E", "#E67E22"
        };

        private string GetShiftStyle(string? color, bool isBackground = true)
        {
            if (string.IsNullOrEmpty(color)) return "";
            if (color.StartsWith("#"))
            {
                return isBackground ? $"background-color: {color} !important;" : $"color: {color} !important;";
            }
            return "";
        }

        private string GetShiftClass(string? color, string prefix = "badge-")
        {
            if (string.IsNullOrEmpty(color)) return $"{prefix}secondary";
            if (color.StartsWith("#")) return "";
            return $"{prefix}{color}";
        }

        private List<EmployeeDto> SelectedEmployees 
        {
            get 
            {
                if (_selectionMode == "Employee")
                {
                    return _selectedIds
                        .Select(id => _employeeCache.TryGetValue(id, out var e) ? e : new EmployeeDto { Id = id, FullName = "Nhân viên " + id })
                        .ToList();
                }
                
                return _employeeList.Where(e => !string.IsNullOrEmpty(e.DepartmentId) && _selectedDeptIds.Contains(e.DepartmentId)).ToList();
            }
        }

        private List<EmployeeDto> NewlySelectedEmployees => SelectedEmployees.Where(e => !_initialSelectedIds.Contains(e.Id)).ToList();
        private List<EmployeeDto> ExistingSelectedEmployees => SelectedEmployees.Where(e => _initialSelectedIds.Contains(e.Id)).ToList();

        private int SelectedEmployeeCount => SelectedEmployees.Count;
        private int NewEmployeeCount => NewlySelectedEmployees.Count;

        private int SelectedObjectCount => _selectionMode == "Department" ? _selectedDeptIds.Count : _selectedIds.Count;

        private string SelectedObjectLabel => _selectionMode == "Department" ? "Phòng ban" : "Nhân viên";
        
        private int NewObjectCount => _selectionMode == "Department" 
            ? _selectedDeptIds.Count(id => !_initialSelectedDeptIds.Contains(id)) 
            : NewlySelectedEmployees.Count;

        private int ExistingObjectCount => _selectionMode == "Department" 
            ? _selectedDeptIds.Count(id => _initialSelectedDeptIds.Contains(id)) 
            : ExistingSelectedEmployees.Count;

        private List<string> NewlySelectedNames 
        {
            get
            {
                if (_selectionMode == "Department")
                {
                    return _departments.Where(d => _selectedDeptIds.Contains(d.Id) && !_initialSelectedDeptIds.Contains(d.Id))
                                     .Select(d => d.Name).ToList();
                }
                return NewlySelectedEmployees.Select(e => e.FullName ?? "").ToList();
            }
        }

        private List<string> ExistingSelectedNames
        {
            get
            {
                if (_selectionMode == "Department")
                {
                    return _departments.Where(d => _selectedDeptIds.Contains(d.Id) && _initialSelectedDeptIds.Contains(d.Id))
                                     .Select(d => d.Name).ToList();
                }
                return ExistingSelectedEmployees.Select(e => e.FullName ?? "").ToList();
            }
        }

        private List<DateTime> GetPreviewDates()
        {
            var dates = new List<DateTime>();
            if (_assignModel == null) return dates;
            
            for (var d = _assignModel.StartDate.Date; d <= _assignModel.EndDate.Date && dates.Count < 31; d = d.AddDays(1))
            {
                if (_assignModel.DaysOfWeek.Contains(d.DayOfWeek))
                    dates.Add(d);
            }
            return dates;
        }

        private string GetVietnameseDayName(DayOfWeek day)
        {
            return day switch
            {
                DayOfWeek.Monday => "Thứ 2",
                DayOfWeek.Tuesday => "Thứ 3",
                DayOfWeek.Wednesday => "Thứ 4",
                DayOfWeek.Thursday => "Thứ 5",
                DayOfWeek.Friday => "Thứ 6",
                DayOfWeek.Saturday => "Thứ 7",
                DayOfWeek.Sunday => "Chủ nhật",
                _ => ""
            };
        }
        private string _selectedWorkplace = "";
        private List<LookupModel> _statuses = new();
        private List<LookupModel> _workplaces = new();

        // Step 3: Review State
        private List<EmployeeScheduleDto> _readyToAssign = new();
        private List<EmployeeScheduleDto> _conflictToUpdate = new();
        private bool _overwriteConflicts = true;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;
            try
            {
                _departments = await DepartmentService.GetDepartmentsAsync();
                _statuses = await MasterDataService.GetCachedLookupsAsync("EmployeeStatus");
                _workplaces = await MasterDataService.GetCachedLookupsAsync("Workplace");
                _shifts = (await AttendanceService.GetWorkShiftsAsync()).ToList();
                
                var now = DateTime.Today;
                var targetMonth = Month ?? now.Month;
                var targetYear = Year ?? now.Year;

                // Initialize Model
                _assignModel = new AssignWorkScheduleModel
                {
                    StartDate = new DateTime(targetYear, targetMonth, 1),
                    EndDate = new DateTime(targetYear, targetMonth, 1).AddMonths(1).AddDays(-1),
                    ShiftId = !string.IsNullOrEmpty(ShiftId) ? ShiftId : (_shifts.FirstOrDefault()?.Id ?? ""),
                    DaysOfWeek = new List<DayOfWeek> { 
                        DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, 
                        DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday 
                    }
                };

                if (Mode == "clone") _overwriteConflicts = true;

                _isFilteredByShift = !string.IsNullOrEmpty(ShiftId);
                if (_isFilteredByShift)
                {
                    _assignModel.ShiftId = ShiftId;
                    var selectedShift = _shifts.FirstOrDefault(s => s.Id == ShiftId);
                    if (selectedShift != null)
                    {
                        _shifts = new List<WorkShiftModel> { selectedShift };
                    }
                }

                await LoadEmployeesAsync();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initialized: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void SelectShift(string shiftId)
        {
            _assignModel.ShiftId = shiftId;
        }


        private async Task LoadEmployeesAsync()
        {
            // We set a large page size for allocation to make it easier to select many
            var result = await EmployeeService.GetEmployeesPaginatedAsync(1, 1000, _searchTerm, _selectedDeptIdInner, _selectedStatusId, _selectedWorkplace);
            _employeeList = result.Items.ToList();


            _totalCount = _employeeList.Count;
            
            // Populating cache
            foreach (var emp in _employeeList)
            {
                if (!_employeeCache.ContainsKey(emp.Id))
                    _employeeCache[emp.Id] = emp;
            }

            // Re-check all selected state
            _isAllSelected = _employeeList.Any() && _employeeList.All(e => _selectedIds.Contains(e.Id));
            
            StateHasChanged();
        }

        private void ToggleAllEmployees(bool selected)
        {
            _isAllSelected = selected;
            foreach (var emp in _employeeList)
            {
                if (selected) _selectedIds.Add(emp.Id);
                else _selectedIds.Remove(emp.Id);
            }
        }

        private void ToggleEmployeeSelection(string empId)
        {
            if (_selectedIds.Contains(empId)) _selectedIds.Remove(empId);
            else _selectedIds.Add(empId);
            StateHasChanged();
        }

        private void ToggleDepartmentSelection(string deptId)
        {
            if (_selectedDeptIds.Contains(deptId)) _selectedDeptIds.Remove(deptId);
            else _selectedDeptIds.Add(deptId);
            StateHasChanged();
        }

        private void ToggleAllDepartments(bool selected)
        {
            // No longer used in simplified UI as we select via table
        }

        // Navigation
        private async Task GoToStep2()
        {
            // No longer used, we are in single view
        }

        private async Task GoToStep3()
        {
            if (string.IsNullOrEmpty(_assignModel.ShiftId))
            {
                await JS.InvokeVoidAsync("toastr.warning", "Vui lòng chọn ca làm việc");
                return;
            }

            var finalIds = new HashSet<string>();
            if (_selectionMode == "Employee")
            {
                foreach (var id in _selectedIds) finalIds.Add(id);
            }
            else
            {
                if (!_selectedDeptIds.Any())
                {
                    await JS.InvokeVoidAsync("toastr.warning", "Vui lòng chọn ít nhất một phòng ban");
                    return;
                }
                foreach (var deptId in _selectedDeptIds)
                {
                    // Fetch all employees in this department for assignment
                    var result = await EmployeeService.GetEmployeesPaginatedAsync(1, 2000, null, deptId);
                    foreach (var emp in result.Items) finalIds.Add(emp.Id);
                }
            }

            if (!finalIds.Any())
            {
                await JS.InvokeVoidAsync("toastr.warning", "Vui lòng chọn ít nhất một đối tượng áp dụng");
                return;
            }

            _isSaving = true;
            StateHasChanged();

            try
            {
                // Categorize
                var existing = (await AttendanceService.GetMonthlyWorkSchedulesAsync(_assignModel.StartDate, _assignModel.EndDate)).ToList();
                
                _readyToAssign.Clear();
                _conflictToUpdate.Clear();

                foreach (var id in finalIds)
                {
                    var sched = existing.FirstOrDefault(s => s.EmployeeId == id);
                    bool conflict = false;
                    if (sched != null)
                    {
                        for (var d = _assignModel.StartDate.Date; d <= _assignModel.EndDate.Date; d = d.AddDays(1))
                        {
                            if (_assignModel.DaysOfWeek.Contains(d.DayOfWeek))
                            {
                                // A conflict only occurs if the employee is already assigned to a DIFFERENT shift on this day.
                                // Re-assigning the same shift is not a conflict.
                                if (sched.Schedules.Any(s => s.Date.Date == d && !string.IsNullOrEmpty(s.ShiftId) && s.ShiftId != _assignModel.ShiftId))
                                {
                                    conflict = true;
                                    break;
                                }
                            }
                        }
                    }

                    var emp = _employeeList.FirstOrDefault(e => e.Id == id);
                    var dto = sched ?? new EmployeeScheduleDto 
                    { 
                        EmployeeId = id, 
                        EmployeeName = emp?.FullName ?? ("Nhân viên " + id) 
                    };

                    if (conflict) _conflictToUpdate.Add(dto);
                    else _readyToAssign.Add(dto);
                }

                _currentStep = 3; // Show confirmation modal
            }
            finally
            {
                _isSaving = false;
            }
        }

        private async Task SaveAsync()
        {
            _isSaving = true;
            try
            {
                var ids = _readyToAssign.Select(e => e.EmployeeId).ToList();
                if (_overwriteConflicts) ids.AddRange(_conflictToUpdate.Select(e => e.EmployeeId));

                if (string.IsNullOrEmpty(_assignModel.ShiftId))
                {
                    await JS.InvokeVoidAsync("toastr.error", "Mã ca làm việc không hợp lệ. Vui lòng chọn lại ca.");
                    return;
                }

                _assignModel.EmployeeIds = ids ?? new List<string>();
                
                if (!_assignModel.EmployeeIds.Any())
                {
                    await JS.InvokeVoidAsync("toastr.warning", "Vui lòng chọn ít nhất một nhân viên để gán ca.");
                    return;
                }
                var success = await AttendanceService.AssignScheduleAsync(_assignModel);
                if (success)
                {
                    await JS.InvokeVoidAsync("toastr.success", "Phân bổ ca thành công!");
                    _showPreview = true;
                    _currentStep = 1;
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("toastr.error", $"Lỗi: {ex.Message}");
            }
            finally
            {
                _isSaving = false;
            }
        }

        // UI Helpers
        
        private void ToggleDay(DayOfWeek d) {
            if (_assignModel.DaysOfWeek.Contains(d)) _assignModel.DaysOfWeek.Remove(d);
            else _assignModel.DaysOfWeek.Add(d);
        }

        private async Task GoBack()
        {
            await JS.InvokeVoidAsync("history.back");
        }
    }
}
