using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using TTL.HR.Application.Modules.HumanResource.Models;
using TTL.HR.Application.Modules.Organization.Models;
using TTL.HR.Application.Modules.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TTL.HR.Shared.Pages.Employees.Components
{
    public partial class EmployeeStepBasic : ComponentBase
    {
        [Parameter] public EmployeeModel Model { get; set; } = new();
        [Parameter] public bool IsProcessing { get; set; }
        [Parameter] public bool IsReadOnly { get; set; }
        [Parameter] public Dictionary<string, string> Errors { get; set; } = new();

        [Parameter] public List<DepartmentModel> Departments { get; set; } = new();
        [Parameter] public List<PositionModel> Positions { get; set; } = new();
        [Parameter] public List<LookupModel> WorkplaceLookups { get; set; } = new();
        [Parameter] public List<LookupModel> StatusLookups { get; set; } = new();
        [Parameter] public List<LookupModel> ContractTypeLookups { get; set; } = new();

        [Parameter] public EventCallback OnSave { get; set; }
        [Parameter] public EventCallback<InputFileChangeEventArgs> OnAvatarUpload { get; set; }
        [Parameter] public EventCallback OnScanRequested { get; set; }

        private async Task HandleSave()
        {
            if (OnSave.HasDelegate)
            {
                await OnSave.InvokeAsync();
            }
        }

        private async Task HandleAvatarUpload(InputFileChangeEventArgs e)
        {
            if (OnAvatarUpload.HasDelegate)
            {
                await OnAvatarUpload.InvokeAsync(e);
            }
        }

        private async Task HandleScan()
        {
            if (OnScanRequested.HasDelegate)
            {
                await OnScanRequested.InvokeAsync();
            }
        }
    }
}
