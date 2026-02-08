using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace TTL.HR.Shared.Pages.Attendance
{
    public partial class Timesheet
    {
        private bool _showDetail = false;
        private bool _isLoading = true;
        private int _selectedUserId;

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            // Simulate API loading
            await System.Threading.Tasks.Task.Delay(1400);
            _isLoading = false;
        }

        private void OpenDetail(int userId)
        {
            _selectedUserId = userId;
            _showDetail = true;
        }

        private void CloseDetail()
        {
            _showDetail = false;
        }
    }
}
