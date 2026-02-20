using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Organization.Interfaces;
using TTL.HR.Application.Modules.Organization.Models;

namespace TTL.HR.Shared.Pages.Organization
{
    public partial class PositionList
    {
        [Inject] private IPositionService PositionService { get; set; } = default!;
        [Inject] private IDepartmentService DepartmentService { get; set; } = default!;
        [Inject] private Microsoft.JSInterop.IJSRuntime JS { get; set; } = default!;

        private bool _showEditDrawer = false;
        private bool _showDetailDrawer = false;
        private bool _showConfirmDeleteModal = false;
        private bool _isNewPosition = true;
        private bool _loadError = false;
        
        private string _searchTerm = "";
        private string _levelFilter = "All";
        private bool _isLoading = true;
        private int currentPage = 1;
        private int pageSize = 10;

        private List<DepartmentModel> _departments = new();
        private List<PositionItem> _positions = new();
        private PositionItem _editingPos = new();
        private PositionItem? _selectedPos;
        private PositionItem? _positionToDelete;

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async System.Threading.Tasks.Task LoadData()
        {
            _isLoading = true;
            try
            {
                _departments = await DepartmentService.GetDepartmentsAsync();
                var positions = await PositionService.GetPositionsAsync();
                _positions = positions.Select(p => new PositionItem
                {
                    Id = p.Id,
                    NameVN = p.Name ?? "",
                    NameEN = p.Code ?? "", // Assuming Code as EN name for now or map accordingly
                    Group = p.DepartmentName ?? "",
                    DepartmentId = p.DepartmentId,
                    LevelName = p.Level ?? "Nhân viên",
                    LevelBadge = GetLevelBadge(p.Level ?? ""),
                    ActiveEmployees = 0, // Need API to provide this or fetch separately
                    SalaryRange = p.BaseSalaryRangeMin > 0 ? $"{p.BaseSalaryRangeMin} - {p.BaseSalaryRangeMax}" : "Thỏa thuận",
                    JobDescription = p.Description ?? "",
                    Requirements = p.Requirements != null ? string.Join("\n", p.Requirements) : "",
                    Benefits = p.Benefits != null ? string.Join("\n", p.Benefits) : "",
                    Icon = "ki-outline ki-briefcase",
                    IconBg = "bg-light-primary",
                    IconColor = "text-primary"
                }).ToList();
            }
            catch (Exception ex)
            {
                _loadError = true;
                Console.WriteLine($"Error loading positions: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        private string GetLevelBadge(string level) => level switch
        {
            "Director" => "badge-light-danger",
            "Manager" => "badge-light-warning",
            _ => "badge-light-primary"
        };
        
        private List<string> _availableIcons = new() {
            "ki-outline ki-briefcase", "ki-outline ki-award", "ki-outline ki-person-badge",
            "ki-outline ki-palette", "ki-outline ki-megaphone", "ki-outline ki-shield-check",
            "ki-outline ki-bank", "ki-outline ki-graph-up"
        };

        private IEnumerable<PositionItem> FilteredPositions => _positions
            .Where(x => string.IsNullOrEmpty(_searchTerm) || 
                       (x.NameVN?.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) || 
                       (x.NameEN?.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) ?? false))
            .Where(x => _levelFilter == "All" || (x.LevelName?.Contains(_levelFilter) ?? false));

        private void resetFilters() { _searchTerm = ""; _levelFilter = "All"; }

        private void openAddPosition() {
            _isNewPosition = true;
            _editingPos = new PositionItem { LevelName = "Level 1", LevelBadge = "badge-light-primary", Icon = "ki-outline ki-briefcase", IconBg = "bg-light-primary", IconColor = "text-primary" };
            _showEditDrawer = true;
            _showDetailDrawer = false;
        }

        private void editPosition(PositionItem pos) {
            _isNewPosition = false;
            _editingPos = new PositionItem
            {
                Id = pos.Id,
                NameVN = pos.NameVN,
                NameEN = pos.NameEN,
                Group = pos.Group,
                DepartmentId = pos.DepartmentId,
                LevelName = pos.LevelName,
                LevelBadge = pos.LevelBadge,
                ActiveEmployees = pos.ActiveEmployees,
                SalaryRange = pos.SalaryRange,
                JobDescription = pos.JobDescription,
                Requirements = pos.Requirements,
                Benefits = pos.Benefits,
                Icon = pos.Icon,
                IconBg = pos.IconBg,
                IconColor = pos.IconColor
            };
            _showEditDrawer = true;
            _showDetailDrawer = false;
        }

        private void viewDetails(PositionItem pos) {
            _selectedPos = pos;
            _showDetailDrawer = true;
            _showEditDrawer = false;
        }

        private void closeEditDrawer() => _showEditDrawer = false;
        private void closeDetailDrawer() => _showDetailDrawer = false;

        private async System.Threading.Tasks.Task savePosition() {
            _isLoading = true;
            bool success = false;
            try
            {
                // Helper to split text to list for API consistency
                List<string> ToList(string? text) => string.IsNullOrWhiteSpace(text) 
                    ? new List<string>() 
                    : text.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();

                if (_isNewPosition) {
                    var result = await PositionService.CreatePositionAsync(new CreatePositionRequest
                    {
                        Name = _editingPos.NameVN,
                        Code = _editingPos.NameEN,
                        DepartmentId = _editingPos.DepartmentId,
                        Level = _editingPos.LevelName,
                        Description = _editingPos.JobDescription,
                        Requirements = ToList(_editingPos.Requirements),
                        Benefits = ToList(_editingPos.Benefits),
                        Responsibilities = ToList(_editingPos.JobDescription),
                        BaseSalaryRangeMin = 0,
                        BaseSalaryRangeMax = 0
                    });
                    success = result != null;
                }
                else
                {
                    var result = await PositionService.UpdatePositionAsync(_editingPos.Id, new UpdatePositionRequest
                    {
                        Id = _editingPos.Id,
                        Name = _editingPos.NameVN,
                        Code = _editingPos.NameEN,
                        DepartmentId = _editingPos.DepartmentId,
                        Level = _editingPos.LevelName,
                        Description = _editingPos.JobDescription,
                        Requirements = ToList(_editingPos.Requirements),
                        Benefits = ToList(_editingPos.Benefits),
                        Responsibilities = ToList(_editingPos.JobDescription),
                        BaseSalaryRangeMin = 0,
                        BaseSalaryRangeMax = 0,
                        IsActive = true
                    });
                    success = result != null;
                }

                if (success)
                {
                    await JS.InvokeVoidAsync("toastr.success", _isNewPosition ? "Thêm chức danh thành công" : "Cập nhật chức danh thành công");
                    await LoadData();
                    closeEditDrawer();
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", "Lỗi: Không thể lưu thông tin. Vui lòng kiểm tra lại mã hoặc quyền hạn.");
                }
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("toastr.error", $"Lỗi hệ thống: {ex.Message}");
                Console.WriteLine($"Error saving position: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        private void requestDeletePosition(PositionItem pos) {
            _positionToDelete = pos;
            _showConfirmDeleteModal = true;
        }

        private void closeDeleteModal() {
            _showConfirmDeleteModal = false;
            _positionToDelete = null;
        }

        private async System.Threading.Tasks.Task confirmDeletePosition() {
            if (_positionToDelete != null) {
                _isLoading = true;
                try
                {
                    var success = await PositionService.DeletePositionAsync(_positionToDelete.Id);
                    if (success)
                    {
                        await JS.InvokeVoidAsync("toastr.success", "Xóa chức danh thành công");
                        await LoadData();
                    }
                    else
                    {
                        await JS.InvokeVoidAsync("toastr.error", "Lỗi: Không thể xóa chức danh này. Có thể vị trí này đang được gắn với nhân viên.");
                    }
                }
                catch (Exception ex)
                {
                    await JS.InvokeVoidAsync("toastr.error", $"Lỗi hệ thống: {ex.Message}");
                    Console.WriteLine($"Error deleting position: {ex.Message}");
                }
                finally
                {
                    _isLoading = false;
                    closeDeleteModal();
                    StateHasChanged();
                }
            }
        }

        public class PositionItem
        {
            public string Id { get; set; } = string.Empty;
            public string NameVN { get; set; } = "";
            public string NameEN { get; set; } = "";
            public string Group { get; set; } = "";
            public string DepartmentId { get; set; } = "";
            public string LevelName { get; set; } = "";
            public string LevelBadge { get; set; } = "badge-light-primary";
            public int ActiveEmployees { get; set; }
            public string SalaryRange { get; set; } = "Thỏa thuận";
            public string JobDescription { get; set; } = "";
            public string Requirements { get; set; } = "";
            public string Benefits { get; set; } = "";
            public string Icon { get; set; } = "ki-outline ki-briefcase";
            public string IconBg { get; set; } = "bg-light-primary";
            public string IconColor { get; set; } = "text-primary";

            public PositionItem() { }
            public PositionItem(string vn, string en, string group, string level, string badge, int count, string salary, string icon, string bg, string color) {
                NameVN = vn; NameEN = en; Group = group; LevelName = level; LevelBadge = badge; ActiveEmployees = count; SalaryRange = salary;
                Icon = icon; IconBg = bg; IconColor = color;
            }
        }
    }
}
