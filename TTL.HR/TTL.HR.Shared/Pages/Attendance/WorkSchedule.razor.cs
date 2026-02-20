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

namespace TTL.HR.Shared.Pages.Attendance
{
    public partial class WorkSchedule
    {
        [Inject] private IAttendanceService AttendanceService { get; set; } = default!;
        [Inject] private IMasterDataService MasterDataService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        private bool _showDetail = false;
        private bool _isLoading = true;
        private bool _isProcessing = false;
        private WorkScheduleModel? _selectedSchedule;
        private List<WorkScheduleModel> _schedules = new();
        private List<WorkScheduleModel> _filteredSchedules = new();
        
        private List<LookupModel> _departments = new();
        private List<WorkShiftModel> _shifts = new();
        
        // Filters
        private string _searchTerm = "";
        private string _selectedDepartmentId = "";
        private string _selectedShiftType = ""; // Filter by shift name/type

        // Assignment Model
        private AssignWorkScheduleModel _assignModel = new();

        // Pagination
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalPages = 1;
        private List<WorkScheduleModel> _pagedSchedules = new();

        private DateTime _startDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
        private DateTime _endDate => _startDate.AddDays(6);

        // Multi-selection
        private HashSet<string> _selectedEmpIds = new();
        private bool _isAllSelected = false;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;
            try
            {
                await LoadLookups();
                await LoadData();
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task LoadLookups()
        {
            _departments = await MasterDataService.GetCachedLookupsAsync("Department");
            _shifts = (await AttendanceService.GetWorkShiftsAsync()).ToList();
        }

        private async Task LoadData()
        {
            try
            {
                var result = await AttendanceService.GetWorkSchedulesAsync(_startDate, _endDate);
                if (result != null)
                {
                    _schedules = result.ToList();
                    ApplyFilters(resetPage: false);
                }
            }
            catch (Exception ex)
            {
               await JS.InvokeVoidAsync("toastr.error", $"Lỗi tải dữ liệu: {ex.Message}");
            }
        }

        private void ApplyFilters(bool resetPage = true)
        {
            _filteredSchedules = _schedules;

            if (!string.IsNullOrEmpty(_searchTerm))
            {
                var lowerTerm = _searchTerm.ToLower();
                _filteredSchedules = _filteredSchedules.Where(s => 
                    s.EmployeeName.ToLower().Contains(lowerTerm) || 
                    s.EmployeeId.ToLower().Contains(lowerTerm) ||
                    s.Department.ToLower().Contains(lowerTerm)
                ).ToList();
            }

            if (!string.IsNullOrEmpty(_selectedDepartmentId))
            {
                 var dept = _departments.FirstOrDefault(d => d.Id == _selectedDepartmentId);
                 if (dept != null)
                 {
                     _filteredSchedules = _filteredSchedules.Where(s => s.Department == dept.Name).ToList();
                 }
            }

            if (!string.IsNullOrEmpty(_selectedShiftType))
            {
                _filteredSchedules = _filteredSchedules.Where(s => s.CurrentShift.Contains(_selectedShiftType)).ToList();
            }

            // Calculate Pagination
            _totalPages = (int)Math.Ceiling(_filteredSchedules.Count / (double)_pageSize);
            if (_totalPages < 1) _totalPages = 1;
            
            if (resetPage) _currentPage = 1;
            else if (_currentPage > _totalPages) _currentPage = _totalPages;

            UpdatePagedSchedules();
            
            // Reset selection on filter change? Optional. Let's keep it but re-evaluate "Select All" state
            _isAllSelected = _filteredSchedules.Any() && _selectedEmpIds.IsSupersetOf(_filteredSchedules.Select(s => s.Id));
        }

        private void UpdatePagedSchedules()
        {
            _pagedSchedules = _filteredSchedules
                .Skip((_currentPage - 1) * _pageSize)
                .Take(_pageSize)
                .ToList();
        }

        private void ChangePage(int page)
        {
            if (page < 1 || page > _totalPages) return;
            _currentPage = page;
            UpdatePagedSchedules();
        }

        private void OnSearchChange(ChangeEventArgs e)
        {
            _searchTerm = e.Value?.ToString() ?? "";
            ApplyFilters();
        }

        private void OnDepartmentChange(ChangeEventArgs e)
        {
            _selectedDepartmentId = e.Value?.ToString() ?? "";
            ApplyFilters();
        }

        private void OnShiftTypeChange(ChangeEventArgs e)
        {
            _selectedShiftType = e.Value?.ToString() ?? "";
            ApplyFilters();
        }

        // Selection Logic
        private void ToggleSelectAll(ChangeEventArgs e)
        {
            _isAllSelected = (bool)(e.Value ?? false);
            if (_isAllSelected)
            {
                foreach (var s in _filteredSchedules) _selectedEmpIds.Add(s.Id);
            }
            else
            {
                _selectedEmpIds.Clear();
            }
        }

        private void ToggleSelectEmp(string empId, ChangeEventArgs e)
        {
            var isChecked = (bool)(e.Value ?? false);
            if (isChecked) _selectedEmpIds.Add(empId);
            else _selectedEmpIds.Remove(empId);
            
            _isAllSelected = _filteredSchedules.Any() && _selectedEmpIds.IsSupersetOf(_filteredSchedules.Select(s => s.Id));
        }

        private void openDetail(WorkScheduleModel item)
        {
            _selectedSchedule = item;
            
            // Init assign model for single employee
            _assignModel = new AssignWorkScheduleModel
            {
                EmployeeIds = new List<string> { item.Id },
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddMonths(1),
                ShiftId = item.CurrentShiftId ?? "" 
            };
            
            _showDetail = true;
        }

        private void OpenBulkAssign()
        {
            if (_selectedEmpIds.Count == 0)
            {
               JS.InvokeVoidAsync("toastr.warning", "Vui lòng chọn ít nhất một nhân viên");
               return;
            }

            _selectedSchedule = null; // Indicates Bulk Mode
            
            _assignModel = new AssignWorkScheduleModel
            {
                EmployeeIds = _selectedEmpIds.ToList(),
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddMonths(1),
                ShiftId = "" 
            };

            _showDetail = true;
        }

        private void closeDetail()
        {
            _showDetail = false;
        }

        private async Task SaveSchedule()
        {
            if (string.IsNullOrEmpty(_assignModel.ShiftId))
            {
                await JS.InvokeVoidAsync("toastr.warning", "Vui lòng chọn ca làm việc mới");
                return;
            }

            _isProcessing = true;
            try
            {
                var success = await AttendanceService.AssignScheduleAsync(_assignModel);
                if (success)
                {
                    await JS.InvokeVoidAsync("toastr.success", "Đã cập nhật lịch làm việc thành công!");
                    closeDetail();
                    _selectedEmpIds.Clear(); 
                    _isAllSelected = false;
                    await RefreshData(); 
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", "Cập nhật thất bại. Vui lòng thử lại.");
                }
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("toastr.error", $"Lỗi: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
            }
        }
        
        private async Task RefreshData()
        {
            _isLoading = true;
            await LoadData();
            _isLoading = false;
        }

        private async Task ChangeWeek(int delta)
        {
            _startDate = _startDate.AddDays(delta * 7);
            await RefreshData();
        }

        private async Task ExportExcel()
        {
             await JS.InvokeVoidAsync("toastr.info", "Tính năng đang được phát triển");
        }
    }
}
