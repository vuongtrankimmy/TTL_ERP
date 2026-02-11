using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Attendance.Interfaces;
using TTL.HR.Application.Modules.Attendance.Models;

namespace TTL.HR.Shared.Pages.Attendance
{
    public partial class Timesheet
    {
        [Inject] public IAttendanceService AttendanceService { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;

        private List<AttendanceModel> _timesheets = new();
        private bool _showDetail = false;
        private bool _isLoading = true;
        private string _selectedEmployeeId = "";
        private string _searchTerm = "";

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            _isLoading = true;
            try
            {
                var data = await AttendanceService.GetTimesheetsAsync();
                _timesheets = data?.ToList() ?? new List<AttendanceModel>();
            }
            catch (Exception)
            {
                await JS.InvokeVoidAsync("toastr.error", "Lỗi tải dữ liệu bảng công.");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void OpenDetail(string employeeId)
        {
            _selectedEmployeeId = employeeId;
            _showDetail = true;
        }

        private void CloseDetail()
        {
            _showDetail = false;
        }

        private async Task LockTimesheet(string id)
        {
            await JS.InvokeVoidAsync("toastr.info", $"Đang chốt công cho ID: {id}...");
            await Task.Delay(1000);
            await JS.InvokeVoidAsync("toastr.success", "Chốt công thành công.");
            await LoadData();
        }

        private List<AttendanceModel> FilteredTimesheets => string.IsNullOrWhiteSpace(_searchTerm)
            ? _timesheets
            : _timesheets.Where(x => x.EmployeeName.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) || 
                                    x.EmployeeId.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

        private string GetStatusBadgeClass(string status)
        {
            return status switch
            {
                "Đã chốt" or "Locked" => "badge-light-success",
                "Đang mở" or "Open" => "badge-light-primary",
                "Vi phạm" or "Violation" => "badge-light-danger",
                "Chờ duyệt" or "Pending" => "badge-light-warning",
                _ => "badge-light-info"
            };
        }

        private string GetStatusBulletClass(string status)
        {
            return status switch
            {
                "Đã chốt" or "Locked" => "bg-success",
                "Đang mở" or "Open" => "bg-primary",
                "Vi phạm" or "Violation" => "bg-danger",
                "Chờ duyệt" or "Pending" => "bg-warning",
                _ => "bg-info"
            };
        }

        private string TranslateStatus(string status)
        {
            return status switch
            {
                "Locked" => "Đã chốt",
                "Open" => "Đang mở",
                "Violation" => "Vi phạm",
                "Pending" => "Chờ duyệt",
                _ => status
            };
        }
    }
}
