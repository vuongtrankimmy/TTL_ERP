using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.HumanResource.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Models;
using TTL.HR.Application.Modules.Payroll.Interfaces;
using TTL.HR.Application.Modules.Payroll.Models;

namespace TTL.HR.Shared.Pages.Benefits
{
    public partial class BenefitsList
    {
        [Inject] public IBenefitService BenefitService { get; set; } = default!;
        [Inject] public IEmployeeService EmployeeService { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;

        private bool _showDetail = false;
        private bool _isLoading = true;
        private BenefitModel? _selectedBenefit;
        private List<BenefitModel> _benefits = new();
        private string _searchTerm = "";
        private string _selectedCategory = "";

        // Allocations
        private bool _showAssignModal = false;
        private bool _isAssignLoading = false;
        private bool _isAllocationLoading = false;
        private BenefitModel? _benefitToAssign;
        private List<EmployeeDto> _allEmployees = new();
        private List<string> _initiallyAssignedIds = new();
        private List<string> _currentAssignedIds = new();
        
        private string _employeeSearchTerm = "";
        private BenefitAssignRequest _assignRequest = new();
        private int _totalAllocations = 0;
        private string _allocationTab = "unassigned"; // unassigned, assigned

        private BenefitEditModal _editModal = default!;
        private bool IsDeleteModalOpen = false;
        private BenefitModel? BenefitToDelete;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
            try
            {
                _allEmployees = await EmployeeService.GetEmployeesAsync() ?? new();
            }
            catch { }
        }

        private async Task LoadData()
        {
            _isLoading = true;
            try
            {
                var data = await BenefitService.GetBenefitsAsync();
                _benefits = data?.ToList() ?? new List<BenefitModel>();
            }
            catch (Exception)
            {
                try { await JS.InvokeVoidAsync("toastr.error", "L\u1ed7i t\u1ea3i d\u1eef li\u1ec7u ph\u00fac l\u1ee3i."); } catch { }
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        private async Task CreateNewBenefit() => await _editModal.OpenAsync();
        private async Task EditBenefit(BenefitModel item) => await _editModal.OpenAsync(item.Id);

        private void PromptDeleteBenefit(BenefitModel item)
        {
            BenefitToDelete = item;
            IsDeleteModalOpen = true;
        }

        private void CloseDeleteModal()
        {
            IsDeleteModalOpen = false;
            BenefitToDelete = null;
        }

        private async Task ConfirmDelete()
        {
            if (BenefitToDelete != null && !string.IsNullOrEmpty(BenefitToDelete.Id))
            {
                var success = await BenefitService.DeleteBenefitAsync(BenefitToDelete.Id);
                if (success)
                {
                    try { await JS.InvokeVoidAsync("toastr.success", $"Đã xóa phúc lợi {BenefitToDelete.Name}."); } catch { }
                    await LoadData();
                }
                else
                {
                    try { await JS.InvokeVoidAsync("toastr.error", "Xóa phúc lợi thất bại."); } catch { }
                }
                CloseDeleteModal();
            }
        }

        private void openDetail(BenefitModel item)
        {
            _selectedBenefit = item;
            _showDetail = true;
        }

        private void closeDetail() => _showDetail = false;

        // ── ASSIGN MODAL ──────────────────────────────────────────────────────────
        private async Task OpenAssignModal(BenefitModel benefit)
        {
            _benefitToAssign = benefit;
            _showAssignModal = true;
            _employeeSearchTerm = "";
            _allocationTab = "unassigned";
            _assignRequest = new BenefitAssignRequest
            {
                BenefitId = benefit.Id,
                StartDate = DateTime.Today
            };
            await LoadAllocationsForBenefit(benefit.Id);
        }

        private void CloseAssignModal()
        {
            _showAssignModal = false;
            _benefitToAssign = null;
            _initiallyAssignedIds = new();
            _currentAssignedIds = new();
        }

        private async Task LoadAllocationsForBenefit(string benefitId)
        {
            _isAllocationLoading = true;
            try
            {
                var allocations = await BenefitService.GetBenefitAllocationsAsync(benefitId);
                _initiallyAssignedIds = allocations.Select(a => a.EmployeeId).ToList();
                _currentAssignedIds = new List<string>(_initiallyAssignedIds);
                _totalAllocations = _currentAssignedIds.Count;
            }
            catch { }
            _isAllocationLoading = false;
        }

        private void ToggleEmployeeAssignment(string employeeId)
        {
            if (_currentAssignedIds.Contains(employeeId))
                _currentAssignedIds.Remove(employeeId);
            else
                _currentAssignedIds.Add(employeeId);
        }

        private async Task SaveAssignments()
        {
            if (_benefitToAssign == null) return;
            _isAssignLoading = true;
            
            try 
            {
                // Find IDs to add
                var toAdd = _currentAssignedIds.Except(_initiallyAssignedIds).ToList();
                // Find IDs to remove
                var toRemove = _initiallyAssignedIds.Except(_currentAssignedIds).ToList();
                
                // Batched Request
                var batchReq = new BatchAssignRequest
                {
                    BenefitId = _benefitToAssign.Id,
                    EmployeeIdsToAssign = toAdd,
                    EmployeeIdsToRemove = toRemove,
                    StartDate = _assignRequest.StartDate,
                    EndDate = _assignRequest.EndDate,
                    OverrideAmount = _assignRequest.OverrideAmount,
                    Note = _assignRequest.Note
                };

                if (await BenefitService.BatchAssignBenefitsAsync(batchReq))
                {
                    try { await JS.InvokeVoidAsync("toastr.success", $"Đã lưu thay đổi: Thêm {toAdd.Count}, Xóa {toRemove.Count}."); } catch { }
                    await LoadData();
                    CloseAssignModal();
                }
                else
                {
                    try { await JS.InvokeVoidAsync("toastr.error", "Lỗi khi lưu phân công hàng loạt."); } catch { }
                }
            }
            catch (Exception)
            {
                try { await JS.InvokeVoidAsync("toastr.error", "Lỗi khi lưu phân công."); } catch { }
            }
            finally
            {
                _isAssignLoading = false;
            }
        }

        private async Task AssignToAllEmployees()
        {
            if (_benefitToAssign == null || !_allEmployees.Any()) return;
            
            var filtered = FilteredEmployees;
            foreach (var emp in filtered)
            {
                if (!_currentAssignedIds.Contains(emp.Id))
                    _currentAssignedIds.Add(emp.Id);
            }
            StateHasChanged();
        }

        // ── FILTER ───────────────────────────────────────────────────────────────
        private List<BenefitModel> FilteredBenefits => _benefits
            .Where(x => (string.IsNullOrWhiteSpace(_searchTerm) || 
                         x.Name.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) ||
                         x.Code.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase)) &&
                         (string.IsNullOrWhiteSpace(_selectedCategory) || x.Category == _selectedCategory))
            .ToList();

