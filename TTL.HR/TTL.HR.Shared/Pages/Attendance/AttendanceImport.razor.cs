using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Attendance.Interfaces;
using TTL.HR.Application.Modules.Attendance.Models;

namespace TTL.HR.Shared.Pages.Attendance
{
    public class AttendanceImportBase : ComponentBase
    {
        [Inject] protected IAttendanceService AttendanceService { get; set; } = default!;
        [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] protected NavigationManager Nav { get; set; } = default!;

        protected int currentStep = 1;
        protected string importSource = "Fingerprint";
        protected string rawData = "";
        protected IBrowserFile? selectedFile;
        protected byte[]? cachedFileBytes;
        protected string? cachedFileName;
        protected bool _isProcessing = false;
        protected ImportAttendanceResultModel? importResult;
        protected List<string> fileHeaders = new();
        protected int employeeCodeColIndex = 1;
        protected int timestampColIndex = 2;
        protected List<AttendanceModel> recentlyImported = new();
        protected bool _isLoadingHistory = false;

        protected override async Task OnInitializedAsync()
        {
            await LoadHistory();
        }

        protected async Task LoadHistory()
        {
            _isLoadingHistory = true;
            try
            {
                var result = await AttendanceService.GetAttendanceListAsync(1, 10, date: DateTime.Today);
                recentlyImported = result.Items.ToList();
            }
            finally
            {
                _isLoadingHistory = false;
            }
        }

        protected void SetSource(string source) => importSource = source;

        protected async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            selectedFile = e.File;
            if (selectedFile != null)
            {
                var fileName = selectedFile.Name.ToLower();
                var allowedExtensions = new[] { ".xlsx", ".xls", ".csv", ".txt", ".json" };
                if (!allowedExtensions.Any(ext => fileName.EndsWith(ext)))
                {
                    await JSRuntime.InvokeVoidAsync("Swal.fire", "Lỗi", "Định dạng tệp không hỗ trợ. Vui lòng chọn .xlsx, .xls, .csv, .txt hoặc .json", "error");
                    selectedFile = null;
                    return;
                }

                using var stream = selectedFile.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                cachedFileBytes = memoryStream.ToArray();
                cachedFileName = selectedFile.Name;

                // Decide next step
                if (fileName.EndsWith(".xlsx") || fileName.EndsWith(".xls"))
                {
                    await GoToMapping();
                }
                else
                {
                    await GoToPreview();
                }
            }
        }

        protected async Task GoToMapping()
        {
            _isProcessing = true;
            try
            {
                var response = await AttendanceService.ImportAttendanceAsync(null, cachedFileBytes, cachedFileName, importSource, isPreview: true);
                if (response.Success && response.Data != null)
                {
                    fileHeaders = response.Data.FileHeaders;
                    currentStep = 2;
                }
                else
                {
                    await JSRuntime.InvokeVoidAsync("Swal.fire", "Lỗi", response.Message ?? "Không thể đọc tiêu đề file", "error");
                }
            }
            finally
            {
                _isProcessing = false;
            }
        }

        protected async Task GoToPreview()
        {
            _isProcessing = true;
            try
            {
                var response = await AttendanceService.ImportAttendanceAsync(rawData, cachedFileBytes, cachedFileName, importSource, employeeCodeColIndex, timestampColIndex, isPreview: true);
                if (response.Success && response.Data != null)
                {
                    importResult = response.Data;
                    currentStep = 3;
                }
                else
                {
                    await JSRuntime.InvokeVoidAsync("Swal.fire", "Thất bại", response.Message, "error");
                }
            }
            finally
            {
                _isProcessing = false;
            }
        }

        protected async Task ConfirmImport()
        {
            _isProcessing = true;
            try
            {
                await JSRuntime.InvokeVoidAsync("Swal.fire", new { title = "Đang lưu dữ liệu", text = "Vui lòng chờ...", allowOutsideClick = false, showConfirmButton = false });
                await JSRuntime.InvokeVoidAsync("Swal.showLoading");

                var response = await AttendanceService.ImportAttendanceAsync(rawData, cachedFileBytes, cachedFileName, importSource, employeeCodeColIndex, timestampColIndex, isPreview: false);
                await JSRuntime.InvokeVoidAsync("Swal.close");

                if (response.Success)
                {
                    importResult = response.Data;
                    currentStep = 4;
                    await LoadHistory();
                    await JSRuntime.InvokeVoidAsync("Swal.fire", "Thành công", response.Message, "success");
                }
                else
                {
                    await JSRuntime.InvokeVoidAsync("Swal.fire", "Thất bại", response.Message, "error");
                }
            }
            finally
            {
                _isProcessing = false;
            }
        }

        protected void Reset()
        {
            currentStep = 1;
            selectedFile = null;
            cachedFileBytes = null;
            rawData = "";
            importResult = null;
            fileHeaders.Clear();
        }
    }
}
