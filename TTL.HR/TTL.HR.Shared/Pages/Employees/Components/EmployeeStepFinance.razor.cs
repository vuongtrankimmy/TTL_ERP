using Microsoft.AspNetCore.Components;
using TTL.HR.Application.Modules.HumanResource.Models;
using TTL.HR.Application.Modules.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TTL.HR.Shared.Pages.Employees.Components
{
    public partial class EmployeeStepFinance : ComponentBase
    {
        [Parameter] public EmployeeModel Model { get; set; } = new();
        [Parameter] public bool IsProcessing { get; set; }
        [Parameter] public bool IsReadOnly { get; set; }
        [Parameter] public bool IsEditMode { get; set; }
        [Parameter] public Dictionary<string, string> Errors { get; set; } = new();

        [Parameter] public List<RoleModel> AvailableRoles { get; set; } = new();

        [Parameter] public EventCallback OnSave { get; set; }
        [Parameter] public EventCallback<string> OnToggleRole { get; set; }
        [Parameter] public EventCallback OnPasswordResetRequested { get; set; }

        private async Task HandleSave()
        {
            if (OnSave.HasDelegate)
            {
                await OnSave.InvokeAsync();
            }
        }
        private async Task HandleToggleRole(string roleId)
        {
            if (OnToggleRole.HasDelegate)
            {
                await OnToggleRole.InvokeAsync(roleId);
            }
        }

        private async Task HandlePasswordReset()
        {
            if (OnPasswordResetRequested.HasDelegate)
            {
                await OnPasswordResetRequested.InvokeAsync();
            }
        }
    }
}
