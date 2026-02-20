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

        [Parameter, SupplyParameterFromQuery(Name = "q")] public string searchQuery { get; set; } = "";
        [Parameter, SupplyParameterFromQuery(Name = "dept")] public string filterDept { get; set; } = "";
        [Parameter, SupplyParameterFromQuery(Name = "status")] public string filterStatus { get; set; } = "";
        [Parameter, SupplyParameterFromQuery(Name = "page")] public int currentPage { get; set; } = 1;
        
        private bool isContextMenuVisible = false;
        private double contextMenuX = 0;
        private double contextMenuY = 0;
        private bool _isLoading = true;

        private List<DepartmentModel> CachedDepartments = new();
        private List<PositionModel> CachedPositions = new();
        private List<LookupModel> CachedStatuses = new();
        private List<LookupModel> CachedContractTypes = new();
        private List<EmployeeDto> CachedEmployees = new();
        private bool _loadFailed = false;
        
        private int pageSize = 10;
        private long totalCount = 0;
        private int totalPages => (int)Math.Ceiling(totalCount / (double)pageSize);

        protected override async Task OnParametersSetAsync()
        {
            if (currentPage < 1) currentPage = 1;
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            if (_isLoading && employees.Any()) return; // Prevent double load if already loading

            _isLoading = true;
            try 
            {
                if (!CachedDepartments.Any())
                {
                    CachedDepartments = await DepartmentService.GetDepartmentsAsync();
                }
                if (!CachedPositions.Any())
                {
                    CachedPositions = await PositionService.GetPositionsAsync();
                }
                if (!CachedStatuses.Any())
                {
                    CachedStatuses = await MasterDataService.GetCachedLookupsAsync("EmployeeStatus");
                }
                if (!CachedContractTypes.Any())
                {
                    CachedContractTypes = await MasterDataService.GetCachedLookupsAsync("ContractType");
                }
                if (!CachedEmployees.Any())
                {
                    var employeesList = await EmployeeService.GetEmployeesAsync();
                    CachedEmployees = employeesList;
                }
                
                await LoadEmployeesAsync();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"EmployeeList Error: {ex.Message}");
                _loadFailed = true;
            }
            finally 
            {
                _isLoading = false;
            }
        }

        private async Task LoadEmployeesAsync()
        {
            _isLoading = true;
            try
            {
                var departmentId = CachedDepartments.FirstOrDefault(d => d.Name == filterDept)?.Id;
                var pagedResult = await EmployeeService.GetEmployeesPaginatedAsync(currentPage, pageSize, searchQuery, departmentId, filterStatus);
                
                totalCount = pagedResult.TotalCount;
                var positionsList = await PositionService.GetPositionsAsync();

                employees = pagedResult.Items.Select(e => new EmployeeModel
                {
                    Id = e.Id,
                    Code = e.Code,
                    Name = e.FullName,
                    FullName = e.FullName,
                    Email = e.Email,
                    CompanyEmail = e.Email,
                    Dept = e.DepartmentName,
                    DeptId = e.DepartmentId,
                    DepartmentId = e.DepartmentId,
                    Role = e.PositionName,
                    PositionId = e.PositionId,
                    JoinDate = e.JoinDate,
                    StatusId = e.StatusId,
                    StatusName = e.StatusName,
                    ContractTypeId = e.ContractTypeId,
                    ContractTypeName = e.ContractTypeName,
                    Avatar = e.AvatarUrl,
                    AvatarUrl = e.AvatarUrl,
                    Phone = e.Phone,
                    IsActive = true
                }).ToList();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"LoadEmployeesAsync Error: {ex.Message}");
                _loadFailed = true;
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        private bool isPhoneVisible = false;
        private bool isIdCardVisible = false;
        private bool isSalaryVisible = false;
        private bool isPasswordVisible = false;

        private TTL.HR.Shared.Components.Common.CccdScanner cccdScanner = default!;

        private string activeTab = "profile";

        private HashSet<string> selectedIds = new();
        private bool isAllSelected => FilteredEmployees.Any() && selectedIds.Count >= FilteredEmployees.Count();

        private EmployeeModel selectedEmployee = new();
        private List<EmployeeModel> employees = new();

        private IEnumerable<EmployeeModel> FilteredEmployees => employees;

        private void UpdateUrl()
        {
            var query = new Dictionary<string, object?>();
            if (!string.IsNullOrEmpty(searchQuery)) query["q"] = searchQuery;
            if (!string.IsNullOrEmpty(filterDept)) query["dept"] = filterDept;
            if (!string.IsNullOrEmpty(filterStatus)) query["status"] = filterStatus;
            if (currentPage > 1) query["page"] = currentPage;

            var url = Nav.GetUriWithQueryParameters(query);
            Nav.NavigateTo(url);
        }

        private async Task ResetFilters() {
            searchQuery = "";
            filterDept = "";
            filterStatus = "";
            currentPage = 1;
            selectedIds.Clear();
            UpdateUrl();
        }

        private async Task ApplyFilters() {
            currentPage = 1;
            selectedIds.Clear();
            UpdateUrl();
        }
        
        private async Task HandleSearch(ChangeEventArgs e)
        {
            searchQuery = e.Value?.ToString() ?? "";
            currentPage = 1;
            UpdateUrl();
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

        private async Task GoToPage(int page) {
            if (page < 1 || page > totalPages) return;
            currentPage = page;
            UpdateUrl();
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
                // Client-side validation check
                if (string.IsNullOrEmpty(selectedEmployee.FullName) && string.IsNullOrEmpty(selectedEmployee.Name))
                {
                    await JSRuntime.InvokeVoidAsync("Swal.fire", "Thông báo", "Vui lòng nhập Họ tên nhân viên.", "warning");
                    return;
                }
                if (string.IsNullOrEmpty(selectedEmployee.DepartmentId))
                {
                    await JSRuntime.InvokeVoidAsync("Swal.fire", "Thông báo", "Vui lòng chọn Phòng ban.", "warning");
                    return;
                }
                if (string.IsNullOrEmpty(selectedEmployee.PositionId))
                {
                    await JSRuntime.InvokeVoidAsync("Swal.fire", "Thông báo", "Vui lòng chọn Chức vụ.", "warning");
                    return;
                }
                if (string.IsNullOrEmpty(selectedEmployee.StatusId))
                {
                    await JSRuntime.InvokeVoidAsync("Swal.fire", "Thông báo", "Vui lòng chọn Trạng thái nhân viên.", "warning");
                    return;
                }
                if (string.IsNullOrEmpty(selectedEmployee.Email))
                {
                    await JSRuntime.InvokeVoidAsync("Swal.fire", "Thông báo", "Vui lòng nhập Email.", "warning");
                    return;
                }
                if (string.IsNullOrEmpty(selectedEmployee.Phone))
                {
                    await JSRuntime.InvokeVoidAsync("Swal.fire", "Thông báo", "Vui lòng nhập Số điện thoại.", "warning");
                    return;
                }
                if (string.IsNullOrEmpty(selectedEmployee.IdCard))
                {
                    await JSRuntime.InvokeVoidAsync("Swal.fire", "Thông báo", "Vui lòng nhập Số CCCD.", "warning");
                    return;
                }

                var updateRequest = new UpdateEmployeeRequest
                {
                    Id = selectedEmployee.Id,
                    FullName = string.IsNullOrEmpty(selectedEmployee.FullName) ? selectedEmployee.Name : selectedEmployee.FullName,
                    Email = selectedEmployee.Email,
                    CompanyEmail = selectedEmployee.CompanyEmail,
                    Phone = selectedEmployee.Phone,
                    AvatarUrl = string.IsNullOrEmpty(selectedEmployee.Avatar) ? selectedEmployee.AvatarUrl : selectedEmployee.Avatar,
                    DepartmentId = selectedEmployee.DepartmentId,
                    PositionId = selectedEmployee.PositionId,
                    ReportToId = IsValidObjectId(selectedEmployee.ReportToId) ? selectedEmployee.ReportToId : null,
                    StatusId = IsValidObjectId(selectedEmployee.StatusId) ? selectedEmployee.StatusId : (CachedStatuses.FirstOrDefault()?.Id),
                    ContractTypeId = IsValidObjectId(selectedEmployee.ContractTypeId) ? selectedEmployee.ContractTypeId : (CachedContractTypes.FirstOrDefault()?.Id ?? "65dae2f30000000000000202"),
                    JoinDate = selectedEmployee.JoinDate ?? DateTime.UtcNow,
                    Salary = ParseSalary(selectedEmployee.Salary),
                    ContractEndDate = selectedEmployee.ContractExpiry ?? selectedEmployee.ContractEndDate,
                    Workplace = selectedEmployee.Workplace,
                    IsAccountActive = selectedEmployee.IsActive,
                    PersonalDetails = new PersonalDetailsUpdateDto
                    {
                        DOB = selectedEmployee.DOB,
                        Gender = selectedEmployee.Gender,
                        Address = selectedEmployee.Address,
                        Hometown = selectedEmployee.Hometown,
                        IdCardNumber = selectedEmployee.IdCard,
                        IdCardPlace = selectedEmployee.CccdIssuePlace,
                        IdCardIssueDate = selectedEmployee.CccdIssueDate,
                        TaxCode = selectedEmployee.TaxId,
                        BankAccount = selectedEmployee.BankAccountNumber,
                        BankName = selectedEmployee.BankName,
                        Nationality = string.IsNullOrEmpty(selectedEmployee.Nationality) ? "Việt Nam" : selectedEmployee.Nationality,
                        Ethnicity = string.IsNullOrEmpty(selectedEmployee.Ethnicity) ? "Kinh" : selectedEmployee.Ethnicity,
                        Religion = string.IsNullOrEmpty(selectedEmployee.Religion) ? "Không" : selectedEmployee.Religion,
                        MaritalStatus = string.IsNullOrEmpty(selectedEmployee.MaritalStatus) ? "Độc thân" : selectedEmployee.MaritalStatus,
                        PlaceOfOrigin = selectedEmployee.PlaceOfOrigin,
                        Residence = selectedEmployee.Residence,
                        SocialInsuranceId = selectedEmployee.SocialInsuranceId
                    },
                    EmergencyContact = new EmergencyContactUpdateDto
                    {
                        Name = selectedEmployee.EmergencyContactName,
                        Relation = selectedEmployee.EmergencyContactRelation,
                        Phone = selectedEmployee.EmergencyContactPhone
                    }
                };

                var errorMsg = await EmployeeService.UpdateEmployeeAsync(selectedEmployee.Id, updateRequest);
                
                if (string.IsNullOrEmpty(errorMsg))
                {
                    await JSRuntime.InvokeVoidAsync("Swal.fire", "Thành công", "Thông tin nhân viên đã được cập nhật.", "success");
                    
                    // Update data in the list immediately to reflect changes
                    var targetEmp = employees.FirstOrDefault(e => e.Id == selectedEmployee.Id);
                    if (targetEmp != null)
                    {
                        targetEmp.FullName = updateRequest.FullName;
                        targetEmp.Name = updateRequest.FullName; // Ensure Name property is also updated
                        targetEmp.Email = updateRequest.Email;
                        targetEmp.Phone = updateRequest.Phone;
                        targetEmp.StatusName = CachedStatuses.FirstOrDefault(s => s.Id == updateRequest.StatusId)?.Name ?? "Unknown";
                        targetEmp.Dept = CachedDepartments.FirstOrDefault(d => d.Id == updateRequest.DepartmentId)?.Name ?? "Unknown";
                        targetEmp.Role = CachedPositions.FirstOrDefault(p => p.Id == updateRequest.PositionId)?.Name ?? "Unknown";
                        targetEmp.Avatar = updateRequest.AvatarUrl;
                        targetEmp.AvatarUrl = updateRequest.AvatarUrl;
                    }

                    // Also update the currently selected employee model to reflect changes in the UI
                    selectedEmployee.FullName = updateRequest.FullName;
                    selectedEmployee.Name = updateRequest.FullName; 
                    selectedEmployee.Email = updateRequest.Email;
                    selectedEmployee.Phone = updateRequest.Phone;
                    selectedEmployee.DepartmentId = updateRequest.DepartmentId;
                    selectedEmployee.Dept = CachedDepartments.FirstOrDefault(d => d.Id == updateRequest.DepartmentId)?.Name ?? "Unknown";
                    selectedEmployee.PositionId = updateRequest.PositionId;
                    selectedEmployee.Role = CachedPositions.FirstOrDefault(p => p.Id == updateRequest.PositionId)?.Name ?? "Unknown";
                    selectedEmployee.StatusId = updateRequest.StatusId;
                    selectedEmployee.StatusName = CachedStatuses.FirstOrDefault(s => s.Id == updateRequest.StatusId)?.Name ?? "Unknown";
                    selectedEmployee.ContractTypeId = updateRequest.ContractTypeId;
                    selectedEmployee.ContractTypeName = CachedContractTypes.FirstOrDefault(c => c.Id == updateRequest.ContractTypeId)?.Name ?? "Unknown";
                    selectedEmployee.Avatar = updateRequest.AvatarUrl;
                    selectedEmployee.AvatarUrl = updateRequest.AvatarUrl;
                    
                    // Force UI refresh
                    StateHasChanged();

                    await LoadEmployeesAsync(); 
                    await CloseModal();
                }
                else 
                {
                   await JSRuntime.InvokeVoidAsync("Swal.fire", "Thông báo", "Cập nhật không thành công. Chi tiết lỗi: " + errorMsg, "error");
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("Swal.fire", "Lỗi", "Không thể cập nhật hồ sơ: " + ex.Message, "error");
            }
        }

        private async Task CloseModal()
        {
            try {
                await JSRuntime.InvokeVoidAsync("eval", "document.getElementById('kt_drawer_employee_detail_close')?.click()");
            } catch { } // Ignore errors if element doesn't exist
        }

        private async Task ScanCCCD() {
            if (selectedEmployee == null) return;

            var scannedData = await cccdScanner.ScanAsync();
            
            if (scannedData != null)
            {
                selectedEmployee.Name = scannedData.Name;
                selectedEmployee.FullName = scannedData.Name;
                selectedEmployee.DOB = scannedData.DOB ?? selectedEmployee.DOB;
                selectedEmployee.Gender = scannedData.Gender;
                selectedEmployee.IdCard = scannedData.IdCard;
                selectedEmployee.Address = scannedData.Address;
                selectedEmployee.Hometown = scannedData.Hometown;
                selectedEmployee.CccdIssuePlace = scannedData.IssuePlace;
                selectedEmployee.CccdIssueDate = DateTime.TryParse(scannedData.IssueDate, out var iDate) ? iDate : null;
                
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

        private async Task SelectEmployeeByObj(EmployeeModel emp) {
            _isLoading = true;
            try
            {
                var fullDetail = await EmployeeService.GetEmployeeAsync(emp.Id);
                if (fullDetail != null)
                {
                    selectedEmployee = fullDetail;
                    if (string.IsNullOrEmpty(selectedEmployee.Name)) selectedEmployee.Name = selectedEmployee.FullName;
                    
                    // Synchronize fields that have different names in API vs UI Model
                    if (selectedEmployee.ContractEndDate.HasValue)
                        selectedEmployee.ContractExpiry = selectedEmployee.ContractEndDate;
                    
                    if (selectedEmployee.SalaryAmount.HasValue)
                        selectedEmployee.Salary = selectedEmployee.SalaryAmount.Value.ToString("N0");

                    // Ensure ID fields are consistent for Dropdown binding
                    if (string.IsNullOrEmpty(selectedEmployee.DepartmentId) && !string.IsNullOrEmpty(selectedEmployee.DeptId))
                        selectedEmployee.DepartmentId = selectedEmployee.DeptId;
                    else if (!string.IsNullOrEmpty(selectedEmployee.DepartmentId))
                        selectedEmployee.DeptId = selectedEmployee.DepartmentId;

                    if (string.IsNullOrEmpty(selectedEmployee.PositionId) && !string.IsNullOrEmpty(emp.PositionId))
                        selectedEmployee.PositionId = emp.PositionId;

                    if (string.IsNullOrEmpty(selectedEmployee.Dept)) selectedEmployee.Dept = emp.Dept;
                    if (string.IsNullOrEmpty(selectedEmployee.Role)) selectedEmployee.Role = emp.Role;
                    
                    if (string.IsNullOrEmpty(selectedEmployee.StatusId)) selectedEmployee.StatusId = emp.StatusId;
                    if (string.IsNullOrEmpty(selectedEmployee.ContractTypeId)) selectedEmployee.ContractTypeId = emp.ContractTypeId;
                    if (string.IsNullOrEmpty(selectedEmployee.Phone)) selectedEmployee.Phone = emp.Phone;
                    if (string.IsNullOrEmpty(selectedEmployee.Email)) selectedEmployee.Email = emp.Email;
                    if (string.IsNullOrEmpty(selectedEmployee.Avatar)) selectedEmployee.Avatar = emp.Avatar;
                    
                    // Flatten PersonalDetails to root properties for direct UI binding
                    if (selectedEmployee.PersonalDetails != null)
                    {
                        selectedEmployee.DOB ??= selectedEmployee.PersonalDetails.DOB;
                        selectedEmployee.Gender = !string.IsNullOrEmpty(selectedEmployee.Gender) ? selectedEmployee.Gender : selectedEmployee.PersonalDetails.Gender;
                        selectedEmployee.Address = !string.IsNullOrEmpty(selectedEmployee.Address) ? selectedEmployee.Address : selectedEmployee.PersonalDetails.Address;
                        selectedEmployee.Hometown = !string.IsNullOrEmpty(selectedEmployee.Hometown) ? selectedEmployee.Hometown : selectedEmployee.PersonalDetails.Hometown;
                        selectedEmployee.IdCard = !string.IsNullOrEmpty(selectedEmployee.IdCard) ? selectedEmployee.IdCard : selectedEmployee.PersonalDetails.IdCardNumber;
                        
                        if (!selectedEmployee.CccdIssueDate.HasValue) 
                            selectedEmployee.CccdIssueDate = selectedEmployee.PersonalDetails.IdCardIssueDate;
                            
                        selectedEmployee.CccdIssuePlace = !string.IsNullOrEmpty(selectedEmployee.CccdIssuePlace) ? selectedEmployee.CccdIssuePlace : selectedEmployee.PersonalDetails.IdCardPlace;
                        selectedEmployee.TaxId = !string.IsNullOrEmpty(selectedEmployee.TaxId) ? selectedEmployee.TaxId : selectedEmployee.PersonalDetails.TaxCode;
                        selectedEmployee.BankAccountNumber = !string.IsNullOrEmpty(selectedEmployee.BankAccountNumber) ? selectedEmployee.BankAccountNumber : selectedEmployee.PersonalDetails.BankAccount;
                        selectedEmployee.BankName = !string.IsNullOrEmpty(selectedEmployee.BankName) ? selectedEmployee.BankName : selectedEmployee.PersonalDetails.BankName;
                        selectedEmployee.Nationality = !string.IsNullOrEmpty(selectedEmployee.Nationality) ? selectedEmployee.Nationality : selectedEmployee.PersonalDetails.Nationality;
                        selectedEmployee.Ethnicity = !string.IsNullOrEmpty(selectedEmployee.Ethnicity) ? selectedEmployee.Ethnicity : selectedEmployee.PersonalDetails.Ethnicity;
                        selectedEmployee.Religion = !string.IsNullOrEmpty(selectedEmployee.Religion) ? selectedEmployee.Religion : selectedEmployee.PersonalDetails.Religion;
                        selectedEmployee.SocialInsuranceId = !string.IsNullOrEmpty(selectedEmployee.SocialInsuranceId) ? selectedEmployee.SocialInsuranceId : selectedEmployee.PersonalDetails.SocialInsuranceId;
                        selectedEmployee.MaritalStatus = !string.IsNullOrEmpty(selectedEmployee.MaritalStatus) ? selectedEmployee.MaritalStatus : selectedEmployee.PersonalDetails.MaritalStatus;
                        selectedEmployee.PlaceOfOrigin = !string.IsNullOrEmpty(selectedEmployee.PlaceOfOrigin) ? selectedEmployee.PlaceOfOrigin : selectedEmployee.PersonalDetails.PlaceOfOrigin;
                        selectedEmployee.Residence = !string.IsNullOrEmpty(selectedEmployee.Residence) ? selectedEmployee.Residence : selectedEmployee.PersonalDetails.Residence;
                    }

                    if (selectedEmployee.EmergencyContact != null)
                    {
                        selectedEmployee.EmergencyContactName = !string.IsNullOrEmpty(selectedEmployee.EmergencyContactName) ? selectedEmployee.EmergencyContactName : selectedEmployee.EmergencyContact.Name;
                        selectedEmployee.EmergencyContactRelation = !string.IsNullOrEmpty(selectedEmployee.EmergencyContactRelation) ? selectedEmployee.EmergencyContactRelation : selectedEmployee.EmergencyContact.Relation;
                        selectedEmployee.EmergencyContactPhone = !string.IsNullOrEmpty(selectedEmployee.EmergencyContactPhone) ? selectedEmployee.EmergencyContactPhone : selectedEmployee.EmergencyContact.Phone;
                    }
                    
                    selectedEmployee.IsActive = selectedEmployee.IsActive || selectedEmployee.IsAccountActive;
                    
                    if (string.IsNullOrEmpty(selectedEmployee.Email) && !string.IsNullOrEmpty(selectedEmployee.CompanyEmail)) 
                        selectedEmployee.Email = selectedEmployee.CompanyEmail;
                }
                else
                {
                    selectedEmployee = emp;
                }
            }
            catch
            {
                selectedEmployee = emp;
            }
            finally
            {
                _isLoading = false;
                isPhoneVisible = false;
                isIdCardVisible = false;
                isSalaryVisible = false;
                isPasswordVisible = false;
                StateHasChanged();
            }
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

                if (_loadFailed)
                {
                    try {
                        await JSRuntime.InvokeVoidAsync("toastr.error", "Có lỗi xảy ra khi tải dữ liệu nhân viên. Vui lòng kiểm tra kết nối API.");
                    } catch { }
                }
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
        private decimal? ParseSalary(string? salaryStr)
        {
            if (string.IsNullOrWhiteSpace(salaryStr)) return null;
            
            // Remove non-numeric characters except for the decimal point
            var cleaned = new string(salaryStr.Where(c => char.IsDigit(c) || c == '.' || c == ',').ToArray());
            cleaned = cleaned.Replace(",", "."); // Standardize decimal separator
            
            // If there are multiple dots (like in 50.000.000), keep only the numeric parts
            if (cleaned.Count(c => c == '.') > 1)
            {
                cleaned = new string(cleaned.Where(char.IsDigit).ToArray());
            }

            return decimal.TryParse(cleaned, out var result) ? result : null;
        }

        private bool IsValidObjectId(string? id)
        {
            if (string.IsNullOrWhiteSpace(id)) return false;
            if (id.Length != 24) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(id, @"^[0-9a-fA-F]{24}$");
        }
    }
}
