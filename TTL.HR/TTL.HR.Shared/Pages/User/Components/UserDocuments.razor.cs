using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Models;

namespace TTL.HR.Shared.Pages.User.Components
{
    public partial class UserDocuments
    {
        [Parameter] public string? EmployeeId { get; set; }
        [Inject] private IEmployeeService EmployeeService { get; set; } = default!;
        [Inject] private IMasterDataService MasterDataService { get; set; } = default!;

        private DigitalProfileModel? _profile;
        private List<TTL.HR.Application.Modules.Common.Models.LookupModel> _documentTypeLookups = new();
        private bool _isLoading = true;
        private bool _isUploading = false;
        private bool _showUploadModal = false;
        private bool _showViewModal = false;
        private EmployeeDocumentModel? _viewingDocument;

        // Upload form state
        private string _selectedDocumentType = "CCCD";
        private string _selectedFileName = "";
        private IBrowserFile? _selectedFile;
        private DateTime? _expiryDate;
        private string _note = "";
        private string? _uploadError;
        private bool _uploadSuccess = false;

        protected override async Task OnInitializedAsync()
        {
            await LoadLookups();
        }

        private async Task LoadLookups()
        {
            try 
            {
                _documentTypeLookups = await MasterDataService.GetCachedLookupsAsync("DocumentType");
                if (_documentTypeLookups.Any())
                {
                    _selectedDocumentType = _documentTypeLookups.First().Code;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserDocuments] LoadLookups error: {ex.Message}");
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrEmpty(EmployeeId))
            {
                await LoadData();
            }
        }

        private async Task LoadData()
        {
            if (string.IsNullOrEmpty(EmployeeId)) return;
            _isLoading = true;
            StateHasChanged();
            try
            {
                _profile = await EmployeeService.GetDigitalProfileAsync(EmployeeId);

                // If null (API error or no profile yet), show empty state
                _profile ??= new DigitalProfileModel
                {
                    EmployeeId = EmployeeId,
                    Documents = new List<EmployeeDocumentModel>()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserDocuments] LoadData error: {ex.Message}");
                _profile = new DigitalProfileModel
                {
                    EmployeeId = EmployeeId,
                    Documents = new List<EmployeeDocumentModel>()
                };
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        private void OpenUploadModal()
        {
            _selectedDocumentType = _documentTypeLookups.FirstOrDefault()?.Code ?? "CCCD";
            _selectedFile = null;
            _selectedFileName = "";
            _expiryDate = null;
            _note = "";
            _uploadError = null;
            _uploadSuccess = false;
            _showUploadModal = true;
        }

        private void CloseUploadModal()
        {
            _showUploadModal = false;
            _uploadError = null;
        }

        private void OpenViewModal(EmployeeDocumentModel doc)
        {
            _viewingDocument = doc;
            _showViewModal = true;
        }

        private void CloseViewModal()
        {
            _showViewModal = false;
            _viewingDocument = null;
        }

        private void OnFileSelected(InputFileChangeEventArgs e)
        {
            _selectedFile = e.File;
            _selectedFileName = e.File.Name;
            _uploadError = null;
        }

        private async Task HandleUpload()
        {
            if (_selectedFile == null)
            {
                _uploadError = "Vui lòng chọn tệp tài liệu.";
                return;
            }
            if (string.IsNullOrEmpty(EmployeeId))
            {
                _uploadError = "Không xác định được nhân viên. Vui lòng thử lại.";
                return;
            }

            // Validate file size (max 10MB)
            if (_selectedFile.Size > 10 * 1024 * 1024)
            {
                _uploadError = "Tệp quá lớn. Tối đa 10MB.";
                return;
            }

            // Validate file type
            var ext = Path.GetExtension(_selectedFile.Name).ToLower();
            if (ext != ".pdf" && ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".doc" && ext != ".docx")
            {
                _uploadError = "Chỉ hỗ trợ: PDF, JPG, PNG, DOC, DOCX.";
                return;
            }

            _isUploading = true;
            _uploadError = null;
            StateHasChanged();

            try
            {
                await using var stream = _selectedFile.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
                // Now: null = success, non-null string = error message
                var errorMsg = await EmployeeService.UploadDocumentAsync(
                    EmployeeId,
                    _selectedDocumentType,
                    stream,
                    _selectedFile.Name,
                    _expiryDate,
                    _note);

                if (errorMsg == null)
                {
                    // SUCCESS
                    _uploadSuccess = true;
                    StateHasChanged();
                    await Task.Delay(500); // brief delay to ensure DB commit
                    await LoadData(); // Refresh list
                    await Task.Delay(1500); // show success message before closing
                    CloseUploadModal();
                }
                else
                {
                    _uploadError = errorMsg;
                }
            }
            catch (Exception ex)
            {
                _uploadError = $"Lỗi: {ex.Message}";
            }
            finally
            {
                _isUploading = false;
                StateHasChanged();
            }
        }

        private static string GetFileIcon(string fileType) => fileType.ToLower() switch
        {
            "pdf" => "assets/media/svg/files/pdf.svg",
            "jpg" or "jpeg" or "png" or "gif" => "assets/media/svg/files/jpg.svg",
            "doc" or "docx" => "assets/media/svg/files/doc.svg",
            "xls" or "xlsx" => "assets/media/svg/files/xls.svg",
            _ => "assets/media/svg/files/pdf.svg"
        };

        private bool IsImage(string fileType) =>
            fileType.ToLower() == "jpg" || fileType.ToLower() == "jpeg" || fileType.ToLower() == "png";

        private int CompletionPercent => _profile == null ? 0 : (int)_profile.CompletionPercentage;
    }
}
