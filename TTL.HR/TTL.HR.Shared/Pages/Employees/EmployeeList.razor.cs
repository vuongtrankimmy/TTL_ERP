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
using TTL.HR.Application.Modules.Attendance.Interfaces;
using TTL.HR.Application.Modules.Attendance.Models;

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
        [Inject] public IAuthService AuthService { get; set; } = default!;
        [Inject] public IContractService ContractService { get; set; } = default!;
        [Inject] public ISettingsService SettingsService { get; set; } = default!;
        [Inject] public IPdfService PdfService { get; set; } = default!;
        [Inject] public IAttendanceService AttendanceService { get; set; } = default!;


        [Parameter, SupplyParameterFromQuery(Name = "q")] public string searchQuery { get; set; } = "";
        [Parameter, SupplyParameterFromQuery(Name = "dept")] public string filterDept { get; set; } = "";
        [Parameter, SupplyParameterFromQuery(Name = "status")] public string filterStatus { get; set; } = "";
        [Parameter, SupplyParameterFromQuery(Name = "workplace")] public string filterWorkplace { get; set; } = "";
        [Parameter, SupplyParameterFromQuery(Name = "page")] public int currentPage { get; set; } = 1;
        [Parameter, SupplyParameterFromQuery(Name = "sortBy")] public string sortBy { get; set; } = "name";
        [Parameter, SupplyParameterFromQuery(Name = "sortDesc")] public bool sortDesc { get; set; } = false;
        [Parameter, SupplyParameterFromQuery(Name = "viewMode")] public string filterViewMode { get; set; } = "table";
        
        private bool isContextMenuVisible = false;
        private double contextMenuX = 0;
        private double contextMenuY = 0;
               private bool _isLoading = false;
        private string _viewMode = "table"; // "table" or "card"
        private CancellationTokenSource? _cts;
        private string _errorMessage = "";

        private List<DepartmentModel> CachedDepartments = new();
        private List<PositionModel> CachedPositions = new();
        private List<LookupModel> CachedStatuses = new();
        private List<LookupModel> CachedContractTypes = new();
        private List<LookupModel> CachedWorkplaces = new();
        private List<LookupModel> CachedAttendanceStatuses = new();
        private List<LookupModel> CachedRoles = new();
        private List<EmployeeDto> CachedEmployees = new();
        private EmployeeStatusCounts _counts = new();
        private bool _loadFailed = false;
        private bool _isPrintLoading = false;
        private string PreviewContent { get; set; } = "";
        private List<EmployeeModel> employees = new();
        private EmployeeModel selectedEmployee = new();
        private long totalCount = 0;
        private bool _isReadOnlyDetail = true;

        private int pageSize = 10;
        private int totalPages => (int)Math.Ceiling(totalCount / (double)pageSize);

        private TTL.HR.Shared.Components.Common.CccdScanner cccdScanner = default!;
        private ImportWizard importWizard = default!;

        private async Task OpenImportWizard()
        {
            await JSRuntime.InvokeVoidAsync("showModal", "#import_wizard_modal");
        }

        private async Task ReloadData()
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            await LoadDataAsync(_cts.Token);
        }

        protected override async Task OnParametersSetAsync()
        {
            if (currentPage < 1) currentPage = 1;
            if (!string.IsNullOrEmpty(filterViewMode)) _viewMode = filterViewMode;

            // Manual sync if needed (though SupplyParameterFromQuery usually handles this)
            // But if the user reports "wrong state", we can be extra careful.
            
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            
            try 
            {
                await LoadDataAsync(_cts.Token);
            }
            catch (OperationCanceledException) { }
        }

        private void ToggleViewMode(string mode)
        {
            _viewMode = mode;
            UpdateUrl(); // Use UpdateUrl to persist view mode and trigger refresh
        }

        private async Task LoadDataAsync(CancellationToken ct = default)
        {
            _isLoading = true;
            _loadFailed = false;
            _errorMessage = "";
            StateHasChanged();

            try 
            {
                // Initial Settings & Language
                await SettingsService.InitializeAsync();
                var lang = SettingsService.CachedSettings?.DefaultLanguage ?? "vi-VN";
                var tasks = new List<Task>();

                if (!CachedDepartments.Any()) tasks.Add(DepartmentService.GetDepartmentsAsync().ContinueWith(t => { if (t.IsCompletedSuccessfully) CachedDepartments = t.Result; }));
                if (!CachedPositions.Any()) tasks.Add(PositionService.GetPositionsAsync().ContinueWith(t => { if (t.IsCompletedSuccessfully) CachedPositions = t.Result; }));
                if (!CachedStatuses.Any()) tasks.Add(MasterDataService.GetCachedLookupsAsync("EmployeeStatus", lang).ContinueWith(t => { if (t.IsCompletedSuccessfully) CachedStatuses = t.Result; }));
                if (!CachedContractTypes.Any()) tasks.Add(MasterDataService.GetCachedLookupsAsync("ContractType", lang).ContinueWith(t => { if (t.IsCompletedSuccessfully) CachedContractTypes = t.Result; }));
                if (!CachedWorkplaces.Any()) tasks.Add(MasterDataService.GetCachedLookupsAsync("Workplace", lang).ContinueWith(t => { if (t.IsCompletedSuccessfully) CachedWorkplaces = t.Result; }));
                if (!CachedAttendanceStatuses.Any()) tasks.Add(MasterDataService.GetCachedLookupsAsync("AttendanceStatus", lang).ContinueWith(t => { if (t.IsCompletedSuccessfully) CachedAttendanceStatuses = t.Result; }));
                if (!CachedRoles.Any()) tasks.Add(MasterDataService.GetCachedLookupsAsync("Role", lang).ContinueWith(t => { if (t.IsCompletedSuccessfully) CachedRoles = t.Result; }));

                if (tasks.Any())
                {
                    await Task.WhenAll(tasks);
                }
                
                if (ct.IsCancellationRequested) return;

                await LoadEmployeesAsync(ct);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                System.Console.WriteLine($"LoadDataAsync Error: {ex.Message}");
                _loadFailed = true;
                _errorMessage = ex.Message;
            }
            finally
            {
                if (!ct.IsCancellationRequested)
                {
                    _isLoading = false;
                    StateHasChanged();
                }
            }
        }

        private async Task LoadEmployeesAsync(CancellationToken ct = default)
        {
            try
            {
                var departmentId = CachedDepartments.FirstOrDefault(d => d.Name == filterDept)?.Id;
                
                // Fetch counts and paged result
                var countTask = EmployeeService.GetStatusCountsAsync(searchQuery, departmentId, filterWorkplace);
                var pagedTask = EmployeeService.GetEmployeesPaginatedAsync(currentPage, pageSize, searchQuery, departmentId, filterStatus, filterWorkplace, sortBy, sortDesc);
                
                await Task.WhenAll(countTask, pagedTask);
                
                if (ct.IsCancellationRequested) return;

                _counts = countTask.Result;
                var pagedResult = pagedTask.Result;
                
                totalCount = pagedResult.TotalCount;
                
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
                    StatusCode = e.StatusCode,
                    StatusColor = e.StatusColor,
                    ContractTypeId = e.ContractTypeId,
                    ContractTypeName = e.ContractTypeName,
                    Avatar = e.AvatarUrl,
                    AvatarUrl = e.AvatarUrl,
                    Phone = e.Phone,
                    DisplayInitials = GetInitials(e.FullName),
                    IsActive = true,
                    WorkplaceId = e.WorkplaceId,
                    Workplace = e.Workplace
                }).ToList();
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                System.Console.WriteLine($"LoadEmployeesAsync Error: {ex.Message}");
                _loadFailed = true;
                _errorMessage = ex.Message;
            }
        }

        private HashSet<string> selectedIds = new();
        private bool isAllSelected => FilteredEmployees != null && FilteredEmployees.Any() && selectedIds.Count >= FilteredEmployees.Count();

        private IEnumerable<EmployeeModel> FilteredEmployees => employees ?? new List<EmployeeModel>();

        private void UpdateUrl()
        {
            var query = new Dictionary<string, object?>();
            if (!string.IsNullOrEmpty(searchQuery)) query["q"] = searchQuery;
            if (!string.IsNullOrEmpty(filterDept)) query["dept"] = filterDept;
            if (!string.IsNullOrEmpty(filterStatus)) query["status"] = filterStatus;
            if (filterWorkplace != "") query["workplace"] = filterWorkplace;
            if (currentPage > 1) query["page"] = currentPage;
            if (sortBy != "name") query["sortBy"] = sortBy;
            if (sortDesc) query["sortDesc"] = sortDesc;
            if (_viewMode != "table") query["viewMode"] = _viewMode;

            var url = Nav.GetUriWithQueryParameters(query);
            Nav.NavigateTo(url);
        }

        private async Task ResetFilters() {
            searchQuery = "";
            filterDept = "";
            filterStatus = "";
            filterWorkplace = "";
            currentPage = 1;
            sortBy = "name";
            sortDesc = false;
            selectedIds.Clear();
            UpdateUrl();
        }

        private async Task ToggleSort(string field)
        {
            if (sortBy == field)
            {
                sortDesc = !sortDesc;
            }
            else
            {
                sortBy = field;
                sortDesc = false;
            }
            currentPage = 1;
            UpdateUrl();
        }

        private void ChangeStatusFilter(string status)
        {
            filterStatus = status;
            currentPage = 1;
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
                foreach (var emp in FilteredEmployees) {
                    if (!string.IsNullOrEmpty(emp.Id)) selectedIds.Add(emp.Id);
                }
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
            try 
            {
                _ = JSRuntime.InvokeVoidAsync("Swal.fire", new {
                    title = "Đang trích xuất dữ liệu",
                    text = "Vui lòng chờ trong giây lát...",
                    allowOutsideClick = false,
                    showConfirmButton = false
                });
                try { _ = JSRuntime.InvokeVoidAsync("Swal.showLoading"); } catch { }

                var departmentId = CachedDepartments.FirstOrDefault(d => d.Name == filterDept)?.Id;
                var fileBytes = await EmployeeService.ExportEmployeesAsync(searchQuery, departmentId, filterStatus, filterWorkplace);

                await JSRuntime.InvokeVoidAsync("Swal.close");

                if (fileBytes != null && fileBytes.Length > 0)
                {
                    var fileName = $"TTL_DanhSachNhanVien_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                    await JSRuntime.InvokeVoidAsync("LayoutHelper.downloadFile", fileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileBytes);
                    
                    await JSRuntime.InvokeVoidAsync("toastr.success", "Đã xuất file thành công!");
                }
                else 
                {
                    await JSRuntime.InvokeVoidAsync("Swal.fire", "Thông báo", "Không thể trích xuất dữ liệu. Vui lòng thử lại sau.", "error");
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("Swal.fire", "Lỗi", "Lỗi hệ thống khi xuất file: " + ex.Message, "error");
            }
        }

        private async Task SaveChanges(EmployeeModel? emp = null) {
            var target = emp ?? selectedEmployee;
            if (target == null) return;

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
                if (!selectedEmployee.StatusId.HasValue)
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
                    StatusId = selectedEmployee.StatusId ?? CachedStatuses.FirstOrDefault()?.LookupID,
                    ContractTypeId = selectedEmployee.ContractTypeId ?? CachedContractTypes.FirstOrDefault()?.LookupID,
                    JoinDate = selectedEmployee.JoinDate ?? DateTime.UtcNow,
                    Salary = ParseSalary(selectedEmployee.SalaryDisplay),
                    ContractEndDate = selectedEmployee.ContractExpiry ?? selectedEmployee.ContractEndDate,
                    WorkplaceId = selectedEmployee.WorkplaceId,
                    IsAccountActive = selectedEmployee.IsActive,
                    PersonalDetails = new PersonalDetailsUpdateDto
                    {
                        DOB = selectedEmployee.DOB,
                        GenderId = selectedEmployee.GenderId,
                        Gender = selectedEmployee.Gender,
                        Address = selectedEmployee.Address,
                        Hometown = selectedEmployee.Hometown,
                        IdCardNumber = selectedEmployee.IdCard,
                        IdCardPlace = selectedEmployee.CccdIssuePlace,
                        IdCardIssueDate = selectedEmployee.CccdIssueDate,
                        TaxCode = selectedEmployee.TaxId,
                        BankAccount = selectedEmployee.BankAccountNumber,
                        BankName = selectedEmployee.BankName,
                        NationalityId = selectedEmployee.NationalityId,
                        Nationality = string.IsNullOrEmpty(selectedEmployee.Nationality) ? "Việt Nam" : selectedEmployee.Nationality,
                        EthnicityId = selectedEmployee.EthnicityId,
                        Ethnicity = string.IsNullOrEmpty(selectedEmployee.Ethnicity) ? "Kinh" : selectedEmployee.Ethnicity,
                        ReligionId = selectedEmployee.ReligionId,
                        Religion = string.IsNullOrEmpty(selectedEmployee.Religion) ? "Không" : selectedEmployee.Religion,
                        MaritalStatusId = selectedEmployee.MaritalStatusId,
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
                        targetEmp.StatusName = CachedStatuses.FirstOrDefault(s => s.LookupID == updateRequest.StatusId)?.Name ?? "Chưa xác định";
                        targetEmp.Dept = CachedDepartments.FirstOrDefault(d => d.Id == updateRequest.DepartmentId)?.Name ?? "Chưa xác định";
                        targetEmp.Role = CachedPositions.FirstOrDefault(p => p.Id == updateRequest.PositionId)?.Name ?? "Chưa xác định";
                        targetEmp.Avatar = updateRequest.AvatarUrl;
                        targetEmp.AvatarUrl = updateRequest.AvatarUrl;
                        targetEmp.WorkplaceId = updateRequest.WorkplaceId;
                        targetEmp.Workplace = CachedWorkplaces.FirstOrDefault(w => w.LookupID == updateRequest.WorkplaceId)?.Name ?? "Chưa xác định";
                    }

                    // Also update the currently selected employee model to reflect changes in the UI
                    selectedEmployee.FullName = updateRequest.FullName;
                    selectedEmployee.Name = updateRequest.FullName; 
                    selectedEmployee.Email = updateRequest.Email;
                    selectedEmployee.Phone = updateRequest.Phone;
                    selectedEmployee.DepartmentId = updateRequest.DepartmentId;
                    selectedEmployee.Dept = CachedDepartments.FirstOrDefault(d => d.Id == updateRequest.DepartmentId)?.Name ?? "Chưa xác định";
                    selectedEmployee.PositionId = updateRequest.PositionId;
                    selectedEmployee.Role = CachedPositions.FirstOrDefault(p => p.Id == updateRequest.PositionId)?.Name ?? "Chưa xác định";
                    selectedEmployee.StatusId = updateRequest.StatusId;
                    selectedEmployee.StatusName = CachedStatuses.FirstOrDefault(s => s.LookupID == updateRequest.StatusId)?.Name ?? "Chưa xác định";
                    selectedEmployee.ContractTypeId = updateRequest.ContractTypeId;
                    selectedEmployee.ContractTypeName = CachedContractTypes.FirstOrDefault(c => c.LookupID == updateRequest.ContractTypeId)?.Name ?? "Chưa xác định";
                    selectedEmployee.Avatar = updateRequest.AvatarUrl;
                    selectedEmployee.AvatarUrl = updateRequest.AvatarUrl;
                    selectedEmployee.WorkplaceId = updateRequest.WorkplaceId;
                    selectedEmployee.Workplace = CachedWorkplaces.FirstOrDefault(w => w.LookupID == updateRequest.WorkplaceId)?.Name ?? "Chưa xác định";
                    
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
            var target = selectedEmployee;
            if (target == null) return;

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
            _isReadOnlyDetail = true;
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
                    
                    if (selectedEmployee.Salary.HasValue)
                        selectedEmployee.SalaryDisplay = selectedEmployee.Salary.Value.ToString("N0");

                    // Ensure ID fields are consistent for Dropdown binding
                    if (string.IsNullOrEmpty(selectedEmployee.DepartmentId) && !string.IsNullOrEmpty(selectedEmployee.DeptId))
                        selectedEmployee.DepartmentId = selectedEmployee.DeptId;
                    else if (!string.IsNullOrEmpty(selectedEmployee.DepartmentId))
                        selectedEmployee.DeptId = selectedEmployee.DepartmentId;

                    if (string.IsNullOrEmpty(selectedEmployee.PositionId) && !string.IsNullOrEmpty(emp.PositionId))
                        selectedEmployee.PositionId = emp.PositionId;

                    if (string.IsNullOrEmpty(selectedEmployee.Dept)) selectedEmployee.Dept = emp.Dept;
                    if (string.IsNullOrEmpty(selectedEmployee.Role)) selectedEmployee.Role = emp.Role;
                    
                    if (!selectedEmployee.StatusId.HasValue) selectedEmployee.StatusId = emp.StatusId;
                    if (!selectedEmployee.ContractTypeId.HasValue) selectedEmployee.ContractTypeId = emp.ContractTypeId;
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

        private async Task PrintContract(EmployeeModel? emp = null)
        {
            var target = emp ?? selectedEmployee;
            if (target == null) return;

            _isPrintLoading = true;
            try
            {
                // 1. Fetch templates for this type
                var templates = await ContractService.GetTemplatesAsync(1, 10, typeId: target.ContractTypeId);
                
                // Pick active one or just the first one if filtered by type
                var templateBase = templates.Items.FirstOrDefault(t => t.StatusName == "Đang sử dụng" || t.StatusName == "Active") 
                               ?? templates.Items.FirstOrDefault();

                if (templateBase == null)
                {
                    await JSRuntime.InvokeVoidAsync("Swal.fire", "Thông báo", "Không tìm thấy mẫu hợp đồng phù hợp cho loại hợp đồng này. Vui lòng kiểm tra cấu hình mẫu hợp đồng.", "warning");
                    return;
                }

                // 2. Fetch FULL CONTENT (CRITICAL: List API doesn't include content)
                var template = await ContractService.GetTemplateAsync(templateBase.Id);
                if (template == null || string.IsNullOrEmpty(template.ContentHtml))
                {
                     await JSRuntime.InvokeVoidAsync("Swal.fire", "Thông báo", "Mẫu hợp đồng không có nội dung HTML. Vui lòng kiểm tra lại thiết lập mẫu.", "warning");
                     return;
                }

                // 3. Fetch Real API/DB Data (Direct Integration)
                var settings = await SettingsService.GetSettingsAsync();
                
                // Try to get latest active contract record for this employee
                var empContracts = await ContractService.GetEmployeeContractsAsync(1, 1, employeeId: target.Id, status: "Active");
                var latestContract = empContracts.Items.OrderByDescending(c => c.CreatedAt).FirstOrDefault();

                // 4. Prepare data for replacement
                var data = new Dictionary<string, string>
                {
                    {"Ten_Cong_Ty", settings?.CompanyName ?? "CÔNG TY TNHH TÂN TẤN LỘC"},
                    {"Dia_Chi_Cong_Ty", settings?.CompanyAddress ?? "Số 123, Đường ABC, Quận XYZ, TP.HCM"},
                    {"SDT_Cong_Ty", settings?.CompanyPhone ?? "028.1234.5678"},
                    {"MST_Cong_Ty", settings?.TaxCode ?? "0312345678"},
                    {"STK_Cong_Ty", "123456789 Tại VCB - TP.HCM"},
                    {"Nguoi_Dai_Dien", settings?.ContactPersonName ?? "TRẦN VƯƠNG KIM MY"},
                    {"Chuc_Vu_Nguoi_Dai_Dien", "Giám Đốc"},
                    {"Ten_Nhan_Vien", target.FullName?.ToUpper() ?? ""},
                    {"Ma_Nhan_Vien", target.Code},
                    {"Ngay_Sinh", target.DOB?.ToString("dd/MM/yyyy") ?? ""},
                    {"Quoc_Tich", target.Nationality ?? "Việt Nam"},
                    {"Nghe_Nghiep", target.Role},
                    {"Dia_Chi_Thuong_Tru", target.Address},
                    {"So_CCCD", target.IdCard},
                    {"So_So_Lao_Dong", ""},
                    {"Ma_Hop_Dong", latestContract?.ContractNumber ?? $"HĐ/{DateTime.Now.Year}/{target.Code}"},
                    {"Loai_Hop_Dong", target.ContractTypeName},
                    {"Thoi_Han_Hop_Dong", "12"},
                    {"Ngay_Bat_Dau", (latestContract?.StartDate ?? target.JoinDate)?.ToString("dd/MM/yyyy") ?? ""},
                    {"Ngay_Ket_Thuc", (latestContract?.EndDate ?? target.ContractEndDate)?.ToString("dd/MM/yyyy") ?? ""},
                    {"Dia_Diem_Lam_Viec", target.Workplace ?? "TP.HCM"},
                    {"Phong_Ban", target.Dept},
                    {"Chuc_Vu", target.Role},
                    {"Thoi_Gian_Lam_Viec", "44 giờ/tuần (Thứ 2 - Thứ 7)"},
                    {"Ngay_Ky", DateTime.Now.ToString("dd")},
                    {"Thang_Ky", DateTime.Now.ToString("MM")},
                    {"Nam_Ky", DateTime.Now.ToString("yyyy")},
                    {"Dia_Diem_Ky", "TP.HCM"},
                    {"Muc_Luong", (latestContract?.BasicSalary ?? target.Salary)?.ToString("N0") ?? "0"},
                    {"Phu_Cap", (latestContract?.AllowanceTotal ?? 0).ToString("N0")},
                    {"FullName", target.FullName},
                    {"Code", target.Code},
                    {"Today", DateTime.Now.ToString("dd/MM/yyyy")}
                };

                // 5. Replace variables in HTML
                string processedHtml = template.ContentHtml;
                foreach (var item in data)
                {
                    var variablePlaceholder = "{{" + item.Key + "}}";
                    var value = item.Value ?? "";

                    // Support both Metronic badge style and plain placeholders
                    string regexPattern = @"<span[^>]*>\s*{{\s*" + item.Key + @"\s*}}\s*</span>";
                    processedHtml = System.Text.RegularExpressions.Regex.Replace(processedHtml, regexPattern, $"<strong>{value}</strong>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    processedHtml = processedHtml.Replace(variablePlaceholder, $"<strong>{value}</strong>");
                }

                PreviewContent = processedHtml;
                StateHasChanged();

                // 6. Show modal
                await JSRuntime.InvokeVoidAsync("showModal", "#preview_modal");
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("Swal.fire", "Lỗi", $"Không thể tạo bản in: {ex.Message}", "error");
            }
            finally
            {
                _isPrintLoading = false;
            }
        }

        private async Task ViewCurrentContract(EmployeeModel? emp = null)
        {
            var target = emp ?? selectedEmployee;
            if (target == null) return;

            // Check if there are any uploaded signed contracts first
            var profile = await EmployeeService.GetDigitalProfileAsync(target.Id);
            var signedContract = profile?.Documents?.FirstOrDefault(d => 
                (d.DocumentType.Contains("Contract") || d.DocumentName.Contains("Hợp đồng")) && 
                !string.IsNullOrEmpty(d.FileUrl));

            if (signedContract != null)
            {
                // Open the signed file
                await JSRuntime.InvokeVoidAsync("open", signedContract.FileUrl, "_blank");
            }
            else
            {
                // Fallback to viewing generated preview if no signed contract exists
                await PrintContract(target);
            }
        }


        private string GetInitials(string? name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "??";
            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
            {
                return (parts[0][0].ToString() + parts[parts.Length - 1][0].ToString()).ToUpper();
            }
            return name.Length >= 2 ? name.Substring(0, 2).ToUpper() : name.ToUpper();
        }

        private long FromRecord() => totalCount == 0 ? 0 : (currentPage - 1) * pageSize + 1;
        private long ToRecord() => Math.Min(currentPage * (long)pageSize, totalCount);

        private async Task DownloadProfile(EmployeeModel emp)
        {
            if (emp == null) return;
            
            try
            {
                var pdfBytes = await PdfService.GenerateEmployeeProfilePdfAsync(emp);
                var fileName = $"Profile_{emp.Code}_{emp.FullName}.pdf";
                
                await JSRuntime.InvokeVoidAsync("LayoutHelper.downloadFile", fileName, "application/pdf", Convert.ToBase64String(pdfBytes));
                await JSRuntime.InvokeVoidAsync("toastr.success", "Đã khởi tạo file PDF hồ sơ nhân viên.");
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("toastr.error", $"Lỗi tạo PDF: {ex.Message}");
            }
        }
    }
}
