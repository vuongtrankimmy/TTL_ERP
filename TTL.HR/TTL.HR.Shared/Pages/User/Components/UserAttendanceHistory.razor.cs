using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using TTL.HR.Application.Modules.Attendance.Interfaces;
using TTL.HR.Application.Modules.Attendance.Models;

namespace TTL.HR.Shared.Pages.User.Components
{
    public partial class UserAttendanceHistory
    {
        [Parameter] public string? EmployeeId { get; set; }
        [Parameter] public string? EmployeeName { get; set; }
        [Parameter] public string? EmployeeCode { get; set; }
        [Parameter] public string? AvatarUrl { get; set; }
        
        [Inject] public IAttendanceService AttendanceService { get; set; } = default!;

        private bool _showExplanationDrawer = false;
        private AttendanceDetailModel? _selectedRecordForExplanation;
        private List<AttendanceDetailModel> _attendanceRecords = new();
        private bool _isLoading = true;
        private TTL.HR.Shared.Components.Attendance.CreateShiftRequestModal? _createRequestModal;

        // Tháng đang xem, mặc định là tháng hiện tại
        private DateTime _currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrEmpty(EmployeeId))
            {
                await LoadData();
            }
        }

        private async Task LoadData()
        {
            if (string.IsNullOrEmpty(EmployeeId)) return;

            _isLoading = true;
            StateHasChanged();
            try
            {
                var data = await AttendanceService.GetAttendanceDetailsAsync(EmployeeId, _currentMonth);
                _attendanceRecords = data?.OrderByDescending(x => x.Date).ToList() ?? new List<AttendanceDetailModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading attendance: {ex.Message}");
                _attendanceRecords = new List<AttendanceDetailModel>();
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        private async Task PreviousMonth()
        {
            _currentMonth = _currentMonth.AddMonths(-1);
            await LoadData();
        }

        private async Task NextMonth()
        {
            if (_currentMonth < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1))
            {
                _currentMonth = _currentMonth.AddMonths(1);
                await LoadData();
            }
        }

        private void OpenExplanation(AttendanceDetailModel record)
        {
            _selectedRecordForExplanation = record;
            _showExplanationDrawer = true;
        }

        private void CloseExplanation()
        {
            _showExplanationDrawer = false;
            _selectedRecordForExplanation = null;
        }

        private void OpenCreateRequestModal() => _createRequestModal?.Open();

        private string GetStatusClass(string status) => status switch
        {
            "Normal" or "On Time" or "Đúng giờ" => "badge-light-success",
            "Late" or "Đi muộn" => "badge-light-warning",
            "EarlyLeave" or "Early Leave" or "Về sớm" => "badge-light-warning",
            "Leave" or "Nghỉ phép" => "badge-light-info",
            "Holiday" or "Ngày nghỉ" => "badge-light-secondary",
            "Absent" or "Vắng mặt" => "badge-light-danger",
            _ => "badge-light-primary"
        };

        private string TranslateStatus(string status) => status switch
        {
            "Normal" => "Đúng giờ",
            "On Time" => "Đúng giờ",
            "Late" => "Đi muộn",
            "EarlyLeave" => "Về sớm",
            "Early Leave" => "Về sớm",
            "Leave" => "Nghỉ phép",
            "Holiday" => "Ngày nghỉ",
            "Absent" => "Vắng mặt",
            "Ngày nghỉ" => "Ngày nghỉ",
            "Vắng mặt" => "Vắng mặt",
            _ => status
        };

        // Thống kê tháng
        private int TotalWorkDays => _attendanceRecords.Count(r => r.WorkValue > 0);
        private int LateDays => _attendanceRecords.Count(r => r.Status == "Late" || r.IsLate);
        private int EarlyLeaveDays => _attendanceRecords.Count(r => r.Status == "EarlyLeave" || r.IsEarlyLeave);
        private double TotalOvertimeHours => _attendanceRecords.Sum(r => r.OvertimeHours);
    }
}
