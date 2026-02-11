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
        [Parameter] public string EmployeeId { get; set; } = "";
        [Inject] public IAttendanceService AttendanceService { get; set; } = default!;

        private bool _showExplanationDrawer = false;
        private AttendanceDetailModel? _selectedRecordForExplanation;
        private List<AttendanceDetailModel> _attendanceRecords = new();
        private bool _isLoading = true;

        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrEmpty(EmployeeId))
            {
                await LoadData();
            }
        }

        private async Task LoadData()
        {
            _isLoading = true;
            try
            {
                var month = new DateTime(2026, 2, 1); // Current month for demo
                var data = await AttendanceService.GetAttendanceDetailsAsync(EmployeeId, month);
                _attendanceRecords = data?.OrderByDescending(x => x.Date).ToList() ?? new List<AttendanceDetailModel>();
            }
            catch (Exception)
            {
                // Error handled by parent or toastr
            }
            finally
            {
                _isLoading = false;
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

        private string GetStatusClass(string status)
        {
            return status switch
            {
                "Đúng giờ" or "On Time" => "badge-light-success",
                "Đi muộn" or "Late" => "badge-light-warning",
                "Về sớm" or "Early Leave" => "badge-light-warning",
                "Nghỉ phép" or "Leave" => "badge-light-info",
                "Ngày nghỉ" or "Holiday" => "badge-light-secondary",
                "Vắng mặt" or "Absent" => "badge-light-danger",
                _ => "badge-light-primary"
            };
        }

        private string TranslateStatus(string status)
        {
            return status switch
            {
                "On Time" => "Đúng giờ",
                "Late" => "Đi muộn",
                "Early Leave" => "Về sớm",
                "Leave" => "Nghỉ phép",
                "Holiday" => "Ngày nghỉ",
                "Absent" => "Vắng mặt",
                _ => status
            };
        }
    }
}
