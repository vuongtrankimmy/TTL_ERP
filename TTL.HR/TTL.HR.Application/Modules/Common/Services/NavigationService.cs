using System.Collections.Generic;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Application.Modules.Common.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IAuthService _authService;
        private readonly ISettingsService _settingsService;

        public NavigationService(IAuthService authService, ISettingsService settingsService)
        {
            _authService = authService;
            _settingsService = settingsService;
        }

        public async Task<bool> UserHasPermissionAsync(string? permission)
        {
            if (string.IsNullOrEmpty(permission)) return true;
            
            var user = await _authService.GetCurrentUserAsync();
            if (user == null) return false;

            // 1. SuperAdmin & Admin bypass
            if (user.Role == "SuperAdmin" || user.Role == "Super Administrator" || user.Role == "Admin") 
            {
                return true;
            }

            // 2. IT Support
            if (user.Role == "IT Support" || user.Role == "ITSupport")
            {
                // IT Support can access settings and logs but NOT payroll or HR details
                if (permission.Contains("Payroll") || permission.Contains("Salary") || permission.Contains("Benefits")) return false;
                if (permission.Contains("Contract")) return false;
                if (permission.Contains("Settings") || permission.Contains("Audit") || permission.Contains("Administration") || permission.Contains("Permissions")) return true;
                return true; 
            }

            // 3. HR Manager
            if (user.Role == "HR Manager" || user.Role == "HRManager")
            {
                // Access to HR, Payroll, Recruitment, Training, Assets
                // But block system-level configuration (Administration)
                if (permission.Contains("Administration.Settings") || permission.Contains("Permissions.Edit")) return false;
                return true;
            }

            // 4. Dept Manager / Team Leader
            if (user.Role == "Dept Manager" || user.Role == "DeptManager" || user.Role == "TeamLead")
            {
                // Can view/approve for their department
                if (permission.Contains("Payroll") || permission.Contains("Settings") || permission.Contains("Contract") || permission.Contains("Administration")) return false;
                
                // Allowed actions
                if (permission.Contains("View") || permission.Contains("Approve") || permission.Contains("Create"))
                {
                    if (permission.Contains("LeaveRequests") || permission.Contains("ShiftRequests") || permission.Contains("Attendance")) return true;
                }
                
                if (permission.Contains("Employees.View")) return true;
                
                return false;
            }

            // 5. Employee (Self-Service)
            if (user.Role == "Employee" || user.Role == "User")
            {
                // Restricted to self-service items
                if (permission.Contains("Administration") || permission.Contains("Settings") || permission.Contains("Payroll.ViewAll")) return false;
                
                // Actions
                if (permission.Contains("Create"))
                {
                    if (permission.Contains("LeaveRequests.Create") || permission.Contains("ShiftRequests.Create")) return true;
                    return false;
                }
                
                // Restricted viewing
                if (permission.Contains("Employees.View") || permission.Contains("Contracts.View")) return false; 
                if (permission.Contains("Payroll.ViewSlip")) return true;
                if (permission.Contains("Attendance.ViewMine")) return true;
                
                return permission.Contains("View") && !permission.Contains("All");
            }

            return false;
        }

        public async Task<List<NavItem>> GetMenuItemsAsync()
        {
            var settings = await _settingsService.GetSettingsAsync();
            var allItems = settings?.SidebarMenu;
            var translations = settings?.Translations ?? new List<LanguageTranslationModel>();
            var language = settings?.DefaultLanguage ?? "vi-VN";

            if (allItems == null || allItems.Count == 0)
            {
                allItems = GetDefaultMenuItems();
            }
            else
            {
                // Ensure new menu items are present even if settings are loaded from DB
                var shiftMgmt = allItems.FirstOrDefault(i => i.Title == "Menu_ShiftManagement");
                if (shiftMgmt != null)
                {
                    if (!shiftMgmt.SubItems.Any(s => s.Title == "Menu_OvertimeRequests"))
                    {
                        shiftMgmt.SubItems.Add(new NavItem { Title = "Menu_OvertimeRequests", Href = "/attendance/overtime", Icon = "ki-outline ki-clock", Permission = "Permissions.ShiftRequests.View" });
                    }
                    if (!shiftMgmt.SubItems.Any(s => s.Title == "Menu_ShiftApproval"))
                    {
                        shiftMgmt.SubItems.Add(new NavItem { Title = "Menu_ShiftApproval", Href = "/attendance/approval", Icon = "ki-outline ki-check-square", Permission = "Permissions.ShiftRequests.View" });
                    }
                }
            }

            return await FilterMenuItemsAsync(allItems, translations, language);
        }

        private async Task<List<NavItem>> FilterMenuItemsAsync(List<NavItem> items, List<LanguageTranslationModel> translations, string language)
        {
            var filtered = new List<NavItem>();

            foreach (var item in items)
            {
                if (item.IsActive && await UserHasPermissionAsync(item.Permission))
                {
                    var newItem = new NavItem
                    {
                        Id = item.Id,
                        NumericId = item.NumericId,
                        Title = Translate(item.NumericId, item.Title, translations, language),
                        Icon = item.Icon,
                        Href = item.Href,
                        Permission = item.Permission,
                        IsSection = item.IsSection,
                        IsActive = item.IsActive,
                        Order = item.Order
                    };

                    if (item.HasSubItems)
                    {
                        newItem.SubItems = await FilterMenuItemsAsync(item.SubItems, translations, language);
                    }

                    if (item.IsSection || !string.IsNullOrEmpty(newItem.Href) || newItem.HasSubItems)
                    {
                        filtered.Add(newItem);
                    }
                }
            }

            return filtered;
        }

        private string Translate(int navId, string? key, List<LanguageTranslationModel> translations, string languageCode)
        {
            if (navId <= 0 && string.IsNullOrEmpty(key)) return string.Empty;
            
            var translation = translations.FirstOrDefault(t => t.NavigationID == navId && t.LanguageCode == languageCode);
            if (translation != null) return translation.Value;

            // Fallback to English if not found in current language
            if (languageCode != "en-US")
            {
                var enTranslation = translations.FirstOrDefault(t => t.NavigationID == navId && t.LanguageCode == "en-US");
                if (enTranslation != null) return enTranslation.Value;
            }

            // Fallback to the key (TitleKey) if it looks like a resource key or the key itself
            return !string.IsNullOrEmpty(key) ? key : $"Menu_{navId}";
        }

        private List<NavItem> GetDefaultMenuItems()
        {
            return new List<NavItem>
            {
                new NavItem { Title = "Menu_Dashboards", Icon = "ki-outline ki-element-11", Href = "/dashboard" },
                
                new NavItem { Title = "Section_OrgAndPersonnel", IsSection = true },
                new NavItem { 
                    Title = "Menu_CompanyStructure", 
                    Icon = "ki-outline ki-address-book", 
                    SubItems = new List<NavItem>
                    {
                        new NavItem { Title = "Menu_OrgChart", Href = "/organization/structure", Permission = "Permissions.OrgChart.View" },
                        new NavItem { Title = "Menu_Departments", Href = "/organization/departments", Permission = "Permissions.Administration.Settings" },
                        new NavItem { Title = "Menu_Positions", Href = "/organization/positions", Permission = "Permissions.Administration.Settings" }
                    }
                },
                new NavItem { 
                    Title = "Menu_EmployeeManagement", 
                    Icon = "ki-outline ki-people", 
                    SubItems = new List<NavItem>
                    {
                        new NavItem { Title = "Menu_EmployeeList", Href = "/employees", Permission = "Permissions.Employees.View" },
                        new NavItem { Title = "Menu_Onboarding", Href = "/employees/add", Permission = "Permissions.Employees.Create" },
                        new NavItem { Title = "Menu_Contracts", Href = "/contracts", Permission = "Permissions.Contracts.View" }
                    }
                },
                
                new NavItem { Title = "Section_HROperations", IsSection = true },
                new NavItem { 
                    Title = "Menu_Attendance", 
                    Icon = "ki-outline ki-timer", 
                    SubItems = new List<NavItem>
                    {
                        new NavItem { Title = "Menu_MonthlyTimesheet", Href = "/attendance/timesheet", Permission = "Permissions.Attendance.View" },
                        new NavItem { Title = "Menu_AttendanceData", Href = "/attendance", Permission = "Permissions.Attendance.View" },
                        new NavItem { Title = "Menu_ImportData", Href = "/attendance/import", Permission = "Permissions.Attendance.Edit" }
                    }
                },
                new NavItem
                {
                    Title = "Menu_ShiftManagement",
                    Icon = "ki-outline ki-calendar-8",
                    SubItems = new List<NavItem>
                    {
                        new NavItem { Title = "Menu_ShiftCatalog", Href = "/attendance/shifts", Icon = "ki-outline ki-category", Permission = "Permissions.Administration.Settings" },
                        new NavItem { Title = "Menu_WorkSchedule", Href = "/attendance/schedule", Icon = "ki-outline ki-calendar-edit", Permission = "Permissions.Attendance.Edit" },
                        new NavItem { Title = "Menu_OvertimeRequests", Href = "/attendance/overtime", Icon = "ki-outline ki-clock", Permission = "Permissions.ShiftRequests.View" },
                        new NavItem { Title = "Menu_ShiftApproval", Href = "/attendance/approval", Icon = "ki-outline ki-check-square", Permission = "Permissions.ShiftRequests.View" }
                    }
                },
                new NavItem { 
                    Title = "Menu_LeaveManagement", 
                    Icon = "ki-outline ki-calendar-8", 
                    SubItems = new List<NavItem>
                    {
                        new NavItem { Title = "Menu_LeaveRequests", Href = "/leave/requests", Permission = "Permissions.LeaveRequests.View" },
                        new NavItem { Title = "Menu_BusinessTrip", Href = "/attendance/leave-trip", Permission = "Permissions.LeaveRequests.View" }
                    }
                },
                new NavItem { 
                    Title = "Menu_PayrollAndBenefits", 
                    Icon = "ki-outline ki-wallet", 
                    SubItems = new List<NavItem>
                    {
                        new NavItem { Title = "Menu_PayrollPeriods", Href = "/payroll/periods", Permission = "Permissions.Payroll.View" },
                        new NavItem { Title = "Menu_Benefits", Href = "/benefits", Permission = "Permissions.Benefits.View" }
                    }
                },
                
                new NavItem { Title = "Section_RecruitmentAndTraining", IsSection = true },
                new NavItem { Title = "Menu_Recruitment", Icon = "ki-outline ki-user-tick", Href = "/recruitment", Permission = "Permissions.Recruitment.View" },
                new NavItem { Title = "Menu_Training", Icon = "ki-outline ki-book-open", Href = "/training", Permission = "Permissions.Training.View" },
                
                new NavItem { Title = "Section_AssetsAndEquipment", IsSection = true },
                new NavItem { 
                    Title = "Menu_AssetManagement", 
                    Icon = "ki-outline ki-monitor-mobile", 
                    Permission = "Permissions.Assets.View",
                    SubItems = new List<NavItem>
                    {
                        new NavItem { Title = "Menu_AssetInventory", Href = "/assets/inventory", Permission = "Permissions.Assets.View" },
                        new NavItem { Title = "Menu_AssetAllocation", Href = "/assets/allocation", Permission = "Permissions.Assets.View" }
                    }
                },
                
                new NavItem { Title = "Section_System", IsSection = true, Permission = "Permissions.Administration.Settings" },
                new NavItem { 
                    Title = "Menu_AdminAndConfig", 
                    Icon = "ki-outline ki-setting-2", 
                    Permission = "Permissions.Administration.Settings",
                    SubItems = new List<NavItem>
                    {
                        new NavItem { Title = "Menu_RolesAndGroups", Href = "/administration/roles", Permission = "Permissions.Administration.Settings" },
                        new NavItem { Title = "Menu_Permissions", Href = "/permissions", Permission = "Permissions.Administration.Settings" },
                        new NavItem { Title = "Menu_GeneralSettings", Href = "/settings/general", Permission = "Permissions.Administration.Settings" },
                        new NavItem { Title = "Menu_Banks", Href = "/settings/banks", Permission = "Permissions.Administration.Settings" },
                        new NavItem { Title = "Menu_PayrollConfig", Href = "/settings/payroll-config", Permission = "Permissions.Administration.Settings" }
                    }
                },
                
                new NavItem { Title = "Section_Support", IsSection = true },
                new NavItem { Title = "Menu_HRToolkit", Icon = "ki-outline ki-calculator", Href = "/tools/hr-toolkit" },
                new NavItem { Title = "Menu_UserGuide", Icon = "ki-outline ki-book-open", Href = "/system/guide" }
            };
        }
    }
}
