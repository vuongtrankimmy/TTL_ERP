using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using TTL.HR.Shared.Interfaces;
using TTL.HR.Shared.Models;

namespace TTL.HR.Shared.Pages.Attendance
{
    public partial class Attendance
    {
        [Inject] private IAttendanceService AttendanceService { get; set; } = default!;

        private bool _showDetail = false;
        private bool _isLoading = true;
        private EmployeeAttendance? _selectedEmployee;
        private List<EmployeeAttendance> _employees = new();

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
                    _employees = timesheets.Select(t => new EmployeeAttendance
                    {
                        Id = t.EmployeeId,
                        Name = t.EmployeeName,
                        Role = "Staff",
                        Department = "General",
                        Avatar = "",
                        StandardWork = "22",
                        ActualWork = (t.CheckIn != null && t.CheckOut != null) ? "1.0" : (t.CheckIn != null ? "0.5" : "0.0"),
                        LateEarly = "0/0",
                        LeaveHoliday = "0/0",
                        Overtime = "0h",
                        Status = t.Status,
                        StatusColor = t.Status == "CheckedIn" ? "badge-light-primary" : (t.Status == "Completed" ? "badge-light-success" : "badge-light-warning")
                    }).ToList();
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

        private void openDetail(EmployeeAttendance emp)
        {
            _selectedEmployee = emp;
            _showDetail = true;
        }

        private void closeDetail()
        {
            _showDetail = false;
        }

        private class EmployeeAttendance
        {
            public string Id { get; set; } = "";
            public string Name { get; set; } = "";
            public string Role { get; set; } = "";
            public string Department { get; set; } = "";
            public string Avatar { get; set; } = "";
            public string StandardWork { get; set; } = "";
            public string ActualWork { get; set; } = "";
            public string LateEarly { get; set; } = "";
            public string LeaveHoliday { get; set; } = "";
            public string Overtime { get; set; } = "";
            public string Status { get; set; } = "";
            public string StatusColor { get; set; } = "";
        }
    }
}