        private List<EmployeeDto> FilteredEmployees 
        {
            get 
            {
                var query = _allEmployees.AsEnumerable();
                
                if (_allocationTab == "assigned")
                    query = query.Where(e => _currentAssignedIds.Contains(e.Id));
                else
                    query = query.Where(e => !_currentAssignedIds.Contains(e.Id));

                if (!string.IsNullOrWhiteSpace(_employeeSearchTerm))
                {
                    query = query.Where(e =>
                        (e.FullName ?? "").Contains(_employeeSearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        (e.Code ?? "").Contains(_employeeSearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        (e.DepartmentName ?? "").Contains(_employeeSearchTerm, StringComparison.OrdinalIgnoreCase));
                }
                
                return query.ToList();
            }
        }

        // ── HELPERS ──────────────────────────────────────────────────────────────
        private string GetTypeBadgeClass(string type) => type switch
        {
            "Cố định hàng tháng" or "Monthly" => "badge-light-primary",
            "Sự kiện" or "Event" or "Yearly" => "badge-light-info",
            "Một lần" or "One-time" or "OneTime" => "badge-light-success",
            "Số lượng" or "Quantity" => "badge-light-warning",
            _ => "badge-light-primary"
        };

        private string TranslateType(string type) => type switch
        {
            "Monthly" => "Cố định hàng tháng",
            "Event" or "Yearly" => "Định kỳ năm",
            "One-time" or "OneTime" => "Một lần",
            "Quantity" => "Số lượng",
            _ => type
        };

        private string GetIconByBenefit(BenefitModel item)
        {
            if (!string.IsNullOrEmpty(item.Icon)) return item.Icon;
            return item.Category switch
            {
                "Allowance" => "ki-outline ki-wallet",
                "Insurance" => "ki-outline ki-shield-search",
                "Bonus" => "ki-outline ki-gift",
                _ => "ki-outline ki-briefcase"
            };
        }
        private async Task ExportExcel()
        {
            try
            {
                var bytes = await BenefitService.ExportBenefitsAsync(_searchTerm, _selectedCategory);
                if (bytes != null)
                {
                    var fileName = $"DanhSachPhucLoi_{DateTime.Now:yyyyMMdd}.xlsx";
                    await JS.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(bytes));
                    try { await JS.InvokeVoidAsync("toastr.success", "Đã xuất file Excel thành công."); } catch { }
                }
                else
                {
                    try { await JS.InvokeVoidAsync("toastr.error", "Xuất file Excel thất bại."); } catch { }
                }
            }
            catch { }
        }
    }
}
