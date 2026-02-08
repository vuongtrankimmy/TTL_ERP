using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace TTL.HR.Shared.Pages.Attendance
{
    public partial class WorkSchedule
    {
        private bool _showDetail = false;
        private bool _isLoading = true;
        private ScheduleItem? _selectedSchedule;

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            // Simulate API loading
            await System.Threading.Tasks.Task.Delay(1300);
            _isLoading = false;
        }

        private void openDetail(ScheduleItem item)
        {
            _selectedSchedule = item;
            _showDetail = true;
        }

        private void closeDetail()
        {
            _showDetail = false;
        }

        private List<ScheduleItem> _schedules = new()
        {
            new() { Id = "NV001", Name = "Nguyễn Văn Lộc", Department = "Kỹ thuật", Avatar = "assets/media/avatars/300-1.jpg", CurrentShift = "Ca hành chính (08h00 - 17h00)", ShiftColor = "badge-light-primary", BulletBg = "bg-primary" },
            new() { Id = "NV002", Name = "Lê Thị Mai", Department = "Marketing", Avatar = "assets/media/avatars/300-2.jpg", CurrentShift = "Ca chiều (14h00 - 22h00)", ShiftColor = "badge-light-success", BulletBg = "bg-success" },
            new() { Id = "NV003", Name = "Phạm Hoàng", Department = "Kỹ thuật", Avatar = "", AvatarBg = "bg-dark", CurrentShift = "Ca đêm (22h00 - 06h00)", ShiftColor = "badge-light-dark", BulletBg = "bg-dark" },
            new() { Id = "NV004", Name = "Trần Minh", Department = "Nhân sự", Avatar = "assets/media/avatars/300-3.jpg", CurrentShift = "Ca sáng (06h00 - 14h00)", ShiftColor = "badge-light-success", BulletBg = "bg-success" },
            new() { Id = "NV005", Name = "Hoàng Nam", Department = "Sản xuất", Avatar = "assets/media/avatars/300-5.jpg", CurrentShift = "Ca gãy (10h-14h & 18h-22h)", ShiftColor = "badge-light-warning", BulletBg = "bg-warning" },
            new() { Id = "NV006", Name = "Kiều Linh", Department = "Thiết kế", Avatar = "", AvatarBg = "bg-warning", CurrentShift = "Lịch linh hoạt (IT/Remote)", ShiftColor = "badge-light-info", BulletBg = "bg-info" },
            new() { Id = "NV007", Name = "Vũ Long", Department = "Vận hành", Avatar = "assets/media/avatars/300-6.jpg", CurrentShift = "Ca 12h (07h00 - 19h00)", ShiftColor = "badge-light-danger", BulletBg = "bg-danger" },
            new() { Id = "NV008", Name = "Đặng Thu", Department = "Tài chính", Avatar = "assets/media/avatars/300-9.jpg", CurrentShift = "Nghỉ lễ / Nghỉ bì", ShiftColor = "badge-light-secondary", BulletBg = "bg-secondary" },
            new() { Id = "NV009", Name = "Bùi Cường", Department = "Kinh doanh", Avatar = "", AvatarBg = "bg-primary", CurrentShift = "Công tác / Đào tạo", ShiftColor = "badge-light-primary", BulletBg = "bg-primary", IsNextShiftAssigned = true, NextShift = "Về VP từ 12/02" },
            new() { Id = "NV010", Name = "Ngô Diệp", Department = "Hành chính", Avatar = "assets/media/avatars/300-11.jpg", CurrentShift = "Chưa gán ca", ShiftColor = "badge-light", BulletBg = "bg-gray-400" }
        };

        private class ScheduleItem
        {
            public string Id { get; set; } = "";
            public string Name { get; set; } = "";
            public string Department { get; set; } = "";
            public string Avatar { get; set; } = "";
            public string AvatarBg { get; set; } = "bg-primary";
            public string CurrentShift { get; set; } = "";
            public string ShiftColor { get; set; } = "";
            public string BulletBg { get; set; } = "";
            public bool IsNextShiftAssigned { get; set; } = false;
            public string NextShift { get; set; } = "";
        }
    }
}
