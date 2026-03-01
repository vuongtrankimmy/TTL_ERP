using Microsoft.AspNetCore.Components;
using TTL.HR.Application.Modules.HumanResource.Models;
using System.Threading.Tasks;

namespace TTL.HR.Shared.Components.HumanResource
{
    public partial class EmployeeDetailDrawer
    {
        [Parameter] public EmployeeModel? Employee { get; set; }
        [Parameter] public bool Show { get; set; }
        [Parameter] public EventCallback OnClose { get; set; }
        [Parameter] public EventCallback<EmployeeModel> OnEditEmployee { get; set; }

        private string activeTab = "profile";

        private async Task Close()
        {
            await OnClose.InvokeAsync();
        }

        private async Task OnEdit()
        {
            if (Employee != null)
            {
                await OnEditEmployee.InvokeAsync(Employee);
            }
        }
    }
}
