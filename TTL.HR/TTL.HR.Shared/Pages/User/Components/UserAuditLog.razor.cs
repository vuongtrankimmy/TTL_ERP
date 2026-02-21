using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Shared.Pages.User.Components
{
    public partial class UserAuditLog
    {
        [Parameter] public string? EmployeeId { get; set; }
        [Inject]
        private IAuditService AuditService { get; set; } = default!;

        private bool _isLoading = true;
        private string SearchTerm = "";
        private List<AuditLogModel> _logs = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadLogs();
        }

        private async Task LoadLogs()
        {
            _isLoading = true;
            try
            {
                _logs = await AuditService.GetMyAuditLogsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading logs: {ex.Message}");
                _logs = new List<AuditLogModel>();
            }
            finally
            {
                _isLoading = false;
            }
        }

        private IEnumerable<AuditLogModel> FilteredLogs => 
            string.IsNullOrWhiteSpace(SearchTerm) 
            ? _logs 
            : _logs.Where(l => l.Action.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) || 
                               l.Description.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                               l.Module.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));

        private string GetActionColor(string action)
        {
            return action switch
            {
                "Create" or "Đăng nhập" => "primary",
                "Update" or "Đổi mật khẩu" => "warning",
                "Delete" or "Cập nhật hồ sơ" => "danger",
                "SoftDelete" or "Tải tài liệu" => "info",
                "View" or "Xem bảng lương" => "dark",
                _ => "secondary"
            };
        }

        private string GetActionIcon(string action)
        {
            return action switch
            {
                "Create" or "Đăng nhập" => "ki-plus-square",
                "Update" or "Đổi mật khẩu" => "ki-arrows-circle",
                "Delete" or "Cập nhật hồ sơ" => "ki-trash",
                "View" or "Xem bảng lương" => "ki-eye",
                _ => "ki-notification-on"
            };
        }
    }
}
