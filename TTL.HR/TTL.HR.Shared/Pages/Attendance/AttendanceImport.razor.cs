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
using TTL.HR.Application.Modules.Common.Interfaces;

namespace TTL.HR.Shared.Pages.Attendance
{
    public class AttendanceImportBase : ComponentBase
    {
        [Inject] protected IAttendanceService AttendanceService { get; set; } = default!;
        [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] protected NavigationManager Nav { get; set; } = default!;
        [Inject] protected IFormatService FormatService { get; set; } = default!;

        protected int currentStep = 1;
        protected string importSource = "Fingerprint";
        protected string rawData = "";
        protected List<List<string>> rawDataTable = new();
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
        protected bool showRawData = false;
        protected bool _isDragging = false;
        protected double _progressPercentage = 0;
        protected string _progressStatus = "";
        protected string? _currentJobId;

        protected override async Task OnInitializedAsync()
        {
            await LoadHistory();
        }

        protected async Task LoadHistory()
        {
            _isLoadingHistory = true;
            try
            {
                // Load recent 100 records regardless of date to show what was just imported
                var result = await AttendanceService.GetAttendanceListAsync(1, 100, orderBy: "Recent");
                recentlyImported = result?.Items?.ToList() ?? new List<AttendanceModel>();
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("console.error", "Error loading history:", ex.ToString());
            }
            finally
            {
                _isLoadingHistory = false;
                StateHasChanged();
            }
        }

        protected void SetSource(string source) => importSource = source;

        protected async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            if (e.File != null)
            {
                _isProcessing = true;
                StateHasChanged();

                try
                {
                    var fileName = e.File.Name.ToLower();
                    var allowedExtensions = new[] { ".xlsx", ".xls", ".csv", ".txt", ".json", ".mdb" };
                    if (!allowedExtensions.Any(ext => fileName.EndsWith(ext)))
                    {
                        await JSRuntime.InvokeVoidAsync("Swal.fire", "Lỗi", "Định dạng tệp không hỗ trợ. Vui lòng chọn .xlsx, .xls, .csv, .txt, .json hoặc .mdb", "error");
                        _isProcessing = false;
                        return;
                    }

                    // Reset previous data
                    rawData = "";
                    rawDataTable.Clear();
                    importResult = null;

                    using var stream = e.File.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    cachedFileBytes = memoryStream.ToArray();
                    cachedFileName = e.File.Name;

                    // Set selectedFile only AFTER reading is done to avoid InputFile being removed from DOM too early
                    selectedFile = e.File;

                    // Preview content in rawDataTable for ALL file types to show on Step 1
                    if (!fileName.EndsWith(".xlsx") && !fileName.EndsWith(".xls") && !fileName.EndsWith(".mdb"))
                    {
                        // Register encoding provider for code pages like windows-1258
                        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                        
                        using (var ms = new MemoryStream(cachedFileBytes))
                        using (var reader = new StreamReader(ms, System.Text.Encoding.UTF8, true))
                        {
                            rawData = await reader.ReadToEndAsync();
                        }
                        
                        ParseRawDataForTable();
                    }
                    else
                    {
                        // For Excel/MDB, fetch headers and preview data from backend
                        _progressStatus = "Đang đọc bản xem trước...";
                        var response = await AttendanceService.ImportAttendanceAsync(null, cachedFileBytes, cachedFileName, importSource, 0, 0, isPreview: true);
                        if (response.Success && response.Data != null)
                        {
                            fileHeaders = response.Data.FileHeaders;
                            rawDataTable.Clear();
                            rawDataTable.Add(fileHeaders); // Header row
                            
                            if (response.Data.RawItems != null)
                            {
                                foreach (var raw in response.Data.RawItems.Take(10))
                                {
                                    var row = new List<string>();
                                    foreach (var header in fileHeaders)
                                    {
                                        row.Add(raw.RowData != null && raw.RowData.ContainsKey(header) ? raw.RowData[header] : "");
                                    }
                                    rawDataTable.Add(row);
                                }
                            }
                        }
                    }

                    // DO NOT call ProcessNextStep() automatically anymore
                    // await ProcessNextStep();
                }
                catch (Exception ex)
                {
                    _progressPercentage = 0;
                    _progressStatus = "";
                    await JSRuntime.InvokeVoidAsync("Swal.fire", "Lỗi", "Có lỗi xảy ra khi đọc tệp: " + ex.Message, "error");
                }
                finally
                {
                    _isProcessing = false;
                    _isDragging = false;
                    StateHasChanged();
                }
            }
        }

        private async Task StartStatusPolling(string jobId)
        {
            _currentJobId = jobId;
            _ = Task.Run(async () =>
            {
                while (_currentJobId == jobId && _isProcessing)
                {
                    try
                    {
                        var status = await AttendanceService.GetExportStatusAsync(jobId);
                        if (status.Success && status.Data != null)
                        {
                            _progressPercentage = status.Data.Percentage;
                            _progressStatus = status.Data.Status ?? "Đang xử lý...";
                            StateHasChanged();

                            if (status.Data.IsCompleted || status.Data.IsFailed) break;
                        }
                    }
                    catch { }
                    await Task.Delay(1000);
                }
            });
        }

