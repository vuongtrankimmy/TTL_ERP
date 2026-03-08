using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace TTL.HR.Shared.Pages.Settings;

public partial class BankList
{
    [Inject] private IBankService BankService { get; set; } = default!;
    [Inject] private IFileService FileService { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;

    private bool _isLoading = true;
    private bool _showEditDrawer = false;
    private bool _isNewBank = true;
    private string _searchTerm = "";
    private List<BankDto> _banks = new();
    private BankDto _editingBank = new();

    // UI State
    private string _viewMode = "table"; // "table" or "card"
    private string _filterStatus = ""; // "" (All), "Active", "Inactive"
    private bool _loadFailed = false;
    private string _errorMessage = "";
    private string _sortBy = "priority";
    private bool _sortAscending = false;

    private struct BankCounts
    {
        public int All;
        public int Active;
        public int Inactive;
    }
    private BankCounts _counts;

    protected override async Task OnInitializedAsync()
    {
        await LoadBanks();
    }

    private async Task LoadBanks()
    {
        _isLoading = true;
        _loadFailed = false;
        try
        {
            var result = await BankService.GetBanksAsync(new GetBanksRequest
            {
                Page = 1,
                PageSize = 1000,
                SearchTerm = _searchTerm
            });
            _banks = result?.Items ?? new List<BankDto>();
            UpdateCounts();
        }
        catch (Exception ex)
        {
            _loadFailed = true;
            _errorMessage = ex.Message;
            Console.WriteLine($"Error loading banks: {ex.Message}");
        }
        finally
        {
            _isLoading = false;
            StateHasChanged();
        }
    }

    private void UpdateCounts()
    {
        _counts.All = _banks.Count;
        _counts.Active = _banks.Count(x => x.IsActive);
        _counts.Inactive = _banks.Count(x => !x.IsActive);
    }

    private IEnumerable<BankDto> FilteredBanks
    {
        get
        {
            var query = _banks.AsEnumerable();

            if (!string.IsNullOrEmpty(_filterStatus))
            {
                bool isActive = _filterStatus == "Active";
                query = query.Where(x => x.IsActive == isActive);
            }

            // Apply Sorting
            query = _sortBy switch
            {
                "name" => _sortAscending ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name),
                "code" => _sortAscending ? query.OrderBy(x => x.Code) : query.OrderByDescending(x => x.Code),
                "priority" => _sortAscending ? query.OrderBy(x => x.Priority) : query.OrderByDescending(x => x.Priority),
                _ => query.OrderByDescending(x => x.Priority)
            };

            return query;
        }
    }

    private async Task HandleSearch(ChangeEventArgs e)
    {
        _searchTerm = e.Value?.ToString() ?? "";
        await LoadBanks();
    }

    private void ToggleViewMode(string mode)
    {
        _viewMode = mode;
    }

    private void ChangeStatusFilter(string status)
    {
        _filterStatus = status;
    }

    private void ToggleSort(string column)
    {
        if (_sortBy == column)
        {
            _sortAscending = !_sortAscending;
        }
        else
        {
            _sortBy = column;
            _sortAscending = true;
        }
    }

    private async Task ReloadData()
    {
        await LoadBanks();
    }

    private void OpenAddBank()
    {
        _isNewBank = true;
        _editingBank = new BankDto { IsActive = true, Priority = 0 };
        _showEditDrawer = true;
    }

    private void EditBank(BankDto bank)
    {
        _isNewBank = false;
        _editingBank = new BankDto
        {
            Id = bank.Id,
            Name = bank.Name,
            Code = bank.Code,
            Logo = bank.Logo,
            IsActive = bank.IsActive,
            Priority = bank.Priority,
            SwiftCode = bank.SwiftCode,
            Note = bank.Note
        };
        _showEditDrawer = true;
    }

    private async Task SaveBank()
    {
        _isLoading = true;
        try
        {
            bool success;
            if (_isNewBank)
            {
                var request = new CreateBankRequest
                {
                    Name = _editingBank.Name,
                    Code = _editingBank.Code,
                    Logo = _editingBank.Logo,
                    IsActive = _editingBank.IsActive,
                    Priority = _editingBank.Priority,
                    SwiftCode = _editingBank.SwiftCode,
                    Note = _editingBank.Note
                };
                var result = await BankService.CreateBankAsync(request);
                success = result != null;
            }
            else
            {
                var request = new UpdateBankRequest
                {
                    Id = _editingBank.Id,
                    Name = _editingBank.Name,
                    Code = _editingBank.Code,
                    Logo = _editingBank.Logo,
                    IsActive = _editingBank.IsActive,
                    Priority = _editingBank.Priority,
                    SwiftCode = _editingBank.SwiftCode,
                    Note = _editingBank.Note
                };
                success = await BankService.UpdateBankAsync(request);
            }

            if (success)
            {
                await JS.InvokeVoidAsync("toastr.success", "Lưu thông tin ngân hàng thành công");
                _showEditDrawer = false;
                await LoadBanks();
            }
            else
            {
                await JS.InvokeVoidAsync("toastr.error", "Có lỗi xảy ra khi lưu thông tin");
            }
        }
        catch (Exception ex)
        {
            await JS.InvokeVoidAsync("toastr.error", "Lỗi: " + ex.Message);
        }
        finally
        {
            _isLoading = false;
            StateHasChanged();
        }
    }

    private async Task ToggleStatus(BankDto bank)
    {
        var request = new UpdateBankRequest
        {
            Id = bank.Id,
            Name = bank.Name,
            Code = bank.Code,
            Logo = bank.Logo,
            IsActive = !bank.IsActive,
            Priority = bank.Priority,
            SwiftCode = bank.SwiftCode,
            Note = bank.Note
        };
        
        if (await BankService.UpdateBankAsync(request))
        {
            bank.IsActive = !bank.IsActive;
            UpdateCounts();
            await JS.InvokeVoidAsync("toastr.success", $"Đã {(bank.IsActive ? "mở" : "tắt")} ngân hàng {bank.Code}");
        }
    }

    private async Task HandleLogoUpload(InputFileChangeEventArgs e)
    {
        var file = e.File;
        if (file == null) return;

        // Validation
        if (file.Size > 2 * 1024 * 1024) // 2MB
        {
            await JS.InvokeVoidAsync("toastr.error", "Ảnh quá lớn. Vui lòng chọn ảnh dưới 2MB.");
            return;
        }

        try
        {
            _isLoading = true;
            StateHasChanged();

            var url = await FileService.UploadFileAsync(file, "banks");
            if (!string.IsNullOrEmpty(url))
            {
                _editingBank.Logo = url;
                await JS.InvokeVoidAsync("toastr.success", "Tải logo lên thành công");
            }
            else
            {
                await JS.InvokeVoidAsync("toastr.error", "Tải logo lên thất bại");
            }
        }
        catch (Exception ex)
        {
            await JS.InvokeVoidAsync("toastr.error", "Lỗi: " + ex.Message);
        }
        finally
        {
            _isLoading = false;
            StateHasChanged();
        }
    }
}
