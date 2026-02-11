using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using TTL.HR.Application.Modules.Attendance.Interfaces;
using TTL.HR.Application.Modules.Attendance.Models;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Shared.Pages.Attendance
{
    public partial class Attendance
    {
        [Inject] private IAttendanceService AttendanceService { get; set; } = default!;

        private bool _showDetail = false;
        private bool _isLoading = true;
        private AttendanceModel? _selectedEmployee;
        private List<AttendanceModel> _employees = new();
        private List<AttendanceDetailModel> _selectedDetails = new();

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async System.Threading.Tasks.Task LoadData()
        {
            _isLoading = true;
            try
            {
                var timesheets = await AttendanceService.GetTimesheetsAsync();
                if (timesheets != null)
                {
                    _employees = timesheets.ToList();
                }
            }
            catch (Exception)
            {
                // Error handling
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async System.Threading.Tasks.Task openDetail(AttendanceModel emp)
        {
            _selectedEmployee = emp;
            _selectedDetails = (await AttendanceService.GetAttendanceDetailsAsync(emp.EmployeeId, DateTime.Now)).ToList();
            _showDetail = true;
            StateHasChanged();
        }

        private void closeDetail()
        {
            _showDetail = false;
        }

        private string GetStatusColor(string status)
        {
            return status switch
            {
                "CheckedIn" => "badge-light-primary",
                "Completed" => "badge-light-success",
                "Absent" => "badge-light-danger",
                _ => "badge-light-warning"
            };
        }
    }
}
