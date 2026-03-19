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
#pragma warning disable CS0169, CS0414
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
        public string? Id { get; set; }

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
        private string _selectionMode = "Employee"; // "Employee" or "Department"
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
                        .Select(id => {
                            var eId = id.Trim();
                            return _employeeCache.TryGetValue(eId, out var e) ? e : new EmployeeDto { Id = eId, FullName = "Nhân viên " + eId };
                        })
                        .ToList();
                }
                
                // In department mode, we aggregate all employees currently in cache that belong to the selected departments
                return _employeeCache.Values
                    .Where(e => !string.IsNullOrEmpty(e.DepartmentId) && _selectedDeptIds.Any(dId => dId.Trim().Equals(e.DepartmentId.Trim(), StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }
        }

        private List<EmployeeDto> NewlySelectedEmployees => SelectedEmployees.Where(e => !_initialSelectedIds.Any(id => id.Trim().Equals(e.Id.Trim(), StringComparison.OrdinalIgnoreCase))).ToList();
        private List<EmployeeDto> ExistingSelectedEmployees => SelectedEmployees.Where(e => _initialSelectedIds.Any(id => id.Trim().Equals(e.Id.Trim(), StringComparison.OrdinalIgnoreCase))).ToList();

        private int SelectedEmployeeCount => SelectedEmployees.Count;
        private int NewEmployeeCount => NewlySelectedEmployees.Count;

        private int SelectedObjectCount => SummaryItems.Count;

        private string SelectedObjectLabel => "Đối tượng";
        
        private int NewObjectCount => _selectionMode == "Department" 
            ? _selectedDeptIds.Count(id => !_initialSelectedDeptIds.Any(oid => oid.Trim().Equals(id.Trim(), StringComparison.OrdinalIgnoreCase))) 
            : NewlySelectedEmployees.Count;

        private int ExistingObjectCount => _selectionMode == "Department" 
            ? _selectedDeptIds.Count(id => _initialSelectedDeptIds.Any(oid => oid.Trim().Equals(id.Trim(), StringComparison.OrdinalIgnoreCase))) 
            : ExistingSelectedEmployees.Count;

        private List<SummaryItemModel> _successItems = new();
        private bool _isSuccess = false;
        private bool _showConflictModal = false;
        private string _conflictMessage = "";
        private HashSet<string> _finalIdsToAssign = new();

        private List<string> NewlySelectedNames 
        {
            get
            {
                if (_selectionMode == "Department")
                {
                    return _departments.Where(d => _selectedDeptIds.Any(sid => sid.Trim().Equals(d.Id.Trim(), StringComparison.OrdinalIgnoreCase)) 
                                               && !_initialSelectedDeptIds.Any(oid => oid.Trim().Equals(d.Id.Trim(), StringComparison.OrdinalIgnoreCase)))
                                     .Select(d => d.Name).ToList();
                }
                return NewlySelectedEmployees.Select(e => e.FullName ?? "").ToList();
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
        private bool _overwriteConflicts = true; // Mặc định ghi đè để lưu trạng thái mới nhất

        protected override async Task OnInitializedAsync()
        {
            // Initial load moved to OnParametersSetAsync to handle URL changes
        }

        private string? _lastShiftId;
        private int? _lastMonth;
        private int? _lastYear;
        protected override async Task OnParametersSetAsync()
        {
            if (ShiftId != _lastShiftId || Month != _lastMonth || Year != _lastYear)
            {
                _lastShiftId = ShiftId;
                _lastMonth = Month;
                _lastYear = Year;
                _isInitialized = false; // Force re-init
                await InitializePageAsync();
            }
        }

        private bool _isInitialized = false;
        private async Task InitializePageAsync()
        {
            if (_isInitialized) return;
            _isInitialized = true;
            _isLoading = true;
            try
            {
                // 1. Initial lookup load (Global)
                var deptData = await DepartmentService.GetDepartmentsAsync();
                _departments = deptData?.ToList() ?? new List<DepartmentModel>();
                
                var statusData = await MasterDataService.GetCachedLookupsAsync("EmployeeStatus");
                _statuses = statusData?.ToList() ?? new List<LookupModel>();
                
                var workplaceData = await MasterDataService.GetCachedLookupsAsync("Workplace");
                _workplaces = workplaceData?.ToList() ?? new List<LookupModel>();
                
                var now = DateTime.Today;
                var startDate = now;
                var endDate = new DateTime(now.Year, now.Month, 1).AddMonths(1).AddDays(-1);

                if (Month.HasValue && Year.HasValue)
                {
                    if (Month.Value != now.Month || Year.Value != now.Year)
                    {
                        startDate = new DateTime(Year.Value, Month.Value, 1);
                        endDate = startDate.AddMonths(1).AddDays(-1);
                    }
                }

                _assignModel = new AssignWorkScheduleModel
                {
                    StartDate = DateTime.SpecifyKind(startDate.Date, DateTimeKind.Utc),
                    EndDate = DateTime.SpecifyKind(endDate.Date, DateTimeKind.Utc),
                    // DaysOfWeek defaults to M-S in AssignWorkScheduleModel definition
                    IsFlexible = true,
                    BypassQueue = true
                };

                // 2. Load Shifts based on context - ENSURE ONLY ONE IF SHIFTID PRESENT
                string? targetShiftId = Id ?? ShiftId;
                if (!string.IsNullOrEmpty(targetShiftId))
                {
                    _isFilteredByShift = true;
                    _overwriteConflicts = true;
                    _assignModel.ShiftId = targetShiftId.Trim();

                    // Fetch ONLY the specific shift
                    var found = await AttendanceService.GetWorkShiftByIdAsync(targetShiftId);
                    if (found == null)
                    {
                        var all = await AttendanceService.GetWorkShiftsAsync();
                        found = all.FirstOrDefault(s => NormalizedMatch(s.Id, targetShiftId));
                    }

                    if (found != null)
                    {
                        _shifts = new List<WorkShiftModel> { found };
                        _assignModel.ShiftId = found.Id;
                        _assignModel.IsFlexible = found.IsFlexible;
                        
                        // Auto-fill DaysOfWeek from Shift configuration
                        if (found.WorkingDays != null && found.WorkingDays.Any())
                        {
                            _assignModel.DaysOfWeek = new List<DayOfWeek>(found.WorkingDays);
                        }

                        // Auto-fill Application Period from Shift configuration if available
                        if (found.StartDate.HasValue)
                        {
                            _assignModel.StartDate = DateTime.SpecifyKind(found.StartDate.Value.Date, DateTimeKind.Utc);
                        }
                        if (found.EndDate.HasValue)
                        {
                            _assignModel.EndDate = DateTime.SpecifyKind(found.EndDate.Value.Date, DateTimeKind.Utc);
                        }
                    }
                    else
                    {
                        _shifts = new List<WorkShiftModel>();
                    }
                }
                else
                {
                    _shifts = (await AttendanceService.GetWorkShiftsAsync()).ToList();
                    var firstShift = _shifts.FirstOrDefault();
                    _assignModel.ShiftId = firstShift?.Id ?? "";
                    _assignModel.IsFlexible = firstShift?.IsFlexible ?? false;

                    if (firstShift != null)
                    {
                        if (firstShift.WorkingDays != null && firstShift.WorkingDays.Any())
                            _assignModel.DaysOfWeek = new List<DayOfWeek>(firstShift.WorkingDays);
                        
                        if (firstShift.StartDate.HasValue)
                            _assignModel.StartDate = DateTime.SpecifyKind(firstShift.StartDate.Value.Date, DateTimeKind.Utc);
                        
                        if (firstShift.EndDate.HasValue)
                            _assignModel.EndDate = DateTime.SpecifyKind(firstShift.EndDate.Value.Date, DateTimeKind.Utc);
                    }
                }

                // 3. Sequential Sync
                try 
                {
                    await LoadEmployeesAsync(true);
                    if (!string.IsNullOrEmpty(targetShiftId))
                    {
                        await PreSelectExistingAssignments();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ShiftAllocation] Sync delayed: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ShiftAllocation] Critical Init Failure: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        private void OnSelectionModeChanged(string mode)
        {
            _isSuccess = false;
            _selectionMode = mode;
            // Không xóa danh sách đã chọn để tránh mất trạng thái khi người dùng đổi tab xem
        }

        private void SelectShift(string shiftId)
        {
            _isSuccess = false;
            _assignModel.ShiftId = shiftId;
            var shift = _shifts.FirstOrDefault(s => s.Id == shiftId);
            if (shift != null)
            {
                _assignModel.IsFlexible = shift.IsFlexible;
                
                // Auto-fill DaysOfWeek when switching shifts
                if (shift.WorkingDays != null && shift.WorkingDays.Any())
                {
                    _assignModel.DaysOfWeek = new List<DayOfWeek>(shift.WorkingDays);
                }

                // Auto-fill Application Period when switching shifts
                if (shift.StartDate.HasValue)
                    _assignModel.StartDate = DateTime.SpecifyKind(shift.StartDate.Value.Date, DateTimeKind.Utc);

                if (shift.EndDate.HasValue)
                    _assignModel.EndDate = DateTime.SpecifyKind(shift.EndDate.Value.Date, DateTimeKind.Utc);
            }
            StateHasChanged();
        }

        private bool NormalizedMatch(string? id1, string? id2)
        {
            if (string.IsNullOrEmpty(id1) || string.IsNullOrEmpty(id2)) return false;
            return id1.Trim().Equals(id2.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        private async Task PreSelectExistingAssignments()
        {
            try
            {
                _selectedIds.Clear();
                _initialSelectedIds.Clear();
                _selectedDeptIds.Clear();
                _initialSelectedDeptIds.Clear();

                if (string.IsNullOrEmpty(_assignModel.ShiftId)) return;

                // Sync assignments ONLY for the current selected shift
                // EXPAND range to full month to ensure we find anyone assigned in the current cycle
                var fetchStart = new DateTime(_assignModel.StartDate.Year, _assignModel.StartDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                var fetchEnd = fetchStart.AddMonths(1).AddDays(-1);
                
                var schedules = (await AttendanceService.GetMonthlyWorkSchedulesAsync(fetchStart, fetchEnd, 5000)).ToList();
                
                string matchShiftId = _assignModel.ShiftId;
                var currentShift = _shifts.FirstOrDefault(s => NormalizedMatch(s.Id, matchShiftId));
                if (currentShift != null) _assignModel.IsFlexible = currentShift.IsFlexible;
                
                foreach (var empSch in schedules)
                {
                    // Check if this employee has THIS shift on ANY of the monitored days in the WHOLE MONTH
                    var matchedSchedules = empSch.Schedules.Where(s => !string.IsNullOrEmpty(s.ShiftId) && NormalizedMatch(s.ShiftId, matchShiftId)).ToList();
                    
                    if (matchedSchedules.Any())
                    {
                        var eId = empSch.EmployeeId.Trim();
                        _selectedIds.Add(eId);
                        _initialSelectedIds.Add(eId);
                        
                        // Hydrate cache
                        if (!_employeeCache.ContainsKey(eId))
                        {
                            _employeeCache[eId] = new EmployeeDto { 
                                Id = eId, 
                                FullName = empSch.EmployeeName, 
                                Code = empSch.EmployeeCode,
                                DepartmentId = empSch.DepartmentId,
                                DepartmentName = empSch.Department,
                                AvatarUrl = empSch.AvatarUrl
                            };
                        }
                    }
                }

                // Restore Department selections if ALL employees found for a department have this shift
                if (_departments != null && _departments.Any())
                {
                    foreach (var dept in _departments)
                    {
                        var deptId = dept.Id.Trim();
                        // Find all employees that we know belong to this department (from cache or schedule list)
                        var deptEmployees = _employeeCache.Values
                            .Where(e => !string.IsNullOrEmpty(e.DepartmentId) && NormalizedMatch(e.DepartmentId, deptId))
                            .Select(e => e.Id.Trim())
                            .ToList();
                            
                        // If we have employees for this dept and all of them are assigned this shift, mark the dept as selected
                        if (deptEmployees.Any() && deptEmployees.All(id => _selectedIds.Contains(id)))
                        {
                            _selectedDeptIds.Add(deptId);
                            _initialSelectedDeptIds.Add(deptId);
                        }
                    }
                    
                    // If we have any department fully selected, default to Department mode for better UX
                    if (_selectedDeptIds.Any()) 
                    {
                        _selectionMode = "Department";
                    }
                }
                
                StateHasChanged();
            }
            catch (Exception ex) 
            { 
                Console.WriteLine("[ShiftAllocation] Restoration failed: " + ex.Message); 
            }
        }

        private async Task RemoveFromAllocation(string name, string id)
        {
            var targetId = id.Trim();
            bool isInitial = false;
            
            // Xác định xem đối tượng này đã được gán sẵn trong DB chưa
            if (_selectedDeptIds.Any(did => NormalizedMatch(did, targetId)))
                isInitial = _initialSelectedDeptIds.Any(oid => NormalizedMatch(oid, targetId));
            else
                isInitial = _initialSelectedIds.Any(oid => NormalizedMatch(oid, targetId));

            // Chỉ yêu cầu xác nhận và gọi API xóa nếu đã lưu trong DB
            if (isInitial)
            {
                var confirm = await JS.InvokeAsync<bool>("confirm", $"Bạn có chắc chắn muốn gỡ bỏ {name} khỏi lịch gán này không? Thao tác này sẽ xóa lịch làm việc ĐÃ LƯU của đối tượng.");
                if (!confirm) return;
            }

            try
            {
                var finalEmployeesToRemove = new List<string>();

                if (_selectedDeptIds.Any(did => NormalizedMatch(did, targetId)))
                {
                    // 1. Gỡ Phòng ban
                    _selectedDeptIds.RemoveWhere(did => NormalizedMatch(did, targetId));
                    
                    // 2. Gỡ toàn bộ nhân viên thuộc phòng đó khỏi danh sách chọn
                    var deptEmps = _selectedIds
                        .Where(eid => _employeeCache.TryGetValue(eid, out var e) && !string.IsNullOrEmpty(e.DepartmentId) && NormalizedMatch(e.DepartmentId, targetId))
                        .ToList();
                    
                    foreach(var eid in deptEmps)
                    {
                        _selectedIds.Remove(eid);
                        if (_initialSelectedIds.Any(oid => NormalizedMatch(oid, eid)))
                        {
                            finalEmployeesToRemove.Add(eid);
                        }
                    }
                }
                else
                {
                    // Gỡ Cá nhân
                    _selectedIds.Remove(targetId);
                    if (_initialSelectedIds.Any(oid => NormalizedMatch(oid, targetId)))
                    {
                        finalEmployeesToRemove.Add(targetId);
                    }
                }

                // Chỉ gọi API xóa nếu là dữ liệu đã tồn tại
                if (isInitial && finalEmployeesToRemove.Any())
                {
                    var cmd = new AssignWorkScheduleModel
                    {
                        ShiftId = _assignModel.ShiftId,
                        StartDate = _assignModel.StartDate,
                        EndDate = _assignModel.EndDate,
                        EmployeeIds = finalEmployeesToRemove,
                        IsDelete = true
                    };
                    
                    await AttendanceService.AssignScheduleAsync(cmd);
                    await JS.InvokeVoidAsync("toastr.info", $"Đã gỡ bỏ và cập nhật lịch làm việc của {name}");
                }
                
                // Cập nhật lại danh sách ban đầu sau khi xóa cứng
                if (isInitial)
                {
                    _initialSelectedDeptIds.RemoveWhere(oid => NormalizedMatch(oid, targetId));
                    foreach(var eid in finalEmployeesToRemove) _initialSelectedIds.RemoveWhere(oid => NormalizedMatch(oid, eid));
                }

                StateHasChanged();
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("toastr.error", "Không thể gỡ bỏ: " + ex.Message);
            }
        }

        private async Task RefreshDataAsync() => await LoadEmployeesAsync(false);
        private async Task OnSearchChanged() => await LoadEmployeesAsync(false);

        private async Task LoadEmployeesAsync(bool fetchAll = false)
        {
            try
            {
                // Increase search window for full grouping on init
                int size = fetchAll ? 5000 : 1000;
                var result = await EmployeeService.GetEmployeesPaginatedAsync(1, size, _searchTerm, string.IsNullOrEmpty(_selectedDeptIdInner) ? null : new[] { _selectedDeptIdInner }, _selectedStatusId, _selectedWorkplace);
                _employeeList = result?.Items?.ToList() ?? new List<EmployeeDto>();
                _totalCount = result?.TotalCount ?? 0;
                
                foreach(var emp in _employeeList)
                {
                    if (emp != null && !string.IsNullOrEmpty(emp.Id))
                    {
                        // Update or add to cache to ensure DepartmentId is always present
                        _employeeCache[emp.Id] = emp;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoadEmployees] Failed to fetch employee list: {ex.Message}");
                _employeeList = new List<EmployeeDto>();
                // This will be partially recovered by PreSelectExistingAssignments if data exists in DB
            }
        }

        private void ToggleAllEmployees(bool selected)
        {
            _isSuccess = false;
            _isAllSelected = selected;
            foreach (var emp in _employeeList)
            {
                if (selected) _selectedIds.Add(emp.Id);
                else _selectedIds.Remove(emp.Id);
            }
        }

        private void ToggleEmployeeSelection(string empId, bool selected)
        {
            _isSuccess = false;
            var eId = empId.Trim();
            if (selected) _selectedIds.Add(eId);
            else 
            {
                _selectedIds.Remove(eId);
                // Nếu bỏ chọn cá nhân, thì phòng ban tương ứng cũng không còn là "Chọn tất cả"
                if (_employeeCache.TryGetValue(eId, out var emp) && !string.IsNullOrEmpty(emp.DepartmentId))
                {
                    _selectedDeptIds.Remove(emp.DepartmentId.Trim());
                }
            }
            
            // Tự động kiểm tra xem đã chọn đủ người trong phòng ban chưa để đánh dấu tab phòng ban
            CheckDepartmentCompletion(eId);
            
            StateHasChanged();
        }

        private void CheckDepartmentCompletion(string empId)
        {
            if (_employeeCache.TryGetValue(empId, out var emp) && !string.IsNullOrEmpty(emp.DepartmentId))
            {
                var dId = emp.DepartmentId.Trim();
                var deptEmps = _employeeCache.Values.Where(e => NormalizedMatch(e.DepartmentId, dId)).ToList();
                if (deptEmps.Any() && deptEmps.All(e => _selectedIds.Contains(e.Id.Trim())))
                {
                    _selectedDeptIds.Add(dId);
                }
            }
        }

        private async Task ToggleDepartmentSelection(string deptId, bool selected)
        {
            _isSuccess = false;
            var dId = deptId.Trim();
            if (selected) 
            {
                _selectedDeptIds.Add(dId);
                // Tự động chọn tất cả nhân viên thuộc phòng ban này để đồng bộ tab Nhân viên
                var result = await EmployeeService.GetEmployeesPaginatedAsync(1, 5000, null, string.IsNullOrEmpty(dId) ? null : new[] { dId });
                if (result?.Items != null)
                {
                    foreach(var emp in result.Items) 
                    {
                        var eId = emp.Id.Trim();
                        _selectedIds.Add(eId);
                        _employeeCache[eId] = emp;
                    }
                }
            }
            else 
            {
                _selectedDeptIds.Remove(dId);
                // Bỏ chọn các nhân viên thuộc phòng ban này
                var toRemove = _selectedIds
                    .Where(eid => _employeeCache.TryGetValue(eid, out var e) && !string.IsNullOrEmpty(e.DepartmentId) && NormalizedMatch(e.DepartmentId, dId))
                    .ToList();
                foreach(var id in toRemove) _selectedIds.Remove(id);
            }
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

        private async Task ValidateAndPreviewAssignments()
        {
            if (string.IsNullOrEmpty(_assignModel.ShiftId))
            {
                await JS.InvokeVoidAsync("toastr.warning", "Vui lòng chọn ca làm việc để tiếp tục.");
                return;
            }

            _showConflictModal = false;
            _isSuccess = false;
            _isSaving = true;
            StateHasChanged();

            try
            {
                _finalIdsToAssign.Clear();
                
                // 1. Resolve Selections based on active mode ONLY
                if (_selectionMode == "Department")
                {
                    if (!_selectedDeptIds.Any() && !_selectedIds.Any())
                    {
                        await JS.InvokeVoidAsync("toastr.warning", "Vui lòng chọn ít nhất một phòng ban hoặc nhân viên.");
                        _isSaving = false;
                        return;
                    }
                    
                    foreach (var id in _selectedIds)
                    {
                        if (!string.IsNullOrEmpty(id)) _finalIdsToAssign.Add(id.Trim());
                    }
                }
                else // Employee Mode
                {
                    if (!_selectedIds.Any())
                    {
                        await JS.InvokeVoidAsync("toastr.warning", "Vui lòng chọn ít nhất một nhân viên.");
                        _isSaving = false;
                        return;
                    }

                    foreach (var id in _selectedIds)
                    {
                        if (!string.IsNullOrEmpty(id)) _finalIdsToAssign.Add(id.Trim());
                    }
                }

                if (!_finalIdsToAssign.Any())
                {
                    await JS.InvokeVoidAsync("toastr.warning", "Không tìm thấy danh sách nhân viên để gán. Vui lòng kiểm tra lại lựa chọn.");
                    _isSaving = false;
                    return;
                }

                // 2. Normalize inputs
                string targetShiftId = _assignModel.ShiftId.Trim();
                var startDate = DateTime.SpecifyKind(_assignModel.StartDate.Date, DateTimeKind.Utc);
                var endDate = DateTime.SpecifyKind(_assignModel.EndDate.Date, DateTimeKind.Utc);

                // 3. CHECK FOR CONFLICTS
                _conflictToUpdate.Clear();
                try
                {
                    var currentSchedules = await AttendanceService.GetMonthlyWorkSchedulesAsync(startDate, endDate, 5000);
                    if (currentSchedules != null)
                    {
                        foreach (var empSch in currentSchedules)
                        {
                            if (_finalIdsToAssign.Contains(empSch.EmployeeId.Trim()))
                            {
                                bool hasOtherShift = empSch.Schedules.Any(s => !string.IsNullOrEmpty(s.ShiftId) && !NormalizedMatch(s.ShiftId, targetShiftId));
                                if (hasOtherShift) _conflictToUpdate.Add(empSch);
                            }
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine("Conflict check failed: " + ex.Message); }

                if (_conflictToUpdate.Any())
                {
                    var names = string.Join(", ", _conflictToUpdate.Select(c => c.EmployeeName).Take(5));
                    if (_conflictToUpdate.Count > 5) names += "...";
                    
                    _conflictMessage = $"Phát hiện xung đột: Có {_conflictToUpdate.Count} nhân viên ({names}) đã được gán ca khác trong khoảng thời gian này.";
                    _showConflictModal = true;
                    _isSaving = false;
                    return;
                }

                await SaveAssignmentAsync();
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("toastr.error", "Lỗi kiểm tra gán: " + ex.Message);
                _isSaving = false;
            }
            finally
            {
                StateHasChanged();
            }
        }

        private async Task SaveAssignmentAsync()
        {
            _showConflictModal = false;
            _isSaving = true;
            StateHasChanged();

            try
            {
                // Prepare Command
                _assignModel.EmployeeIds = _finalIdsToAssign.ToList();
                _assignModel.ShiftId = _assignModel.ShiftId.Trim();
                _assignModel.StartDate = DateTime.SpecifyKind(_assignModel.StartDate.Date, DateTimeKind.Utc);
                _assignModel.EndDate = DateTime.SpecifyKind(_assignModel.EndDate.Date, DateTimeKind.Utc);
                _assignModel.BypassQueue = true;

                // Persistence
                var success = await AttendanceService.AssignScheduleAsync(_assignModel);
                if (success)
                {
                    _successItems = SummaryItems;
                    _isSuccess = true;
                    _showPreview = true;
                    await JS.InvokeVoidAsync("toastr.success", "Đã lưu lịch làm việc xuống Database thành công!");
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", "Lỗi server! Không thể ghi dữ liệu xuống database.");
                }
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("toastr.error", "Lỗi lưu gán ca: " + ex.Message);
            }
            finally
            {
                _isSaving = false;
                StateHasChanged();
            }
        }

        // UI Helpers
        
        private void SelectAllDays(bool selected)
        {
            _assignModel.DaysOfWeek.Clear();
            if (selected)
            {
                _assignModel.DaysOfWeek.AddRange(Enum.GetValues<DayOfWeek>());
            }
            StateHasChanged();
        }

        private void ToggleDay(DayOfWeek d) {
            if (_assignModel.DaysOfWeek.Contains(d)) _assignModel.DaysOfWeek.Remove(d);
            else _assignModel.DaysOfWeek.Add(d);
        }

        private async Task GoBack()
        {
            await JS.InvokeVoidAsync("history.back");
        }

        private void FinishAndNavigate()
        {
            NavigationManager.NavigateTo("/attendance/shifts");
        }

        public class SummaryItemModel
        {
            public string Id { get; set; } = "";
            public string Name { get; set; } = "";
            public string Code { get; set; } = "";
            public bool IsNew { get; set; }
            public string GroupName { get; set; } = "";
        }

        private List<SummaryItemModel> SummaryItems
        {
            get
            {
                var list = new List<SummaryItemModel>();
                
                // 1. Nhóm các phòng ban đã chọn "Full"
                foreach (var dId in _selectedDeptIds)
                {
                    var dept = _departments.FirstOrDefault(d => NormalizedMatch(d.Id, dId));
                    list.Add(new SummaryItemModel
                    {
                        Id = dId,
                        Name = dept?.Name ?? ("Phòng " + dId),
                        Code = "Phòng ban",
                        IsNew = !_initialSelectedDeptIds.Any(oid => NormalizedMatch(oid, dId)),
                        GroupName = "DANH SÁCH PHÒNG BAN"
                    });
                }

                // 2. Nhóm các nhân viên lẻ (thuộc các phòng chưa được chọn "Full")
                var individualEmpIds = _selectedIds
                    .Where(id => {
                        if (!_employeeCache.TryGetValue(id, out var e)) return true;
                        return string.IsNullOrEmpty(e.DepartmentId) || !_selectedDeptIds.Any(did => NormalizedMatch(did, e.DepartmentId));
                    })
                    .ToList();

                foreach (var eId in individualEmpIds)
                {
                    if (_employeeCache.TryGetValue(eId, out var emp))
                    {
                        list.Add(new SummaryItemModel
                        {
                            Id = eId,
                            Name = emp.FullName ?? ("NV " + eId),
                            Code = emp.Code,
                            IsNew = !_initialSelectedIds.Any(oid => NormalizedMatch(oid, eId)),
                            GroupName = emp.DepartmentName ?? "CÁ NHÂN TỰ DO"
                        });
                    }
                }

                return list.OrderBy(i => i.GroupName == "DANH SÁCH PHÒNG BAN" ? 0 : 1)
                           .ThenBy(i => i.GroupName)
                           .ThenBy(i => i.Name)
                           .ToList();
            }
        }

        public async Task ResetToStep1()
        {
            _isLoading = true;
            _showPreview = false;
            _isSuccess = false;
            _currentStep = 1;
            
            try
            {
                await PreSelectExistingAssignments();
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }
    }
}
