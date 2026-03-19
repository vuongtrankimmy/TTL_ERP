using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using TTL.HR.Application.Modules.HumanResource.Models;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.Organization.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TTL.HR.Shared.Pages.Employees.Components
{
    public partial class EmployeeStepDocuments : ComponentBase
    {
        [Parameter] public EmployeeModel Model { get; set; } = new();
        [Parameter] public bool IsProcessing { get; set; }
        [Parameter] public bool IsReadOnly { get; set; }
        [Parameter] public List<EmployeeDocumentModel> UploadedDocuments { get; set; } = new();

        [Parameter] public EventCallback OnSave { get; set; }
        [Parameter] public EventCallback<(InputFileChangeEventArgs Args, string DocType)> OnDocumentUpload { get; set; }

        private async Task HandleSave()
        {
            if (OnSave.HasDelegate)
            {
                await OnSave.InvokeAsync();
            }
        }

        private async Task HandleDocumentUpload(InputFileChangeEventArgs e, string docType)
        {
            if (OnDocumentUpload.HasDelegate)
            {
                await OnDocumentUpload.InvokeAsync((e, docType));
            }
        }
    }
}
