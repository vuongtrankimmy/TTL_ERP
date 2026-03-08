using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Attendance.Interfaces;
using TTL.HR.Application.Modules.Attendance.Models;
using TTL.HR.Application.Modules.HumanResource.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Models;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Shared.Components.Attendance
{
    public partial class CreateShiftRequestModal : ComponentBase
    {
        [Inject] public IAttendanceService AttendanceService { get; set; } = default!;
        [Inject] public IEmployeeService EmployeeService { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;

        [Parameter] public string EmployeeId { get; set; } = "";
        [Parameter] public string EmployeeName { get; set; } = "";
        [Parameter] public string EmployeeCode { get; set; } = "";
        [Parameter] public string AvatarUrl { get; set; } = "";
        [Parameter] public bool IsHR { get; set; } = false;
        [Parameter] public EventCallback OnSuccess { get; set; }

        public CreateShiftRequestModel Model { get; set; } = new() { Date = DateTime.Today };
        public List<WorkShiftModel> Shifts { get; set; } = new();
        public List<EmployeeDto> AllEmployees { get; set; } = new();
        public List<EmployeeDto> Managers { get; set; } = new();
        public EmployeeDto? SelectedEmployee { get; set; }

        private bool _show = false;
        private bool _isSubmitting = false;
        private bool _showEmployeeSelection = false;
        private List<string> _initialSelectedIds = new();

        public async Task Open()
        {
            Model = new CreateShiftRequestModel 
            { 
                EmployeeId = EmployeeId, 
                Date = DateTime.Today 
            };
            
            if (Shifts.Count == 0)
                Shifts = (await AttendanceService.GetWorkShiftsAsync()).ToList();

            if (IsHR && AllEmployees.Count == 0)
            {
                AllEmployees = await EmployeeService.GetEmployeesAsync();
                Managers = AllEmployees.Where(e => 
                    (e.PositionName?.Contains("Manager") ?? false) || 
                    (e.PositionName?.Contains("Quản lý") ?? false) || 
                    (e.PositionName?.Contains("HR") ?? false) ||
                    (e.PositionName?.Contains("Trưởng phòng") ?? false)
                ).ToList();
            }

            if (IsHR && !string.IsNullOrEmpty(Model.EmployeeId) && AllEmployees.Any())
            {
                SelectedEmployee = AllEmployees.FirstOrDefault(e => e.Id == Model.EmployeeId);
            }

            _show = true;
            StateHasChanged();
        }

        public void Close()
        {
            _show = false;
            StateHasChanged();
        }

        private async Task HandleSubmit()
        {
            if (string.IsNullOrEmpty(Model.ToShiftId))
            {
                await JS.InvokeVoidAsync("toastr.warning", "Vui lòng chọn ca mục tiêu");
                return;
            }

            if (string.IsNullOrEmpty(Model.EmployeeId))
            {
                await JS.InvokeVoidAsync("toastr.warning", "Vui lòng chọn nhân viên");
                return;
            }

            _isSubmitting = true;
            StateHasChanged();
            await JS.InvokeVoidAsync("toastr.info", "Đang xử lý gửi yêu cầu...");
            try
            {
                var result = await AttendanceService.CreateShiftRequestAsync(Model);
                if (result.Success)
                {
                    await JS.InvokeVoidAsync("toastr.success", result.Message ?? "Gửi yêu cầu đổi ca thành công");
                    await OnSuccess.InvokeAsync();
                    Close();
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", result.Message ?? "Có lỗi xảy ra khi gửi yêu cầu.");
                }
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("toastr.error", $"Lỗi hệ thống: {ex.Message}");
            }
            finally
            {
                _isSubmitting = false;
            }
        }

        private bool IsHoliday(DateTime date)
        {
            // Vietnam Holidays 2026
            var holidays = new HashSet<DateTime>
            {
                new DateTime(2026, 1, 1),   // solar new year
                new DateTime(2026, 2, 16),  // lunar new year day 1
                new DateTime(2026, 2, 17),  // lunar new year day 2
                new DateTime(2026, 2, 18),  // lunar new year day 3
                new DateTime(2026, 2, 19),  // lunar new year day 4
                new DateTime(2026, 2, 20),  // lunar new year day 5
                new DateTime(2026, 2, 21),  // lunar new year day 6
                new DateTime(2026, 2, 22),  // lunar new year day 7
                new DateTime(2026, 3, 27),  // hung king (10/3 lunar)
                new DateTime(2026, 4, 30),  // victory day
                new DateTime(2026, 5, 1),   // labor day
                new DateTime(2026, 9, 2)    // national day
            };

            return holidays.Contains(date.Date);
        }
        private bool IsWeekend(DateTime date) => date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;

        private void OpenEmployeeSelection() 
        {
            _initialSelectedIds = string.IsNullOrEmpty(Model.EmployeeId) ? new List<string>() : new List<string> { Model.EmployeeId };
            _showEmployeeSelection = true;
        }

        private void OnEmployeeSelected(List<string> selectedIds)
        {
            var empId = selectedIds.FirstOrDefault();
            if (!string.IsNullOrEmpty(empId))
            {
                var emp = AllEmployees.FirstOrDefault(e => e.Id == empId);
                if (emp != null)
                {
                    SelectedEmployee = emp;
                    Model.EmployeeId = emp.Id;
                }
            }
            _showEmployeeSelection = false;
            StateHasChanged();
        }

        private string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "?";
            var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0].Length >= 2 ? parts[0].Substring(0, 2).ToUpper() : parts[0].ToUpper();
            return (parts[0].Substring(0, 1) + parts[^1].Substring(0, 1)).ToUpper();
        }

        private readonly string[] _colors = { "primary", "success", "info", "warning", "danger", "dark" };
        private string GetRandomColor(string text)
        {
            if (string.IsNullOrEmpty(text)) return "bg-light-primary text-primary";
            var hash = 0;
            foreach (var c in text) hash += c;
            var index = hash % _colors.Length;
            return $"bg-light-{_colors[index]} text-{_colors[index]}";
        }

        private async Task OpenPicker(string id)
        {
            await JS.InvokeVoidAsync("showPickerHelper", id);
        }
    }
}
