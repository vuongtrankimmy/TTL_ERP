using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Attendance.Interfaces;
using TTL.HR.Application.Modules.Attendance.Models;
using TTL.HR.Application.Modules.HumanResource.Interfaces;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Shared.Components.Attendance;

namespace TTL.HR.Shared.Pages.Attendance
{
    public partial class AttendanceApproval : ComponentBase
    {
        [Inject] public IAttendanceService AttendanceService { get; set; } = default!;
        [Inject] public IEmployeeService EmployeeService { get; set; } = default!;
        [Inject] public IAuthService AuthService { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;

        private List<ShiftRequestModel> _requests = new();
        private ShiftRequestSummaryModel _summary = new();
        private bool _isLoading = true;
        private bool _isRecalculating = false;
        private string _activeTab = "All";
        private string _searchTerm = "";
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalItems = 0;

        private string _myEmployeeId = "";
        private string _myEmployeeName = "";
        private string _myEmployeeCode = "";
        private string _myAvatar = "";

        private string _selectedEmployeeId = "";
        private bool _showStats = false;
        private CreateShiftRequestModal? _createModal;

        protected override async Task OnInitializedAsync()
        {
            var user = await AuthService.GetCurrentUserAsync();
            if (user != null)
            {
                var emp = await EmployeeService.GetMyEmployeeAsync();
                if (emp != null)
                {
                    _myEmployeeId = emp.Id;
                    _myEmployeeName = emp.FullName ?? "";
                    _myEmployeeCode = emp.Code ?? "";
                    _myAvatar = emp.AvatarUrl ?? "";
                }
            }

            await LoadData();
        }

        private async Task LoadData()
        {
            _isLoading = true;
            try
            {
                var status = _activeTab == "All" ? null : _activeTab;
                var result = await AttendanceService.GetShiftRequestsAsync(_currentPage, _pageSize, status, _searchTerm);
                _requests = result?.Items?.ToList() ?? new List<ShiftRequestModel>();
                _totalItems = (int)(result?.TotalCount ?? 0);
                
                _summary = await AttendanceService.GetShiftRequestSummaryAsync();
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("toastr.error", $"Lỗi tải dữ liệu: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task SetTab(string tab)
        {
            _activeTab = tab;
            _currentPage = 1;
            await LoadData();
        }

        private async Task HandleSearch()
        {
            _currentPage = 1;
            await LoadData();
        }

        private async Task OnPageChange(int page)
        {
            _currentPage = page;
            await LoadData();
        }

        private void OpenCreateModal() => _createModal?.Open();

        private void OpenStats(string employeeId)
        {
            _selectedEmployeeId = employeeId;
            _showStats = true;
        }

        private void CloseStats() => _showStats = false;
        
        private async Task HandleRecalculate()
        {
            if (string.IsNullOrEmpty(_myEmployeeId)) return;
            
            _isRecalculating = true;
            try
            {
                var now = DateTime.Now;
                var success = await AttendanceService.RecalculateAttendanceSummaryAsync(_myEmployeeId, now.Month, now.Year);
                if (success)
                {
                    await JS.InvokeVoidAsync("toastr.success", "Đã cập nhật dữ liệu chấm công mới nhất");
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
            finally
            {
                _isRecalculating = false;
            }
        }

        private async Task ConfirmWithdraw(string id)
        {
            bool confirmed = await JS.InvokeAsync<bool>("confirm", "Bạn có chắc chắn muốn hủy yêu cầu này không?");
            if (confirmed)
            {
                var success = await AttendanceService.WithdrawShiftRequestAsync(id);
                if (success)
                {
                    await JS.InvokeVoidAsync("toastr.success", "Đã hủy yêu cầu thành công");
                    await LoadData();
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", "Không thể hủy yêu cầu này");
                }
            }
        }

        private string TranslateStatus(string status)
        {
            return status switch
            {
                "Pending" => "Chờ duyệt",
                "Approved" => "Đã duyệt",
                "Rejected" => "Đã hủy",
                _ => status
            };
        }

        private string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "NV";
            var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0].Length >= 2 ? parts[0].Substring(0, 2).ToUpper() : parts[0].ToUpper();
            return (parts[0].Substring(0, 1) + parts[^1].Substring(0, 1)).ToUpper();
        }

        private readonly string[] _colors = { "primary", "success", "info", "warning", "danger", "dark" };
        private string GetRandomColor(string text)
        {
            if (string.IsNullOrEmpty(text)) return "bg-light-primary text-primary";
            var hash = 0;
            foreach (var c in text) hash += c;
            var index = hash % _colors.Length;
            return $"bg-light-{_colors[index]} text-{_colors[index]} fw-bold fs-7";
        }
    }
}
