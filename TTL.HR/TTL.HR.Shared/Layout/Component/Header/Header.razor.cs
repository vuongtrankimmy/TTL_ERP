using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace TTL.HR.Shared.Layout.Component.Header
{
    public partial class Header : IDisposable
    {
        [Inject] public NavigationManager NavigationManager { get; set; } = default!;
        [Inject] public TTL.HR.Application.Modules.Common.Interfaces.ISettingsService SettingsService { get; set; } = default!;

        private List<BreadcrumbItem> _breadcrumbs = new();

        protected override void OnInitialized()
        {
            NavigationManager.LocationChanged += HandleLocationChanged;
            SettingsService.OnSettingsUpdated += HandleSettingsUpdated;
            UpdateBreadcrumbs();
        }

        private void HandleSettingsUpdated()
        {
            StateHasChanged();
        }

        private void HandleLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            UpdateBreadcrumbs();
            StateHasChanged();
        }


        private void UpdateBreadcrumbs()
        {
            var uri = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            var segments = uri.Split('/', StringSplitOptions.RemoveEmptyEntries);
            _breadcrumbs = segments.Select(s => new BreadcrumbItem { Title = GetTitle(s) }).ToList();
        }

        private string GetTitle(string segment) => segment.ToLower() switch
        {
            "dashboard" => "Dashboard",
            "organization" => "Tổ chức",
            "structure" => "Sơ đồ tổ chức",
            "departments" => "Danh sách phòng ban",
            "positions" => "Chức vụ",
            "employees" => "Danh sách nhân viên",
            "contracts" => "Hợp đồng lao động",
            "attendance" => "Chấm công",
            "timesheet" => "Bảng chấm công tổng hợp",
            "schedule" => "Lịch làm việc",
            "leave" => "Nghỉ phép & Công tác",
            "leave-trip" => "Nghỉ phép & Công tác",
            "approval" => "Phê duyệt đổi ca",
            "approvals" => "Phê duyệt đơn nghỉ",
            "payroll" => "Lương & Phúc lợi",
            "list" => "Bảng lương nhân viên",
            "benefits" => "Phúc lợi",
            "permissions" => "Phân quyền",
            "settings" => "Cấu hình",
            "general" => "Chung",
            "payroll-config" => "Tham số tính lương",
            _ => char.ToUpper(segment[0]) + segment.Substring(1)
        };

        public void Dispose()
        {
            NavigationManager.LocationChanged -= HandleLocationChanged;
            SettingsService.OnSettingsUpdated -= HandleSettingsUpdated;
        }


        private class BreadcrumbItem
        {
            public string Title { get; set; } = "";
        }
    }
}
