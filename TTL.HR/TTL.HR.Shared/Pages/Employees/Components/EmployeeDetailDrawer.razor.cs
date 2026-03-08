using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.HumanResource.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Models;
using TTL.HR.Application.Modules.Organization.Models;

namespace TTL.HR.Shared.Pages.Employees.Components;

public partial class EmployeeDetailDrawer
{
    [Inject] public IEmployeeService EmployeeService { get; set; } = default!;
    [Inject] public IContractService ContractService { get; set; } = default!;
    [Inject] public IJSRuntime JSRuntime { get; set; } = default!;

    [Parameter] public EmployeeModel? Employee { get; set; }
    [Parameter] public bool IsLoading { get; set; }
    [Parameter] public bool IsReadOnly { get; set; } = false;
    [Parameter] public List<DepartmentModel> Departments { get; set; } = new();
    [Parameter] public List<PositionModel> Positions { get; set; } = new();
    [Parameter] public List<EmployeeDto> Managers { get; set; } = new();
    [Parameter] public EventCallback<EmployeeModel> OnSave { get; set; }
    [Parameter] public EventCallback OnScanCCCD { get; set; }
    [Parameter] public EventCallback<EmployeeModel> OnViewContract { get; set; }
    [Parameter] public EventCallback<EmployeeModel> OnPrintContract { get; set; }

    private string activeTab = "profile";
    private bool _isPasswordVisible = false;
    private bool _isPrintLoading = false;

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
}
