using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Attendance.Interfaces;
using TTL.HR.Application.Modules.Attendance.Models;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.Common.Constants;
using TTL.HR.Application.Modules.Common.Interfaces;

namespace TTL.HR.Shared.Pages.Attendance
{
    public partial class Attendance
    {
        [Inject] private IAttendanceService AttendanceService { get; set; } = default!;
        [Inject] private HttpClient _httpClient { get; set; } = default!;
        [Inject] private IMasterDataService MasterDataService { get; set; } = default!;

        private bool _showDetail = false;
        private bool _isLoading = true;
        private AttendanceModel? _selectedEmployee;
        private List<AttendanceModel> _employees = new();
        private List<AttendanceDetailModel> _selectedDetails = new();
        private List<LookupModel> _departments = new();
        private List<DateTime> _periods = new();
        private string _searchTerm = "";
        private string _selectedDepartmentId = "";
        private int _currentMonth = DateTime.Now.Month;
        private int _currentYear = DateTime.Now.Year;

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            // Generate last 12 months for periods
            var today = DateTime.Today;
            for (int i = 0; i < 12; i++)
            {
                _periods.Add(new DateTime(today.Year, today.Month, 1).AddMonths(-i));
            }

            _departments = await MasterDataService.GetCachedLookupsAsync("Department");

            await LoadData();
        }

        [Inject] public IJSRuntime JS { get; set; } = default!;

        private async System.Threading.Tasks.Task LoadData()
        {
            _isLoading = true;
            try
            {
                var timesheets = await AttendanceService.GetTimesheetsAsync(_currentMonth, _currentYear, _selectedDepartmentId, _searchTerm);
                _employees = timesheets?.ToList() ?? new List<AttendanceModel>();
            }
            catch (Exception ex)
            {
                var url = $"{ApiEndpoints.Attendance.Timesheets}";
                await JS.InvokeVoidAsync("toastr.error", $"Lỗi tải dữ liệu chấm công: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        private async Task RefreshData() => await LoadData();

        private async Task OnSearchChange(string searchTerm)
        {
            _searchTerm = searchTerm;
            await LoadData();
        }

        private async Task OnDepartmentChange(ChangeEventArgs e)
        {
            _selectedDepartmentId = e.Value?.ToString() ?? "";
            await LoadData();
        }
        
        private async Task OnPeriodChange(ChangeEventArgs e)
        {
             if (DateTime.TryParse(e.Value?.ToString(), out DateTime date))
             {
                 _currentMonth = date.Month;
                 _currentYear = date.Year;
                 await LoadData();
             }
        }

        private async System.Threading.Tasks.Task openDetail(AttendanceModel emp)
        {
            _selectedEmployee = emp;
            _selectedDetails = (await AttendanceService.GetAttendanceDetailsAsync(emp.EmployeeId, new DateTime(_currentYear, _currentMonth, 1))).ToList();
            _showDetail = true;
            StateHasChanged();
        }

        private void closeDetail()
        {
            _showDetail = false;
        }

        private List<AttendanceModel> FilteredEmployees => _employees;
        
        private bool _isPeriodLocked => _employees.Any() && _employees.All(e => e.Status == "Locked" || e.Status == "Đã chốt");

        private async Task LockMonth()
        {
            _isLoading = true;
            try
            {
                var result = await AttendanceService.CloseMonthlyAsync(_currentMonth, _currentYear);
                if (result.Success)
                {
                    await JS.InvokeVoidAsync("toastr.success", result.Message ?? "Đã chốt công tháng thành công!");
                    await LoadData();
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", result.Message ?? "Có lỗi xảy ra khi chốt công.");
                }
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("toastr.error", $"Lỗi: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private string GetStatusColor(string status)
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
