using Microsoft.AspNetCore.Components;
using TTL.HR.Application.Modules.HumanResource.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TTL.HR.Shared.Pages.User.Components;

public partial class UserProfessional
{
    [Inject] public IEmployeeService EmployeeService { get; set; } = default!;

    [Parameter] public string? EmployeeId { get; set; }

    private EmployeeModel? _employee;
    private bool _isLoading = true;

    protected override async Task OnParametersSetAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        if (string.IsNullOrEmpty(EmployeeId)) return;

        _isLoading = true;
        try
        {
            _employee = await EmployeeService.GetEmployeeAsync(EmployeeId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UserProfessional] Load error: {ex.Message}");
        }
        finally
        {
            _isLoading = false;
        }
    }
}
