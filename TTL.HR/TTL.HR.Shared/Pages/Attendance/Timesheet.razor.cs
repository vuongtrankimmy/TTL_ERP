using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Attendance.Interfaces;
using TTL.HR.Application.Modules.Attendance.Models;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.Common.Constants;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Organization.Models;
using System.Net.Http;

namespace TTL.HR.Shared.Pages.Attendance
{
    public partial class Timesheet
    {
        [Inject] public IAttendanceService AttendanceService { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;
        [Inject] private IMasterDataService MasterDataService { get; set; } = default!;
        [Inject] private HttpClient _httpClient { get; set; } = default!;

        private List<AttendanceModel> _timesheets = new();
        private List<DepartmentModel> _departments = new();
        private List<DateTime> _periods = new();
        private bool _showDetail = false;
        private bool _isLoading = true;
        private string _selectedEmployeeId = "";
        private string _searchTerm = "";
        private string _selectedDepartmentId = "";
        private int _currentMonth = DateTime.Now.Month;
        private int _currentYear = DateTime.Now.Year;
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalItems = 0;

        protected override async Task OnInitializedAsync()
        {
            // Generate last 12 months for periods
            var today = DateTime.Today;
            for (int i = 0; i < 12; i++)
            {
                _periods.Add(new DateTime(today.Year, today.Month, 1).AddMonths(-i));
            }

            var depts = await MasterDataService.GetCachedLookupsAsync("Department");
            _departments = depts.Select(d => new DepartmentModel { Id = d.Id, Name = d.Name }).ToList();

            await LoadData();
        }

        private async Task LoadData()
        {
            _isLoading = true;
            try
            {
                var result = await AttendanceService.GetTimesheetsAsync(_currentMonth, _currentYear, _selectedDepartmentId, _searchTerm, _currentPage, _pageSize);
                _timesheets = result?.Items?.ToList() ?? new List<AttendanceModel>();
                _totalItems = (int)(result?.TotalCount ?? 0);
            }
            catch (Exception ex)
            {
                var url = $"{_httpClient.BaseAddress}{ApiEndpoints.Attendance.Timesheets}";
                await JS.InvokeVoidAsync("toastr.error", $"Lỗi tải dữ liệu bảng công: {ex.Message} (URL: {url})");
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

        private async Task HandleRecalculate(string employeeId)
        {
            try
            {
                var success = await AttendanceService.RecalculateAttendanceSummaryAsync(employeeId, _currentMonth, _currentYear);
                if (success)
                {
                    await JS.InvokeVoidAsync("toastr.success", "Đã cập nhật dữ liệu mới nhất cho nhân viên");
                    await LoadData();
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", "Không thể cập nhật dữ liệu lúc này");
                }
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("toastr.error", $"Lỗi: {ex.Message}");
            }
        }

        private List<AttendanceModel> FilteredTimesheets => _timesheets;

        private async Task OnSearchChange(string searchTerm)
        {
            _searchTerm = searchTerm;
            _currentPage = 1;
            await LoadData();
        }

        private async Task OnDepartmentChange(ChangeEventArgs e)
        {
            _selectedDepartmentId = e.Value?.ToString() ?? "";
            _currentPage = 1;
            await LoadData();
        }

        private async Task OnPageChange(int page)
        {
            _currentPage = page;
            await LoadData();
        }

        private async Task OnPeriodChange(ChangeEventArgs e)
        {
            if (DateTime.TryParse(e.Value?.ToString(), out DateTime date))
            {
                _currentMonth = date.Month;
                _currentYear = date.Year;
                _currentPage = 1;
                await LoadData();
            }
        }

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