        protected void HandleDragEnter() => _isDragging = true;
        protected void HandleDragLeave() => _isDragging = false;

        protected void OnRawDataChanged(ChangeEventArgs e)
        {
            rawData = e.Value?.ToString() ?? "";
            ParseRawDataForTable();
        }

        protected void ParseRawDataForTable()
        {
            rawDataTable.Clear();
            if (string.IsNullOrWhiteSpace(rawData)) return;

            var lines = rawData.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines.Take(100))
            {
                var parts = line.Split(new[] { ',', '\t', ';' }).Select(x => x.Trim()).ToList();
                rawDataTable.Add(parts);
            }
        }

        protected void RemoveFile()
        {
            selectedFile = null;
            cachedFileBytes = null;
            cachedFileName = null;
            rawData = "";
            rawDataTable.Clear();
        }

        protected bool CanProceed() => selectedFile != null || !string.IsNullOrWhiteSpace(rawData);

        protected async Task ProcessNextStep()
        {
            if (selectedFile != null && !selectedFile.Name.ToLower().EndsWith(".json") && !selectedFile.Name.ToLower().EndsWith(".mdb"))
            {
                await GoToMapping();
            }
            else if (selectedFile == null && !string.IsNullOrWhiteSpace(rawData) && !rawData.TrimStart().StartsWith("["))
            {
                await GoToMapping();
            }
            else
            {
                await GoToPreview();
            }
        }

        protected void GoBack()
        {
            if (currentStep == 2)
            {
                currentStep = 1;
            }
            else if (currentStep == 3)
            {
                bool isMappingRequired = false;
                if (selectedFile != null && !selectedFile.Name.ToLower().EndsWith(".json") && !selectedFile.Name.ToLower().EndsWith(".mdb"))
                {
                    isMappingRequired = true;
                }
                else if (selectedFile == null && !string.IsNullOrEmpty(rawData) && !rawData.TrimStart().StartsWith("["))
                {
                    isMappingRequired = true;
                }

                if (isMappingRequired)
                {
                    currentStep = 2;
                }
                else
                {
                    currentStep = 1;
                }
            }
        }

        protected async Task GoToMapping()
        {
            _isProcessing = true;
            _progressPercentage = 0;
            _progressStatus = "Đang chuẩn bị tiêu đề...";
            StateHasChanged();
            try
            {
                _currentJobId = Guid.NewGuid().ToString();
                _ = StartStatusPolling(_currentJobId);
                var response = await AttendanceService.ImportAttendanceAsync(rawData, cachedFileBytes, cachedFileName, importSource, 0, 0, isPreview: true, jobId: _currentJobId);
                if (response.Success && response.Data != null)
                {
                    fileHeaders = response.Data.FileHeaders;
                    
                    var codeHeaders = new[] { "mã nv", "employee code", "employee_code", "manv", "ma_nv", "code" };
                    var timeHeaders = new[] { "thời gian", "timestamp", "check time", "check_time", "time", "date" };
                    for (int i = 0; i < fileHeaders.Count; i++)
                    {
                        var headerLower = fileHeaders[i].ToLower();
                        if (codeHeaders.Any(c => headerLower.Contains(c))) employeeCodeColIndex = i + 1;
                        if (timeHeaders.Any(t => headerLower.Contains(t))) timestampColIndex = i + 1;
                    }

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
            _progressPercentage = 0;
            _progressStatus = "Đang tải dữ liệu xem trước...";
            StateHasChanged();
            try
            {
                _currentJobId = Guid.NewGuid().ToString();
                _ = StartStatusPolling(_currentJobId);
                var response = await AttendanceService.ImportAttendanceAsync(rawData, cachedFileBytes, cachedFileName, importSource, employeeCodeColIndex, timestampColIndex, isPreview: true, jobId: _currentJobId);
                if (response.Success && response.Data != null)
                {
                    importResult = response.Data;
                    showRawData = true; // Auto-expand raw data for verification
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
                _ = JSRuntime.InvokeVoidAsync("Swal.fire", new { title = "Đang lưu dữ liệu", text = "Vui lòng chờ...", allowOutsideClick = false, showConfirmButton = false });
                try { _ = JSRuntime.InvokeVoidAsync("Swal.showLoading"); } catch { }

                _currentJobId = Guid.NewGuid().ToString();
                _progressPercentage = 0;
                _progressStatus = "Đang lưu...";
                _ = StartStatusPolling(_currentJobId);
                var response = await AttendanceService.ImportAttendanceAsync(rawData, cachedFileBytes, cachedFileName, importSource, employeeCodeColIndex, timestampColIndex, isPreview: false, jobId: _currentJobId);
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

        protected void ToggleRawData() => showRawData = !showRawData;
    }
}
