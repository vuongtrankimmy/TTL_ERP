using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.HumanResource.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Models;

namespace TTL.HR.Shared.Pages.Employees;

public partial class ImportWizard
{
    [Inject] public IEmployeeImportService ImportService { get; set; } = default!;
    [Inject] public IJSRuntime JSRuntime { get; set; } = default!;

    [Parameter] public EventCallback OnImportCompleted { get; set; }

    private int step = 1;
    private IBrowserFile? selectedFile;
    private bool isImporting = false;
    private ImportResultModel? importResult;

    private async Task DownloadTemplate()
    {
        var content = await ImportService.ExportEmployeeTemplateAsync();
        var fileName = "Employee_Import_Template.xlsx";
        await JSRuntime.InvokeVoidAsync("downloadFile", content, fileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    private void HandleFileSelected(InputFileChangeEventArgs e)
    {
        selectedFile = e.File;
    }

    private async Task ProcessImport()
    {
        if (selectedFile == null) return;

        isImporting = true;
        try
        {
            using var stream = selectedFile.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024);
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);

            importResult = await ImportService.ImportEmployeesFromExcelAsync(ms.ToArray());
            step = 3;
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("Swal.fire", "Lỗi", "Không thể xử lý file: " + ex.Message, "error");
        }
        finally
        {
            isImporting = false;
        }
    }

    private async Task Finish()
    {
        await OnImportCompleted.InvokeAsync();
        step = 1;
        selectedFile = null;
        importResult = null;
    }
}
