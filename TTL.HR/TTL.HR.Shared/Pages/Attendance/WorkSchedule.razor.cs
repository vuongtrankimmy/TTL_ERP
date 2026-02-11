using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using TTL.HR.Application.Modules.Attendance.Interfaces;
using TTL.HR.Application.Modules.Attendance.Models;

namespace TTL.HR.Shared.Pages.Attendance
{
    public partial class WorkSchedule
    {
        [Inject] private IAttendanceService AttendanceService { get; set; } = default!;

        private bool _showDetail = false;
        private bool _isLoading = true;
        private WorkScheduleModel? _selectedSchedule;
        private List<WorkScheduleModel> _schedules = new();

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async System.Threading.Tasks.Task LoadData()
        {
            _isLoading = true;
            try
            {
                var result = await AttendanceService.GetWorkSchedulesAsync();
                if (result != null)
                {
                    _schedules = result.ToList();
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

        private void openDetail(WorkScheduleModel item)
        {
            _selectedSchedule = item;
            _showDetail = true;
        }

        private void closeDetail()
        {
            _showDetail = false;
        }
    }
}
