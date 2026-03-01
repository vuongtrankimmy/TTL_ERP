using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Attendance.Interfaces;
using TTL.HR.Application.Modules.Attendance.Models;

namespace TTL.HR.Shared.Pages.Attendance
{
    public partial class ShiftList
    {
        [Inject] private IAttendanceService AttendanceService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        [Parameter]
        [SupplyParameterFromQuery]
        public int? Month { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public int? Year { get; set; }

        private List<WorkShiftModel> _shifts = new();
        private List<WorkShiftModel> _filteredShifts => _shifts
            .Where(s => string.IsNullOrEmpty(_searchTerm) || 
                        s.Name.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) || 
                        s.Code.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase))
            .ToList();

        private string _searchTerm = "";
        private bool _isLoading = true;
        private bool _showDialog = false;
        private bool _isSaving = false;

        // Delete Modal State
        private bool _isDeleteModalOpen = false;
        private WorkShiftModel? _shiftToDelete;
        private WorkShiftModel _editingModel = new();

        // Assigned Objects Drawer
        private bool _showAssignedObjects = false;
        private WorkShiftModel? _selectedShiftForDetail;
        private List<EmployeeScheduleDto> _assignedEmployees = new();
        private List<EmployeeScheduleDto> _allAssignedEmployees = new();
        private bool _isLoadingDetail = false;
        private int _detailPageIndex = 1;
        private int _detailPageSize = 5;
        private int _totalDetailCount = 0;
        private int _totalDetailPages => (int)Math.Ceiling(_totalDetailCount / (double)_detailPageSize);

        private List<EmployeeScheduleDto> _pagedAssignedEmployees => _allAssignedEmployees
            .Skip((_detailPageIndex - 1) * _detailPageSize)
            .Take(_detailPageSize)
            .ToList();

        private List<string> _predefinedColors = new() { 
            "primary", "success", "info", "warning", "danger", "dark",
            "#F1416C", "#7239EA", "#50CD89", "#FFC700", "#009EF7", "#3F4254", // Metronic Hex equivalents
            "#FF5733", "#C70039", "#900C3F", "#581845", "#FFC300", "#DAF7A6", // Vibrant custom
            "#1ABC9C", "#2ECC71", "#3498DB", "#9B59B6", "#34495E", "#E67E22"  // Flat UI
        };

        private string GetShiftStyle(string color, bool isBackground = true)
        {
            if (string.IsNullOrEmpty(color)) return "";
            if (color.StartsWith("#"))
            {
                return isBackground ? $"background-color: {color} !important;" : $"color: {color} !important;";
            }
            return "";
        }

        private string GetShiftClass(string color, string prefix = "badge-")
        {
            if (string.IsNullOrEmpty(color)) return $"{prefix}secondary";
            if (color.StartsWith("#")) return "";
            return $"{prefix}{color}";
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadShiftsAsync();
        }

        private async Task LoadShiftsAsync()
        {
            _isLoading = true;
            try
            {
                var shifts = await AttendanceService.GetWorkShiftsAsync();
                _shifts = shifts.ToList();
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void OpenShiftDialog(WorkShiftModel? shift = null)
        {
            if (shift != null)
            {
                _editingModel = new WorkShiftModel
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
                _editingModel = new WorkShiftModel
                {
                    Color = "primary",
                    StartTime = "08:00",
                    EndTime = "17:00"
                };
            }
            _showDialog = true;
        }

        private async Task SaveShiftAsync()
        {
            if (string.IsNullOrWhiteSpace(_editingModel.Name) || string.IsNullOrWhiteSpace(_editingModel.Code))
            {
                await JS.InvokeVoidAsync("toastr.warning", "Vui lòng nhập đầy đủ Tên và Mã ca");
                return;
            }

            _isSaving = true;
            try
            {
                if (string.IsNullOrEmpty(_editingModel.Id))
                {
                    var result = await AttendanceService.CreateWorkShiftAsync(_editingModel);
                    if (result.Success)
                    {
                        await JS.InvokeVoidAsync("toastr.success", "Thêm ca mới thành công");
                        _showDialog = false;
                        await LoadShiftsAsync();
                    }
                    else
                    {
                        await JS.InvokeVoidAsync("toastr.error", result.Message ?? "Lỗi khi thêm ca mới");
                    }
                }
                else
                {
                    var result = await AttendanceService.UpdateWorkShiftAsync(_editingModel.Id, _editingModel);
                    if (result.Success)
                    {
                        await JS.InvokeVoidAsync("toastr.success", "Cập nhật thông tin thành công");
                        _showDialog = false;
                        await LoadShiftsAsync();
                    }
                    else
                    {
                        await JS.InvokeVoidAsync("toastr.error", result.Message ?? "Lỗi khi cập nhật");
                    }
                }
            }
            finally
            {
                _isSaving = false;
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
                    CloseDeleteModal();
                    await LoadShiftsAsync();
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", result.Message ?? "Lỗi khi xóa ca");
                }
            }
        }

        private async Task OpenAssignedObjects(WorkShiftModel shift)
        {
            _selectedShiftForDetail = shift;
            _showAssignedObjects = true;
            _isLoadingDetail = true;
            _allAssignedEmployees = new();
            _detailPageIndex = 1;

            try
            {
                // Fetch schedules for specified month/year to see assignments
                var now = DateTime.Today;
                var m = Month ?? now.Month;
                var y = Year ?? now.Year;
                var startDate = new DateTime(y, m, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                
                var allSchedules = await AttendanceService.GetMonthlyWorkSchedulesAsync(startDate, endDate);
                
                // Filter employees who have this shift assigned at least once in the period
                _allAssignedEmployees = allSchedules
                    .Where(e => e.Schedules.Any(s => s.ShiftId == shift.Id))
                    .ToList();
                
                _totalDetailCount = _allAssignedEmployees.Count;
            }
            finally
            {
                _isLoadingDetail = false;
            }
        }

        private void ChangeDetailPage(int page)
        {
            if (page >= 1 && page <= _totalDetailPages)
            {
                _detailPageIndex = page;
                StateHasChanged();
            }
        }

        private void CloseAssignedObjects()
        {
            _showAssignedObjects = false;
            _selectedShiftForDetail = null;
        }

        private void GoToScheduler(WorkShiftModel shift)
        {
            NavigationManager.NavigateTo($"/attendance/scheduler?shiftId={shift.Id}");
        }
    }
}
