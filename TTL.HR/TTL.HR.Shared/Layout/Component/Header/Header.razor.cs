using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Shared;

namespace TTL.HR.Shared.Layout.Component.Header
{
    public partial class Header : IDisposable
    {
        [Inject] public NavigationManager NavigationManager { get; set; } = default!;
        [Inject] public ISettingsService SettingsService { get; set; } = default!;
        [Inject] public INotificationService NotificationService { get; set; } = default!;
        [Inject] public IStringLocalizer<SharedResource> L { get; set; } = default!;
        [Inject] public IJSRuntime JSRuntime { get; set; } = default!;

        private List<BreadcrumbItem> _breadcrumbs = new();
        private List<NotificationModel> _notifications = new();
        private List<RecentMenuModel> _recentMenus = new();
        private string _activeNotifyTab = "Notify";

        protected override async Task OnInitializedAsync()
        {
            NavigationManager.LocationChanged += HandleLocationChanged;
            SettingsService.OnSettingsUpdated += HandleSettingsUpdated;
            UpdateBreadcrumbs();
            await LoadNotifications();
            await LoadRecentMenus();
        }

        private void HandleSettingsUpdated()
        {
            StateHasChanged();
        }

        private async void HandleLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            UpdateBreadcrumbs();
            await TrackRecentMenu();
            StateHasChanged();
        }


        private void UpdateBreadcrumbs()
        {
            var uri = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            var queryIndex = uri.IndexOf('?');
            if (queryIndex >= 0) uri = uri.Substring(0, queryIndex);
            
            var segments = uri.Split('/', StringSplitOptions.RemoveEmptyEntries);
            _breadcrumbs = segments.Select(s => new BreadcrumbItem { Title = GetTitle(s) }).ToList();
        }

        private string GetTitle(string segment)
        {
            if (string.IsNullOrEmpty(segment)) return "Menu_Dashboards";
            
            // Check if segment is an ID (GUID or ObjectId)
            if ((segment.Length >= 24) && System.Text.RegularExpressions.Regex.IsMatch(segment, @"^[a-fA-F0-9-]+$"))
            {
                return "Label_Details";
            }

            var lowSegment = segment.ToLower();

            // Try to find mapping in database/settings
            var mapping = SettingsService.CachedSettings?.UrlTitleMappings?.FirstOrDefault(m => m.Segment.Equals(lowSegment, StringComparison.OrdinalIgnoreCase));
            if (mapping != null)
            {
                return mapping.TitleKey;
            }

            // Fallback for known legacy segments or default behavior
            return lowSegment switch
            {
                "dashboard" => "Menu_Dashboards",
                "organization" => "Section_OrgAndPersonnel",
                "structure" => "Menu_OrgChart",
                "departments" => "Menu_Departments",
                "positions" => "Menu_Positions",
                "employees" => "Menu_EmployeeList",
                "add" => "Menu_Onboarding",
                "edit" => "Label_Edit",
                "contracts" => "Menu_Contracts",
                "attendance" => "Menu_Attendance",
                "timesheet" => "Menu_MonthlyTimesheet",
                "schedule" => "Menu_WorkSchedule",
                "leave" => "Menu_LeaveManagement",
                "leave-trip" => "Menu_BusinessTrip",
                "approval" => "Menu_ShiftApproval",
                "approvals" => "Menu_LeaveApprovals",
                "payroll" => "Menu_PayrollAndBenefits",
                "periods" => "Menu_PayrollPeriods",
                "benefits" => "Menu_Benefits",
                "recruitment" => "Menu_Recruitment",
                "training" => "Menu_Training",
                "assets" => "Menu_AssetManagement",
                "inventory" => "Menu_AssetInventory",
                "allocation" => segment.ToLower() == "allocation" && NavigationManager.Uri.ToLower().Contains("attendance") ? "Menu_ShiftAllocation" : "Menu_AssetAllocation",
                "administration" => "Menu_AdminAndConfig",
                "roles" => "Menu_RolesAndGroups",
                "settings" => "Menu_AdminAndConfig",
                "general" => "Menu_GeneralSettings",
                "banks" => "Menu_Banks",
                "payroll-config" => "Menu_PayrollConfig",
                _ => segment
            };
        }

        private async Task LoadNotifications()
        {
            _notifications = await NotificationService.GetMyNotificationsAsync(50);
            StateHasChanged();
        }

        private async Task MarkAsRead(NotificationModel notify)
        {
            if (notify.IsRead) return;
            var success = await NotificationService.MarkAsReadAsync(notify.Id);
            if (success)
            {
                notify.IsRead = true;
                StateHasChanged();
            }
        }

        private async Task LoadRecentMenus()
        {
            try
            {
                var json = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "recent_menus");
                if (!string.IsNullOrEmpty(json))
                {
                    _recentMenus = System.Text.Json.JsonSerializer.Deserialize<List<RecentMenuModel>>(json) ?? new();
                }
            }
            catch { }
        }

        private async Task TrackRecentMenu()
        {
            var uri = "/" + NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            if (uri.Contains("?")) uri = uri.Substring(0, uri.IndexOf('?'));
            if (uri == "/") return; // Don't track dashboard or home

            var settings = SettingsService.CachedSettings;
            if (settings == null || settings.SidebarMenu == null) return;

            // Find the menu item in sidebar
            var allItems = new List<NavItem>();
            FlattenMenu(settings.SidebarMenu, allItems);

            var currentItem = allItems.FirstOrDefault(i => i.Href == uri);
            if (currentItem != null)
            {
                var recent = _recentMenus.FirstOrDefault(r => r.Href == uri);
                if (recent != null) _recentMenus.Remove(recent);

                _recentMenus.Insert(0, new RecentMenuModel 
                { 
                    Title = currentItem.Title, 
                    Href = currentItem.Href, 
                    Icon = currentItem.Icon,
                    LastAccessed = DateTime.Now 
                });

                if (_recentMenus.Count > 5) _recentMenus = _recentMenus.Take(5).ToList();

                try
                {
                    await JSRuntime.InvokeVoidAsync("localStorage.setItem", "recent_menus", System.Text.Json.JsonSerializer.Serialize(_recentMenus));
                }
                catch (JSDisconnectedException) { /* Ignore if circuit is already disconnected */ }
                catch (TaskCanceledException) { /* Ignore if task was canceled */ }
                catch (Exception) { /* Ignore other JS errors for tracking purposes */ }
            }
        }

        private void FlattenMenu(List<NavItem> items, List<NavItem> result)
        {
            foreach (var item in items)
            {
                result.Add(item);
                if (item.HasSubItems) FlattenMenu(item.SubItems, result);
            }
        }

        private string GetNotifyBgClass(string type)
        {
            return type switch
            {
                "Notify" => "bg-light-primary",
                "Email" => "bg-light-success",
                "SMS" => "bg-light-info",
                "System" => "bg-light-danger",
                _ => "bg-light-light"
            };
        }

        private string GetNotifyTextClass(string type)
        {
            return type switch
            {
                "Notify" => "text-primary",
                "Email" => "text-success",
                "SMS" => "text-info",
                "System" => "text-danger",
                _ => "text-gray-600"
            };
        }

        private string GetTimeAgo(DateTime dateTime)
        {
            var span = DateTime.Now - dateTime;
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes} m";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours} h";
            return dateTime.ToString("dd/MM");
        }

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
