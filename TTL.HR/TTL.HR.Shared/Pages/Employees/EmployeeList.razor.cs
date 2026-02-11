using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.HumanResource.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Models;
using TTL.HR.Application.Modules.Organization.Interfaces;
using TTL.HR.Application.Modules.Organization.Models;
using Entities = TTL.HR.Application.Modules.HumanResource.Entities;

namespace TTL.HR.Shared.Pages.Employees
{
    public partial class EmployeeList
    {
        [Inject] public IEmployeeService EmployeeService { get; set; } = default!;
        [Inject] public IMasterDataService MasterDataService { get; set; } = default!;
        [Inject] public IDepartmentService DepartmentService { get; set; } = default!;
        [Inject] public IPositionService PositionService { get; set; } = default!;
        [Inject] public IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] public NavigationManager Nav { get; set; } = default!;

        private string searchQuery = "";
        private string filterDept = "";
        private string filterStatus = "";
        private bool isContextMenuVisible = false;
        private double contextMenuX = 0;
        private double contextMenuY = 0;
        private bool _isLoading = true;

        private List<DepartmentModel> CachedDepartments = new();
        private List<LookupModel> CachedStatuses = new();

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;
            try 
            {
                CachedDepartments = await DepartmentService.GetDepartmentsAsync();
                var positionsList = await PositionService.GetPositionsAsync();
                CachedStatuses = await MasterDataService.GetCachedLookupsAsync("EmployeeStatus");

                var empList = await EmployeeService.GetEmployeesAsync();
                employees = empList.Select(e => new EmployeeModel
                {
                    Id = e.Id,
                    Code = e.Code,
                    Name = e.FullName,
                    Email = e.Email,
                    CompanyEmail = e.CompanyEmail,
                    Dept = CachedDepartments.FirstOrDefault(d => d.Id == e.DepartmentId)?.Name ?? e.DepartmentId,
                    DeptId = e.DepartmentId,
                    Role = positionsList.FirstOrDefault(p => p.Id == e.PositionId)?.Name ?? e.PositionId,
                    PositionId = e.PositionId,
                    JoinDate = e.JoinDate,
                    Status = e.Status.ToString(),
                    Avatar = e.AvatarUrl,
                    Phone = e.Phone,
                    IdCard = e.PersonalDetails?.IdCardNumber ?? "",
                    DOB = e.PersonalDetails?.DOB,
                    Gender = e.PersonalDetails?.Gender ?? "",
                    Address = e.PersonalDetails?.Address ?? "",
                    Hometown = e.PersonalDetails?.Hometown ?? "",
                    Salary = e.PersonalDetails?.TaxCode ?? "", // Just a placeholder
                    IsActive = true
                }).ToList();
            }
            finally 
            {
                _isLoading = false;
            }
        }

        private int currentPage = 1;
        private bool isPhoneVisible = false;
        private bool isIdCardVisible = false;
        private bool isSalaryVisible = false;

        private TTL.HR.Shared.Components.Common.CccdScanner cccdScanner = default!;

        private string activeTab = "profile";

        private HashSet<string> selectedIds = new();
        private bool isAllSelected => FilteredEmployees.Any() && selectedIds.Count >= FilteredEmployees.Count();

        private EmployeeModel selectedEmployee = new();
        private List<EmployeeModel> employees = new();

        private IEnumerable<EmployeeModel> FilteredEmployees => employees
            .Where(e => (string.IsNullOrEmpty(searchQuery) || e.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) || e.Id.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                    && (string.IsNullOrEmpty(filterDept) || e.Dept == filterDept)
                    && (string.IsNullOrEmpty(filterStatus) || e.Status == filterStatus));

        private void ResetFilters() {
            filterDept = "";
            filterStatus = "";
            searchQuery = "";
        }

        private void ApplyFilters() {
            currentPage = 1;
            selectedIds.Clear();
            StateHasChanged();
        }

        private void ToggleSelectAll(ChangeEventArgs e) {
            bool isChecked = (bool)(e.Value ?? false);
            if (isChecked) {
                foreach (var emp in FilteredEmployees) selectedIds.Add(emp.Id);
            } else {
                selectedIds.Clear();
            }
        }

        private void ToggleSelection(string id, object value) {
            bool isChecked = (bool)(value ?? false);
            if (isChecked) selectedIds.Add(id);
            else selectedIds.Remove(id);
        }

        private void GoToPage(int page) {
            if (page < 1 || page > 3) return;
            currentPage = page;
            StateHasChanged();
        }

        private async Task ExportExcel() {
            await JSRuntime.InvokeVoidAsync("Swal.fire", new {
                title = "Xuất dữ liệu Excel",
                text = "Hệ thống đang trích xuất dữ liệu của " + FilteredEmployees.Count() + " nhân viên...",
                timer = 1500,
                icon = "info",
                showConfirmButton = false
            });
        }

        private async Task SaveChanges() {
            if (selectedEmployee == null) return;

            try 
            {
                var empEntity = new Entities.Employee
                {
                    Code = selectedEmployee.Code,
                    FullName = selectedEmployee.Name,
                    Email = selectedEmployee.Email,
                    CompanyEmail = selectedEmployee.CompanyEmail,
                    Phone = selectedEmployee.Phone,
                    AvatarUrl = selectedEmployee.Avatar,
                    DepartmentId = selectedEmployee.DeptId,
                    PositionId = selectedEmployee.PositionId,
                    JoinDate = selectedEmployee.JoinDate,
                    Status = Enum.TryParse<Entities.EmployeeStatus>(selectedEmployee.Status, true, out var status) ? status : Entities.EmployeeStatus.Probation,
                    PersonalDetails = new Entities.PersonalInfo
                    {
                        DOB = selectedEmployee.DOB,
                        Gender = selectedEmployee.Gender,
                        Address = selectedEmployee.Address,
                        Hometown = selectedEmployee.Hometown,
                        IdCardNumber = selectedEmployee.IdCard,
                        TaxCode = selectedEmployee.Salary // Mapping Salary field back to TaxCode as placeholder
                    }
                };

                var updated = await EmployeeService.UpdateEmployeeAsync(selectedEmployee.Id, empEntity);
                
                if (updated != null)
                {
                    await JSRuntime.InvokeVoidAsync("Swal.fire", new {
                        title = "Thành công!",
                        text = "Dữ liệu hồ sơ của " + selectedEmployee?.Name + " đã được cập nhật.",
                        icon = "success",
                        confirmButtonText = "Đóng"
                    });
                    
                    // Refresh list or update local item
                    var index = employees.FindIndex(e => e.Id == selectedEmployee.Id);
                    if (index != -1)
                    {
                        employees[index].Name = updated.FullName;
                        employees[index].Email = updated.Email;
                        // ... other fields if necessary
                    }
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("Swal.fire", "Lỗi", "Không thể cập nhật hồ sơ: " + ex.Message, "error");
            }
        }

        private async Task ScanCCCD() {
            if (selectedEmployee == null) return;

            var scannedData = await cccdScanner.ScanAsync();
            
            if (scannedData != null)
            {
                selectedEmployee.Name = scannedData.Name;
                selectedEmployee.DOB = scannedData.DOB ?? selectedEmployee.DOB;
                selectedEmployee.Gender = scannedData.Gender;
                selectedEmployee.IdCard = scannedData.IdCard;
                selectedEmployee.Address = scannedData.Address;
                selectedEmployee.Hometown = scannedData.Hometown;
                
                await JSRuntime.InvokeVoidAsync("Swal.fire", new {
                    title = "Cập nhật thành công!",
                    text = $"Hồ sơ nhân viên {selectedEmployee.Name} đã được đồng bộ dữ liệu từ thẻ CCCD.",
                    icon = "success",
                    timer = 2000,
                    showConfirmButton = false
                });

                StateHasChanged();
            } else {
                await JSRuntime.InvokeVoidAsync("Swal.fire", "Thông báo", "Quá trình quét bị gián đoạn. Vui lòng thử lại.", "info");
            }
        }

        private void SelectEmployee(int index) {
            selectedEmployee = employees[index - 1];
            StateHasChanged();
        }

        private void SelectEmployeeByObj(EmployeeModel emp) {
            selectedEmployee = emp;
            isPhoneVisible = false;
            isIdCardVisible = false;
            isSalaryVisible = false;
            StateHasChanged();
        }

        private void ShowContextMenu(MouseEventArgs e, EmployeeModel emp) {
            selectedEmployee = emp;
            contextMenuX = e.ClientX;
            contextMenuY = e.ClientY;
            isContextMenuVisible = true;
        }

        private void HideContextMenu() {
            isContextMenuVisible = false;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender) {
            if (firstRender) {
                await JSRuntime.InvokeVoidAsync("KTMenu.createInstances");
                await JSRuntime.InvokeVoidAsync("KTDrawer.createInstances");
            }
        }

        private void ViewProfile() {
            isContextMenuVisible = false;
            // The drawer is toggled by class, but we need to ensure State is updated
            StateHasChanged();
            JSRuntime.InvokeVoidAsync("openEmployeeDetail");
        }

        private async Task GenerateQR() {
            isContextMenuVisible = false;
            await JSRuntime.InvokeVoidAsync("Swal.fire", new {
                title = "Thẻ Nhân viên Kỹ thuật số",
                html = "<div class='text-center'><img src='https://api.qrserver.com/v1/create-qr-code/?size=150x150&data=" + selectedEmployee?.Code + "' class='rounded mb-3' /><div class='fw-bolder fs-4'>" + selectedEmployee?.Name + "</div><div class='text-muted fs-7'>" + selectedEmployee?.Role + "</div></div>",
                showConfirmButton = false,
                showCloseButton = true
            });
        }

        private async Task Offboarding() {
            if (selectedEmployee == null) return;
            
            var result = await JSRuntime.InvokeAsync<bool>("eval", $@"
                (async () => {{
                    const result = await Swal.fire({{
                        text: 'Bạn có chắc chắn muốn kích hoạt quy trình nghỉ việc cho nhân viên {selectedEmployee.Name} (Mã: {selectedEmployee.Code})?',
                        icon: 'warning',
                        showCancelButton: true,
                        buttonsStyling: false,
                        confirmButtonText: 'Đúng, Kích hoạt ngay',
                        cancelButtonText: 'Hủy bỏ',
                        customClass: {{
                            confirmButton: 'btn fw-bold btn-danger',
                            cancelButton: 'btn fw-bold btn-active-light-primary'
                        }}
                    }});
                    return result.isConfirmed;
                }})()
            ");

            if (result) {
                await EmployeeService.DeleteEmployeeAsync(selectedEmployee.Id);
                var empToRemove = employees.FirstOrDefault(e => e.Id == selectedEmployee.Id);
                if (empToRemove != null)
                {
                    employees.Remove(empToRemove);
                }
                
                await JSRuntime.InvokeVoidAsync("Swal.fire", new {
                    title = "Đã xử lý!",
                    text = $"Quy trình nghỉ việc cho nhân viên {selectedEmployee.Name} đã được khởi tạo.",
                    icon = "success",
                    buttonsStyling = false,
                    confirmButtonText = "Hoàn tất",
                    customClass = new {
                        confirmButton = "btn fw-bold btn-primary"
                    }
                });

                StateHasChanged();
            }
        }
    }
}
