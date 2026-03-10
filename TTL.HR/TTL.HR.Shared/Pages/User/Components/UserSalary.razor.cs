using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Payroll.Interfaces;
using TTL.HR.Application.Modules.Payroll.Models;

namespace TTL.HR.Shared.Pages.User.Components;

public partial class UserSalary
{
    [Inject] public IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] public IPayrollService PayrollService { get; set; } = default!;

    [Parameter] public string? EmployeeId { get; set; }

    private List<PayrollModel> _salaries = new();
    private bool _isLoading = true;
    private bool _isDrawerOpen = false;
    private PayrollModel? _selectedPayroll;

    protected override async Task OnParametersSetAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        _isLoading = true;
        try
        {
            var result = await PayrollService.GetMyPayrollsAsync(EmployeeId);
            _salaries = result.Items.ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UserSalary] Load error: {ex.Message}");
        }
        finally
        {
            _isLoading = false;
        }
    }

    private void OpenDetail(PayrollModel item)
    {
        _selectedPayroll = item;
        _isDrawerOpen = true;
    }

    private void CloseDrawer()
    {
        _isDrawerOpen = false;
        StateHasChanged();
    }

    private async Task PrintPayslip()
    {
        await JSRuntime.InvokeVoidAsync("window.print");
    }

    private string GetStatusColor(PayrollModel item) => !string.IsNullOrEmpty(item.StatusColor) ? item.StatusColor : "info";
    private string GetStatusText(PayrollModel item) => $"text-{GetStatusColor(item)}";
    private string GetStatusBgCard(PayrollModel item)
    {
        var color = GetStatusColor(item);
        if (color == "secondary" || color == "info" || color == "warning") return "danger";
        return color;
    }
}
