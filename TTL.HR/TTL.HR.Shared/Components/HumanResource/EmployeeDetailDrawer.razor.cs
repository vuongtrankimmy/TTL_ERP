using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Attendance.Models;
using TTL.HR.Application.Modules.HumanResource.Models;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.Attendance.Interfaces;
using TTL.HR.Application.Modules.Organization.Models;

namespace TTL.HR.Shared.Components.HumanResource;

public partial class EmployeeDetailDrawer
{
    [Inject] private IAttendanceService AttendanceService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    [Parameter] public EmployeeModel? Employee { get; set; }
    [Parameter] public bool IsLoading { get; set; }
    [Parameter] public bool IsReadOnly { get; set; } = false;
    [Parameter] public List<DepartmentModel> Departments { get; set; } = new();
    [Parameter] public List<PositionModel> Positions { get; set; } = new();
    [Parameter] public List<EmployeeDto> Managers { get; set; } = new();
    [Parameter] public List<LookupModel> Roles { get; set; } = new();
    [Parameter] public List<LookupModel> Workplaces { get; set; } = new();
    [Parameter] public EventCallback<EmployeeModel> OnSave { get; set; }
    [Parameter] public EventCallback OnScanCCCD { get; set; }
    [Parameter] public EventCallback<EmployeeModel> OnViewContract { get; set; }
    [Parameter] public EventCallback<EmployeeModel> OnPrintContract { get; set; }

    private string activeTab = "profile";
    private string permissionTab = "role";
    private bool _isPasswordVisible = false;
    private bool _isPrintLoading = false;
    
    // Attendance State
    private int _calendarMonth = DateTime.Now.Month;
    private int _calendarYear = DateTime.Now.Year;
    private List<AttendanceDetailModel> _attendanceDetails = new();
    private EmployeeStatsModel? _stats;
    private bool _isAttendanceLoading = false;

    private async Task SetActiveTab(string tab)
    {
        activeTab = tab;
        if (tab == "calendar" && !_attendanceDetails.Any())
        {
            await LoadAttendanceAsync();
        }
    }

    private async Task LoadAttendanceAsync()
    {
        if (Employee == null) return;
        _isAttendanceLoading = true;
        try
        {
            var date = new DateTime(_calendarYear, _calendarMonth, 1);
            var details = await AttendanceService.GetAttendanceDetailsAsync(Employee.Id, date);
            _attendanceDetails = details?.ToList() ?? new();
            
            _stats = await AttendanceService.GetEmployeeStatsAsync(Employee.Id, _calendarMonth, _calendarYear);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading attendance in drawer: {ex.Message}");
        }
        finally
        {
            _isAttendanceLoading = false;
            StateHasChanged();
        }
    }

    private async Task PrevMonth()
    {
        if (_calendarMonth == 1) { _calendarMonth = 12; _calendarYear--; }
        else { _calendarMonth--; }
        await LoadAttendanceAsync();
    }

    private async Task NextMonth()
    {
        if (_calendarMonth == 12) { _calendarMonth = 1; _calendarYear++; }
        else { _calendarMonth++; }
        await LoadAttendanceAsync();
    }

    private List<DateTime> GetLeadingBlankDates()
    {
        var firstDay = new DateTime(_calendarYear, _calendarMonth, 1);
        int dayOfWeek = (int)firstDay.DayOfWeek; // 0=Sunday, 1=Monday...
        
        // Sunday-first grid: Sunday=0, Monday=1...
        int blanks = dayOfWeek;
        
        var prevMonth = firstDay.AddMonths(-1);
        int daysInMonthInPrevMonth = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);
        
        return Enumerable.Range(daysInMonthInPrevMonth - blanks + 1, blanks)
                         .Select(d => new DateTime(prevMonth.Year, prevMonth.Month, d))
                         .ToList();
    }

    private List<DateTime> GetTrailingBlankDates()
    {
        var firstDay = new DateTime(_calendarYear, _calendarMonth, 1);
        int daysInMonth = DateTime.DaysInMonth(_calendarYear, _calendarMonth);
        var lastDay = new DateTime(_calendarYear, _calendarMonth, daysInMonth);
        int lastDayOfWeek = (int)lastDay.DayOfWeek; // 0=Sunday...
        
        // Sunday-first grid: Saturday=6
        int blanks = 6 - lastDayOfWeek;
        
        var nextMonth = firstDay.AddMonths(1);
        
        return Enumerable.Range(1, blanks)
                         .Select(d => new DateTime(nextMonth.Year, nextMonth.Month, d))
                         .ToList();
    }

    private async Task HandlePrintContract()
    {
        _isPrintLoading = true;
        try
        {
            await OnPrintContract.InvokeAsync(Employee);
        }
        finally
        {
            _isPrintLoading = false;
        }
    }

    private async Task HandleViewContract()
    {
        await OnViewContract.InvokeAsync(Employee);
    }

    private void SelectAllPermissions()
    {
        if (Employee?.ModulePermissions == null) return;
        foreach (var perm in Employee.ModulePermissions)
        {
            perm.CanView = true;
            perm.CanAdd = true;
            perm.CanEdit = true;
            perm.CanDelete = true;
        }
        StateHasChanged();
    }
}
