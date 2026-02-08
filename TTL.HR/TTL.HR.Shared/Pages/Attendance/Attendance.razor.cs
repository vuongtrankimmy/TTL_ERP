using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace TTL.HR.Shared.Pages.Attendance
{
    public partial class Attendance
    {
        private bool _showDetail = false;
        private bool _isLoading = true;
        private EmployeeAttendance? _selectedEmployee;

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            // Simulate API loading
            await System.Threading.Tasks.Task.Delay(1100);
            _isLoading = false;
        }

        private List<EmployeeAttendance> _employees = new()
        {
            new() { Id = "NV001", Name = "Nguyễn Văn Lộc", Role = "Dev Lead", Department = "Phòng Kỹ thuật", Avatar = "assets/media/avatars/300-1.jpg", StandardWork = "22", ActualWork = "22.0", LateEarly = "2/0", LeaveHoliday = "0/0", Overtime = "4.5h", Status = "Đang mở", StatusColor = "badge-light-primary" },
            new() { Id = "NV002", Name = "Lê Thị Mai", Role = "Marketing", Department = "Phòng Marketing", Avatar = "assets/media/avatars/300-2.jpg", StandardWork = "22", ActualWork = "21.5", LateEarly = "0/1", LeaveHoliday = "0/0", Overtime = "0h", Status = "Đang mở", StatusColor = "badge-light-primary" },
            new() { Id = "NV003", Name = "Phạm Hoàng", Role = "Team Lead", Department = "Phòng Kỹ thuật", Avatar = "", StandardWork = "22", ActualWork = "22.0", LateEarly = "0/0", LeaveHoliday = "0/0", Overtime = "12.0h", Status = "Đã chốt", StatusColor = "badge-light-success" },
            new() { Id = "NV004", Name = "Trần Minh", Role = "HR Manager", Department = "Phòng Nhân sự", Avatar = "assets/media/avatars/300-3.jpg", StandardWork = "22", ActualWork = "22.0", LateEarly = "1/0", LeaveHoliday = "0/0", Overtime = "2.0h", Status = "Đang mở", StatusColor = "badge-light-primary" },
            new() { Id = "NV005", Name = "Hoàng Nam", Role = "Sales Executive", Department = "Phòng Kinh doanh", Avatar = "assets/media/avatars/300-5.jpg", StandardWork = "22", ActualWork = "18.0", LateEarly = "5/3", LeaveHoliday = "4/0", Overtime = "0h", Status = "Thiếu công", StatusColor = "badge-light-danger" },
            new() { Id = "NV006", Name = "Kiều Linh", Role = "Senior Designer", Department = "Phòng Marketing", Avatar = "", StandardWork = "22", ActualWork = "20.0", LateEarly = "0/0", LeaveHoliday = "2/0", Overtime = "0h", Status = "Nghỉ phép", StatusColor = "badge-light-warning" },
            new() { Id = "NV007", Name = "Vũ Long", Role = "DevOps Engineer", Department = "Phòng Kỹ thuật", Avatar = "assets/media/avatars/300-6.jpg", StandardWork = "22", ActualWork = "23.0", LateEarly = "0/0", LeaveHoliday = "0/1", Overtime = "15.5h", Status = "Làm lễ", StatusColor = "badge-light-info" },
            new() { Id = "NV008", Name = "Đặng Thu", Role = "Accountant", Department = "Phòng Tài chính", Avatar = "assets/media/avatars/300-9.jpg", StandardWork = "22", ActualWork = "22.0", LateEarly = "0/0", LeaveHoliday = "0/0", Overtime = "0h", Status = "Đã chốt", StatusColor = "badge-light-success" },
            new() { Id = "NV009", Name = "Bùi Cường", Role = "Frontend Dev", Department = "Phòng Kỹ thuật", Avatar = "", StandardWork = "22", ActualWork = "15.0", LateEarly = "0/0", LeaveHoliday = "0/0", Overtime = "0h", Status = "Công tác", StatusColor = "badge-light-info" },
            new() { Id = "NV010", Name = "Ngô Diệp", Role = "L&D Specialist", Department = "Phòng Nhân sự", Avatar = "assets/media/avatars/300-11.jpg", StandardWork = "22", ActualWork = "10.0", LateEarly = "0/0", LeaveHoliday = "0/0", Overtime = "0h", Status = "Mới vào", StatusColor = "badge-light-primary" },
        };

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
