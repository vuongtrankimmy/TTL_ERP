using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.HumanResource.Models;
using TTL.HR.Application.Modules.Organization.Models;
using TTL.HR.Shared.Components.Common;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Organization.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Entities = TTL.HR.Application.Modules.HumanResource.Entities;
using Microsoft.AspNetCore.Components.Forms;

namespace TTL.HR.Shared.Pages.Employees
{
    public partial class EmployeeAdd
    {
        [Inject] public IMasterDataService MasterDataService { get; set; } = default!;
        [Inject] public IDepartmentService DepartmentService { get; set; } = default!;
        [Inject] public IPositionService PositionService { get; set; } = default!;
        [Inject] public IEmployeeService EmployeeService { get; set; } = default!;
        [Inject] public IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] public NavigationManager Navigation { get; set; } = default!;
        [Inject] public IPermissionService PermissionService { get; set; } = default!;
        [Inject] public IFormatService FormatService { get; set; } = default!;
        [Inject] public IAuthService AuthService { get; set; } = default!;

        [Parameter] public string? Id { get; set; }
        public bool IsEditMode => !string.IsNullOrEmpty(Id);
        private bool IsEmailValid => !string.IsNullOrEmpty(newEmployee.Email) && FormatService.IsValidEmail(newEmployee.Email);

        private List<LookupModel> genderLookups = new();
        private List<LookupModel> maritalStatusLookups = new();
        private List<LookupModel> employeeStatusLookups = new();
        private List<LookupModel> contractTypeLookups = new();
        private List<LookupModel> workplaceLookups = new();
        private List<DepartmentModel> departments = new();
        private List<PositionModel> positions = new();
        private List<EmployeeDto> allEmployees = new();
        private List<RoleModel> availableRoles = new();
        private bool _isProcessing = false;

        private EmployeeModel newEmployee = new()
        {
            IsActive = true,
            JoinDate = DateTime.Now,
            DOB = new DateTime(1995, 1, 1)
        };

        private List<EmployeeDocumentModel> uploadedDocuments = new();

        protected override async Task OnInitializedAsync()
        {
            genderLookups = await MasterDataService.GetCachedLookupsAsync("Gender");
            maritalStatusLookups = await MasterDataService.GetCachedLookupsAsync("MaritalStatus");
            employeeStatusLookups = await MasterDataService.GetCachedLookupsAsync("EmployeeStatus");
            contractTypeLookups = await MasterDataService.GetCachedLookupsAsync("ContractType");
            workplaceLookups = await MasterDataService.GetCachedLookupsAsync("Workplace");
            departments = await DepartmentService.GetDepartmentsAsync();
            positions = await PositionService.GetPositionsAsync();
            availableRoles = await PermissionService.GetRolesAsync();
            var employees = await EmployeeService.GetEmployeesAsync();
            allEmployees = employees;

            if (IsEditMode)
            {
                var employee = await EmployeeService.GetEmployeeAsync(Id);
                if (employee != null)
                {
                    newEmployee = employee;
                    
                    // Map Backend/Compatibility properties
                    if (string.IsNullOrEmpty(newEmployee.Name) && !string.IsNullOrEmpty(newEmployee.FullName))
                        newEmployee.Name = newEmployee.FullName;
                    
                    if (string.IsNullOrEmpty(newEmployee.DeptId) && !string.IsNullOrEmpty(newEmployee.DepartmentId))
                        newEmployee.DeptId = newEmployee.DepartmentId;
                    
                    if (string.IsNullOrEmpty(newEmployee.Avatar) && !string.IsNullOrEmpty(newEmployee.AvatarUrl))
                        newEmployee.Avatar = newEmployee.AvatarUrl;

                    if (newEmployee.PersonalDetails != null)
                    {
                        if (newEmployee.DOB == null) newEmployee.DOB = newEmployee.PersonalDetails.DOB;
                        if (string.IsNullOrEmpty(newEmployee.Gender)) newEmployee.Gender = newEmployee.PersonalDetails.Gender;
                        if (string.IsNullOrEmpty(newEmployee.Address)) newEmployee.Address = newEmployee.PersonalDetails.Address;
                        if (string.IsNullOrEmpty(newEmployee.Hometown)) newEmployee.Hometown = newEmployee.PersonalDetails.Hometown;
                        if (string.IsNullOrEmpty(newEmployee.IdCard)) newEmployee.IdCard = newEmployee.PersonalDetails.IdCardNumber;
                        if (newEmployee.CccdIssueDate == null) newEmployee.CccdIssueDate = newEmployee.PersonalDetails.IdCardIssueDate;
                        if (string.IsNullOrEmpty(newEmployee.CccdIssuePlace)) newEmployee.CccdIssuePlace = newEmployee.PersonalDetails.IdCardPlace;
                        if (string.IsNullOrEmpty(newEmployee.Nationality)) newEmployee.Nationality = newEmployee.PersonalDetails.Nationality;
                        if (string.IsNullOrEmpty(newEmployee.Ethnicity)) newEmployee.Ethnicity = newEmployee.PersonalDetails.Ethnicity;
                        if (string.IsNullOrEmpty(newEmployee.Religion)) newEmployee.Religion = newEmployee.PersonalDetails.Religion;
                        if (string.IsNullOrEmpty(newEmployee.MaritalStatus)) newEmployee.MaritalStatus = newEmployee.PersonalDetails.MaritalStatus;
                        if (string.IsNullOrEmpty(newEmployee.PlaceOfOrigin)) newEmployee.PlaceOfOrigin = newEmployee.PersonalDetails.PlaceOfOrigin;
                        if (string.IsNullOrEmpty(newEmployee.Residence)) newEmployee.Residence = newEmployee.PersonalDetails.Residence;
                        if (string.IsNullOrEmpty(newEmployee.SocialInsuranceId)) newEmployee.SocialInsuranceId = newEmployee.PersonalDetails.SocialInsuranceId;
                        if (string.IsNullOrEmpty(newEmployee.TaxId)) newEmployee.TaxId = newEmployee.PersonalDetails.TaxCode;
                        if (string.IsNullOrEmpty(newEmployee.BankAccountNumber)) newEmployee.BankAccountNumber = newEmployee.PersonalDetails.BankAccount;
                        if (string.IsNullOrEmpty(newEmployee.BankName)) newEmployee.BankName = newEmployee.PersonalDetails.BankName;
                    }

                    // The Salary might be mapped to SalaryDisplay, so ensure compatibility
                    if (newEmployee.Salary.HasValue && string.IsNullOrEmpty(newEmployee.SalaryDisplay))
                    {
                        newEmployee.SalaryDisplay = newEmployee.Salary.Value.ToString("N0");
                    }
                    
                    // Account state initialization
                    newEmployee.Username = employee.Username;
                    newEmployee.IsCreateAccount = employee.IsCreateAccount;
                    newEmployee.IsAccountActive = employee.IsAccountActive;
                    newEmployee.IsActive = employee.IsAccountActive; // Primary toggle in UI

                    if (employee.Roles != null && employee.Roles.Any())
                    {
                        var firstRole = employee.Roles.First();
                        newEmployee.Role = firstRole switch
                        {
                            "65fc5b5b0000000000000001" or "ADMIN" => "Admin",
                            "65fc5b5b0000000000000002" or "HR_MGR" => "HR",
                            "65fc5b5b0000000000000005" or "DEPT_MGR" => "Manager",
                            "65fc5b5b0000000000000004" or "EMPLOYEE" => "User",
                            _ => firstRole
                        };
                    }
                    else if (string.IsNullOrEmpty(newEmployee.Role))
                    {
                        newEmployee.Role = "User";
                    }

                    // Load Digital Profile (Documents)
                    var profile = await EmployeeService.GetDigitalProfileAsync(Id!);
                    if (profile != null)
                    {
                        uploadedDocuments = profile.Documents ?? new List<EmployeeDocumentModel>();
                    }
                }
                else
                {
                    await JSRuntime.InvokeVoidAsync("Swal.fire", "Lỗi", "Không tìm thấy hồ sơ nhân viên này", "error");
                    Navigation.NavigateTo("/employees");
                }
            }
            else
            {
                // Ensure lookups are not null
                if (genderLookups == null) genderLookups = new();
                if (employeeStatusLookups == null) employeeStatusLookups = new();
                if (contractTypeLookups == null) contractTypeLookups = new();
                if (departments == null) departments = new();
                if (positions == null) positions = new();
                
                // Set defaults if lists are not empty
                if (string.IsNullOrEmpty(newEmployee.Gender)) newEmployee.Gender = genderLookups.FirstOrDefault()?.Name ?? "Nam";
                if (!newEmployee.StatusId.HasValue || newEmployee.StatusId <= 0) newEmployee.StatusId = employeeStatusLookups.FirstOrDefault()?.LookupID;
                if (!newEmployee.ContractTypeId.HasValue) newEmployee.ContractTypeId = contractTypeLookups.FirstOrDefault()?.LookupID;
                if (string.IsNullOrEmpty(newEmployee.DeptId)) newEmployee.DeptId = departments.FirstOrDefault()?.Id ?? string.Empty;
                if (string.IsNullOrEmpty(newEmployee.PositionId)) newEmployee.PositionId = positions.FirstOrDefault()?.Id ?? string.Empty;
                if (string.IsNullOrEmpty(newEmployee.Workplace)) newEmployee.Workplace = workplaceLookups.FirstOrDefault()?.Name ?? "Văn phòng Hồ Chí Minh";
                
                // Defaults for Account
                if (string.IsNullOrEmpty(newEmployee.Role)) newEmployee.Role = "User";
            }
        }

        private CccdScanner cccdScanner;

        private async Task ScanCCCD()
        {
            var scannedData = await cccdScanner.ScanAsync();
            
            if (scannedData != null)
            {
                newEmployee.Name = scannedData.Name;
                newEmployee.Code = "NV" + new Random().Next(100, 999);
                newEmployee.IdCard = scannedData.IdCard;
                newEmployee.DOB = scannedData.DOB ?? DateTime.Now;
                newEmployee.Address = scannedData.Address;
                newEmployee.Hometown = scannedData.Hometown;
                newEmployee.Gender = scannedData.Gender;
                newEmployee.Nationality = scannedData.Nationality;
                newEmployee.Ethnicity = scannedData.Ethnicity;
                newEmployee.Religion = scannedData.Religion;
                newEmployee.CccdIssueDate = DateTime.TryParse(scannedData.IssueDate, out var iDate) ? iDate : null;
                newEmployee.CccdIssuePlace = scannedData.IssuePlace;
                newEmployee.PlaceOfOrigin = scannedData.Hometown;
                newEmployee.Residence = scannedData.Address;
                
                // Tự sinh email và điện thoại demo
                newEmployee.Name = FormatService.FormatFullName(scannedData.Name);
                newEmployee.Email = FormatService.FormatEmail(newEmployee.Name.Replace(" ", ".")) + "@gmail.com";
                
                var rawPhone = "0" + new Random().Next(900000000, 999999999).ToString();
                newEmployee.Phone = FormatService.FormatPhone(rawPhone);
                
                // Pre-fill username from name (normalized)
                if (string.IsNullOrEmpty(newEmployee.Username))
                {
                    newEmployee.Username = FormatService.FormatEmail(newEmployee.Name.Replace(" ", ""));
                }
                
                await JSRuntime.InvokeVoidAsync("Swal.fire", new {
                    title = "Thành công!",
                    text = "Dữ liệu đã được điền tự động vào biểu mẫu.",
                    icon = "success",
                    timer = 1500,
                    showConfirmButton = false
                });
                
                StateHasChanged();
            }
        }

        private void GenerateUsername()
        {
            var username = FormatService.GenerateDefaultUsername(newEmployee.Phone, newEmployee.Email, newEmployee.IdCard);
            if (!string.IsNullOrEmpty(username))
            {
                newEmployee.Username = username;
                StateHasChanged();
                _ = JSRuntime.InvokeVoidAsync("toastr.info", $"Đã tạo tên đăng nhập: {username}");
            }
            else
            {
                _ = JSRuntime.InvokeVoidAsync("toastr.warning", "Vui lòng nhập Số điện thoại, Email hoặc CCCD để tự động tạo Tên đăng nhập.");
            }
        }

        private async Task RequestPasswordReset()
        {
            if (string.IsNullOrEmpty(newEmployee.Email))
            {
                await JSRuntime.InvokeVoidAsync("toastr.warning", "Nhân viên chưa có Email để nhận link đặt lại mật khẩu.");
                return;
            }

            var confirmElement = await JSRuntime.InvokeAsync<System.Text.Json.JsonElement>("Swal.fire", new
            {
                title = "Xác nhận",
                text = $"Hệ thống sẽ gửi link đặt lại mật khẩu tới email: {newEmployee.Email}. Bạn có chắc chắn muốn thực hiện?",
                icon = "question",
                showCancelButton = true,
                confirmButtonText = "Đồng ý, gửi ngay!",
                cancelButtonText = "Hủy"
            });

            if (confirmElement.TryGetProperty("isConfirmed", out var isConfirmed) && isConfirmed.GetBoolean())
            {
                _isProcessing = true;
                StateHasChanged();

                try
                {
                    var result = await AuthService.RequestPasswordResetAsync(newEmployee.Email);
                    if (result.Success)
                    {
                        await JSRuntime.InvokeVoidAsync("Swal.fire", "Thành công", "Link đặt lại mật khẩu đã được gửi.", "success");
                    }
                    else
                    {
                        await JSRuntime.InvokeVoidAsync("Swal.fire", "Thất bại", result.Message, "error");
                    }
                }
                catch (Exception ex)
                {
                    await JSRuntime.InvokeVoidAsync("Swal.fire", "Lỗi", ex.Message, "error");
                }
                finally
                {
                    _isProcessing = false;
                    StateHasChanged();
                }
            }
        }

        private async Task UploadAvatar(InputFileChangeEventArgs e)
        {
            var file = e.File;
            if (file == null) return;

            // Simple validation
            if (file.Size > 2 * 1024 * 1024) // 2MB
            {
                await JSRuntime.InvokeVoidAsync("toastr.error", "Ảnh quá lớn. Vui lòng chọn ảnh dưới 2MB.");
                return;
            }

            try
            {
                _isProcessing = true;
                StateHasChanged();

                // 1. Local Preview
                var buffer = new byte[file.Size];
                await file.OpenReadStream(2 * 1024 * 1024).ReadAsync(buffer);
                var base64 = Convert.ToBase64String(buffer);
                newEmployee.Avatar = $"data:{file.ContentType};base64,{base64}";
                
                // 2. Upload to Server
                if (IsEditMode)
                {
                    using var stream = file.OpenReadStream(2 * 1024 * 1024);
                    var result = await EmployeeService.UploadDocumentAsync(Id!, "Avatar", stream, file.Name, null, "Cập nhật ảnh đại diện");
                    
                    if (string.IsNullOrEmpty(result)) // Null means success in this specialized API
                    {
                        await JSRuntime.InvokeVoidAsync("toastr.success", "Đã cập nhật ảnh đại diện thành công!");
                    }
                    else
                    {
                        await JSRuntime.InvokeVoidAsync("toastr.error", $"Lỗi tải ảnh: {result}");
                    }
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("toastr.error", $"Lỗi tải ảnh: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
                StateHasChanged();
            }
        }

        private async Task GetResidenceCoordinates()
        {
            if (string.IsNullOrWhiteSpace(newEmployee.Residence))
            {
                await JSRuntime.InvokeVoidAsync("toastr.warning", "Vui lòng nhập địa chỉ trước khi lấy tọa độ.");
                return;
            }

            try
            {
                var coords = await JSRuntime.InvokeAsync<Coordinates?>("LayoutHelper.getCoordinatesFromAddress", newEmployee.Residence);
                if (coords != null)
                {
                    newEmployee.Latitude = coords.Lat;
                    newEmployee.Longitude = coords.Lon;
                    StateHasChanged();
                    await JSRuntime.InvokeVoidAsync("toastr.success", "Đã lấy tọa độ thành công!");
                }
                else
                {
                    await JSRuntime.InvokeVoidAsync("toastr.error", "Không tìm thấy tọa độ cho địa chỉ này. Vui lòng kiểm tra lại.");
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("toastr.error", $"Lỗi lấy tọa độ: {ex.Message}");
            }
        }

        public class Coordinates
        {
            public double Lat { get; set; }
            public double Lon { get; set; }
        }

        private async Task UploadDocument(InputFileChangeEventArgs e, string documentType)
        {
            if (!IsEditMode)
            {
                await JSRuntime.InvokeVoidAsync("Swal.fire", "Thông báo", "Vui lòng lưu hồ sơ nhân viên trước khi tải lên tài liệu đính kèm.", "info");
                return;
            }

            var file = e.File;
            if (file == null) return;

            if (file.Size > 10 * 1024 * 1024) // 10MB as per UI
            {
                await JSRuntime.InvokeVoidAsync("toastr.error", "Tài liệu quá lớn. Vui lòng chọn file dưới 10MB.");
                return;
            }

            try
            {
                _isProcessing = true;
                StateHasChanged();

                using var stream = file.OpenReadStream(10 * 1024 * 1024);
                var result = await EmployeeService.UploadDocumentAsync(Id!, documentType, stream, file.Name, null, $"Tải lên {documentType}");
                
                if (string.IsNullOrEmpty(result))
                {
                    await JSRuntime.InvokeVoidAsync("toastr.success", $"Đã tải lên {documentType} thành công!");
                    
                    // Refresh documents list
                    var profile = await EmployeeService.GetDigitalProfileAsync(Id!);
                    if (profile != null)
                    {
                        uploadedDocuments = profile.Documents ?? new List<EmployeeDocumentModel>();
                    }
                }
                else
                {
                    await JSRuntime.InvokeVoidAsync("toastr.error", $"Lỗi tải tài liệu: {result}");
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("toastr.error", $"Lỗi tải tài liệu: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
                StateHasChanged();
            }
        }

        private void AddDependent()
        {
            if (newEmployee.PersonalDetails == null) newEmployee.PersonalDetails = new EmployeePersonalDetails();
            if (newEmployee.PersonalDetails.Dependents == null) newEmployee.PersonalDetails.Dependents = new List<DependentDetailDto>();
            newEmployee.PersonalDetails.Dependents.Add(new DependentDetailDto { IsEligibleForDeduction = true });
            StateHasChanged();
        }

        private void RemoveDependent(DependentDetailDto item)
        {
            newEmployee.PersonalDetails?.Dependents?.Remove(item);
            StateHasChanged();
        }

        private void AddEducation()
        {
            if (newEmployee.Education == null) newEmployee.Education = new List<EducationDetailDto>();
            newEmployee.Education.Add(new EducationDetailDto { StartYear = DateTime.Now.Year - 4, EndYear = DateTime.Now.Year });
            StateHasChanged();
        }

        private void RemoveEducation(EducationDetailDto item)
        {
            newEmployee.Education?.Remove(item);
            StateHasChanged();
        }

        private void AddExperience()
        {
            if (newEmployee.Experience == null) newEmployee.Experience = new List<ExperienceDetailDto>();
            newEmployee.Experience.Add(new ExperienceDetailDto { StartDate = DateTime.Now.AddYears(-2), EndDate = DateTime.Now });
            StateHasChanged();
        }

        private void RemoveExperience(ExperienceDetailDto item)
        {
            newEmployee.Experience?.Remove(item);
            StateHasChanged();
        }


        private async Task Submit()
        {
            // 1. Clean & Format Data
            newEmployee.Name = FormatService.FormatFullName(newEmployee.Name);
            newEmployee.IdCard = FormatService.FormatIdCard(newEmployee.IdCard);
            newEmployee.Email = FormatService.FormatEmail(newEmployee.Email);
            newEmployee.Phone = FormatService.CleanDigits(newEmployee.Phone);

            // 2. Strict Validation
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(newEmployee.Name)) errors.Add("Họ tên nhân viên");
            if (!string.IsNullOrWhiteSpace(newEmployee.Email) && !FormatService.IsValidEmail(newEmployee.Email)) 
            {
                errors.Add("Email không đúng định dạng (VD: example@mail.com)");
            }

            if (!string.IsNullOrWhiteSpace(newEmployee.Phone) && newEmployee.Phone.Length < 10) 
            {
                errors.Add("Số điện thoại phải từ 10 số");
            }

            if (!string.IsNullOrWhiteSpace(newEmployee.IdCard) && newEmployee.IdCard.Length != 12 && newEmployee.IdCard.Length != 9) 
            {
                errors.Add("Số CCCD phải đủ 12 chữ số (hoặc 9 số đối với CMND cũ)");
            }

            // Only require structural fields when CREATING, not when editing
            if (!IsEditMode)
            {
                if (string.IsNullOrEmpty(newEmployee.DeptId)) errors.Add("Phòng ban");
                if (string.IsNullOrEmpty(newEmployee.PositionId)) errors.Add("Chức vụ");
                if (!newEmployee.StatusId.HasValue || newEmployee.StatusId <= 0) errors.Add("Trạng thái");
                if (string.IsNullOrEmpty(newEmployee.Workplace)) errors.Add("Nơi làm việc");
            }
            
            if (!IsEmailValid)
            {
                newEmployee.IsCreateAccount = false;
            }

            if (newEmployee.IsCreateAccount)
            {
                if (string.IsNullOrWhiteSpace(newEmployee.Username))
                {
                    // If username is empty but toggle is on, try to auto-generate
                    newEmployee.Username = FormatService.GenerateDefaultUsername(newEmployee.Phone, newEmployee.Email, newEmployee.IdCard);
                }

                if (string.IsNullOrWhiteSpace(newEmployee.Username))
                {
                    errors.Add("Không thể tự động tạo Tên đăng nhập. Vui lòng nhập thủ công.");
                }
                else
                {
                    // Force normalization: lowercase, remove whitespace, remove unicode, etc.
                    var normalized = FormatService.NormalizeUsername(newEmployee.Username);
                    if (normalized != newEmployee.Username)
                    {
                        newEmployee.Username = normalized;
                    }
                    
                    if (string.IsNullOrEmpty(newEmployee.Username))
                    {
                        errors.Add("Tên đăng nhập không hợp lệ (phải chứa ít nhất 1 ký tự a-z hoặc 0-9)");
                    }
                }
            }

            if (errors.Any())
            {
                await JSRuntime.InvokeVoidAsync("Swal.fire", "Thông tin chưa hợp lệ", 
                    $"{string.Join("<br/>", errors.Select(e => "• " + e))}", "warning");
                return;
            }

            // 3. Format Phone for Display/Storage: 0xxx xxx xxx
            newEmployee.Phone = FormatService.FormatPhone(newEmployee.Phone);

            _isProcessing = true;
            StateHasChanged();

            // 4. Show Loading Animation - DON'T await this as it blocks the C# thread until the modal closes
            // and we only close it AFTER the API call completes, causing a deadlock.
            _ = JSRuntime.InvokeVoidAsync("Swal.fire", new
            {
                title = "Đang xử lý...",
                text = "Hệ thống đang khởi tạo hồ sơ nhân sự mới",
                allowOutsideClick = false,
                showConfirmButton = false
            });
            
            // Standard way to show loading, fire and forget
            try { _ = JSRuntime.InvokeVoidAsync("Swal.showLoading"); } catch { }

            try 
            {
                if (IsEditMode)
                {
                    var updateRequest = new UpdateEmployeeRequest
                    {
                        Id = Id!,
                        FullName = newEmployee.Name,
                        TimekeepingCode = newEmployee.TimekeepingCode,
                        Email = FormatService.FormatEmail(newEmployee.Email),
                        CompanyEmail = FormatService.FormatEmail(newEmployee.CompanyEmail),
                        Phone = FormatService.CleanDigits(newEmployee.Phone),
                        AvatarUrl = newEmployee.Avatar,
                        
                        DepartmentId = IsValidObjectId(newEmployee.DeptId) ? newEmployee.DeptId : null,
                        PositionId = IsValidObjectId(newEmployee.PositionId) ? newEmployee.PositionId : null,
                        ReportToId = IsValidObjectId(newEmployee.ReportToId) ? newEmployee.ReportToId : null,
                        StatusId = newEmployee.StatusId > 0 ? newEmployee.StatusId : null,
                        ContractTypeId = newEmployee.ContractTypeId,
                        
                        JoinDate = newEmployee.OfficialJoinDate ?? newEmployee.JoinDate ?? DateTime.Now,
                        Salary = ParseSalary(newEmployee.SalaryDisplay),
                        ContractEndDate = newEmployee.ContractExpiry ?? newEmployee.ContractEndDate,
                        Workplace = newEmployee.Workplace,
                        IsAccountActive = newEmployee.IsActive,
                        IsCreateAccount = newEmployee.IsCreateAccount,
                        Username = newEmployee.Username,
                        Password = newEmployee.Password,
                        Role = newEmployee.Role,
                        Roles = newEmployee.Roles ?? new List<string>(),
                        
                        PersonalDetails = new PersonalDetailsUpdateDto
                        {
                            DOB = newEmployee.DOB,
                            GenderId = newEmployee.GenderId,
                            Address = newEmployee.Address,
                            Hometown = newEmployee.Hometown,
                            IdCardNumber = FormatService.CleanDigits(newEmployee.IdCard),
                            IdCardIssueDate = newEmployee.CccdIssueDate,
                            IdCardPlace = newEmployee.CccdIssuePlace,
                            TaxCode = newEmployee.TaxId,
                            BankAccount = newEmployee.BankAccountNumber,
                            BankName = newEmployee.BankName,
                            NationalityId = newEmployee.NationalityId,
                            EthnicityId = newEmployee.EthnicityId,
                            ReligionId = newEmployee.ReligionId,
                            MaritalStatusId = newEmployee.MaritalStatusId,
                            PlaceOfOrigin = newEmployee.PlaceOfOrigin,
                            Residence = newEmployee.Residence,
                            SocialInsuranceId = newEmployee.SocialInsuranceId,
                            Latitude = newEmployee.Latitude,
                            Longitude = newEmployee.Longitude,
                            Dependents = newEmployee.PersonalDetails?.Dependents ?? new List<DependentDetailDto>()
                        },
                        EmergencyContact = new EmergencyContactUpdateDto
                        {
                            Name = newEmployee.EmergencyContactName,
                            Relation = newEmployee.EmergencyContactRelation,
                            Phone = FormatService.CleanDigits(newEmployee.EmergencyContactPhone)
                        },
                        Education = newEmployee.Education ?? new List<EducationDetailDto>(),
                        Experience = newEmployee.Experience ?? new List<ExperienceDetailDto>()
                    };
                    
                    Console.WriteLine($"[EmployeeAdd] UpdateRequest.IsCreateAccount={updateRequest.IsCreateAccount}, Username={updateRequest.Username}, Role={updateRequest.Role}");
                    Console.WriteLine($"[EmployeeAdd] Submitting update for employee ID: {Id} at {DateTime.Now:HH:mm:ss}");
                    var resultStr = await EmployeeService.UpdateEmployeeAsync(Id!, updateRequest);
                    
                    if (resultStr == null) // Service returns null on success for Update
                    {
                        await JSRuntime.InvokeVoidAsync("Swal.fire", new
                        {
                            title = "Thành công!",
                            text = $"Hồ sơ nhân viên {newEmployee.Name} đã được cập nhật.",
                            icon = "success",
                            confirmButtonText = "Hoàn tất"
                        });
                        Navigation.NavigateTo("/employees");
                    }
                    else
                    {
                        var errorMsg = !string.IsNullOrEmpty(resultStr) ? resultStr : "Không thể cập nhật hồ sơ. Vui lòng kiểm tra lại dữ liệu.";
                        if (errorMsg.Contains("<!DOCTYPE html>")) errorMsg = "Lỗi máy chủ (500 Internal Server Error).";
                        else if (errorMsg.Length > 500) errorMsg = errorMsg.Substring(0, 500) + "...";
                        await JSRuntime.InvokeVoidAsync("Swal.fire", "Thông báo", errorMsg, "error");
                    }
                }
                else
                {
                    // 5. Map Data to CreateEmployeeCommand structure (Backend expected format)
                    var createCommand = new CreateEmployeeCommandDto
                    {
                        FullName = newEmployee.Name,
                        TimekeepingCode = newEmployee.TimekeepingCode,
                        Email = FormatService.FormatEmail(newEmployee.Email),
                        CompanyEmail = FormatService.FormatEmail(newEmployee.CompanyEmail),
                        Phone = FormatService.CleanDigits(newEmployee.Phone),
                        AvatarUrl = newEmployee.Avatar,
                        
                        DepartmentId = IsValidObjectId(newEmployee.DeptId) ? newEmployee.DeptId : null,
                        PositionId = IsValidObjectId(newEmployee.PositionId) ? newEmployee.PositionId : null,
                        ReportToId = IsValidObjectId(newEmployee.ReportToId) ? newEmployee.ReportToId : null,
                        StatusId = newEmployee.StatusId > 0 ? newEmployee.StatusId : null,
                        ContractTypeId = newEmployee.ContractTypeId,
                        
                        JoinDate = newEmployee.OfficialJoinDate ?? newEmployee.JoinDate ?? DateTime.Now,
                        Salary = ParseSalary(newEmployee.SalaryDisplay),
                        ContractEndDate = newEmployee.ContractExpiry ?? newEmployee.ContractEndDate,
                        Workplace = newEmployee.Workplace,
                        
                        PersonalDetails = new PersonalDetailsCommandDto
                        {
                            DOB = newEmployee.DOB,
                            GenderId = newEmployee.GenderId,
                            Address = newEmployee.Address,
                            Hometown = newEmployee.Hometown,
                            IdCardNumber = FormatService.CleanDigits(newEmployee.IdCard),
                            IdCardIssueDate = newEmployee.CccdIssueDate,
                            IdCardPlace = newEmployee.CccdIssuePlace,
                            TaxCode = newEmployee.TaxId,
                            BankAccount = newEmployee.BankAccountNumber,
                            BankName = newEmployee.BankName,
                            NationalityId = newEmployee.NationalityId,
                            EthnicityId = newEmployee.EthnicityId,
                            ReligionId = newEmployee.ReligionId,
                            MaritalStatusId = newEmployee.MaritalStatusId,
                            PlaceOfOrigin = newEmployee.PlaceOfOrigin,
                            Residence = newEmployee.Residence,
                            SocialInsuranceId = newEmployee.SocialInsuranceId,
                            Latitude = newEmployee.Latitude,
                            Longitude = newEmployee.Longitude,
                            Dependents = newEmployee.PersonalDetails?.Dependents ?? new List<DependentDetailDto>()
                        },
                        
                        EmergencyContact = new EmergencyContactCommandDto
                        {
                            Name = newEmployee.EmergencyContactName,
                            Relation = newEmployee.EmergencyContactRelation,
                            Phone = FormatService.CleanDigits(newEmployee.EmergencyContactPhone)
                        },
                        Education = newEmployee.Education ?? new List<EducationDetailDto>(),
                        Experience = newEmployee.Experience ?? new List<ExperienceDetailDto>(),
                        
                        // Account Creation Fields
                        IsCreateAccount = newEmployee.IsCreateAccount,
                        Username = newEmployee.Username,
                        Password = newEmployee.Password,
                        Role = newEmployee.Role,
                        Roles = newEmployee.Roles ?? new List<string>(),
                        IsAccountActive = newEmployee.IsActive
                    };

                    // 4. Send API Request (We cast to object for serialization compatibility if needed, but CreateEmployeeAsync handles it)
                    Console.WriteLine($"[EmployeeAdd] Submitting employee: {newEmployee.Name} at {DateTime.Now:HH:mm:ss}");
                    var result = await EmployeeService.CreateEmployeeAsync(MapToLegacyEntity(createCommand));
                    Console.WriteLine($"[EmployeeAdd] API Response received: {result?.Substring(0, Math.Min(result?.Length ?? 0, 50))}");
                    
                    // 7. Handle Response
                    if (!string.IsNullOrEmpty(result) && IsValidObjectId(result))
                    {
                        await JSRuntime.InvokeVoidAsync("Swal.fire", new
                        {
                            title = "Thành công!",
                            text = $"Hồ sơ nhân viên {newEmployee.Name} ({newEmployee.Code}) đã được tạo.",
                            icon = "success",
                            confirmButtonText = "Hoàn tất"
                        });
                        Navigation.NavigateTo("/employees");
                    }
                    else 
                    {
                        var errorMsg = !string.IsNullOrEmpty(result) ? result : "Không thể lưu hồ sơ. Vui lòng kiểm tra lại dữ liệu hoặc thử lại.";
                        
                        // Simple HTML check
                        if (errorMsg.Contains("<!DOCTYPE html>") || errorMsg.Contains("<html"))
                        {
                            errorMsg = "Lỗi máy chủ (500 Internal Server Error). Vui lòng liên hệ bộ phận kỹ thuật.";
                        }
                        else if (errorMsg.Length > 500) 
                        {
                            errorMsg = errorMsg.Substring(0, 500) + "...";
                        }

                        await JSRuntime.InvokeVoidAsync("Swal.fire", "Thông báo", errorMsg, "error");
                    }
                }

            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("Swal.fire", "Lỗi gửi yêu cầu", ex.Message, "error");
            }
            finally
            {
                _isProcessing = false;
                StateHasChanged();
                // We DON'T call Swal.close here because it would close the Success/Error modals 
                // that were opened in the try/catch blocks. 
                // Success/Error modals from Swal.fire already replace the loading spinner.
            }
        }

        // Helper classes to ensure exact matching with Backend Commands
        private class CreateEmployeeCommandDto {
            public string FullName { get; set; } = string.Empty;
            public string TimekeepingCode { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string CompanyEmail { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
            public string AvatarUrl { get; set; } = string.Empty;
            public string? DepartmentId { get; set; }
            public string? PositionId { get; set; }
            public string? ReportToId { get; set; }
            public int? StatusId { get; set; }
            public int? ContractTypeId { get; set; }
            public DateTime JoinDate { get; set; }
            public decimal? Salary { get; set; }
            public DateTime? ContractEndDate { get; set; }
            public string Workplace { get; set; } = string.Empty;
            public PersonalDetailsCommandDto PersonalDetails { get; set; } = new();
            public EmergencyContactCommandDto EmergencyContact { get; set; } = new();
            public List<EducationDetailDto> Education { get; set; } = new();
            public List<ExperienceDetailDto> Experience { get; set; } = new();
            public bool IsCreateAccount { get; set; }
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
            public List<string> Roles { get; set; } = new();
            public bool IsAccountActive { get; set; }
        }

        private class PersonalDetailsCommandDto {
            public DateTime? DOB { get; set; }
            public int? GenderId { get; set; }
            public string Gender { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
            public string Hometown { get; set; } = string.Empty;
            public string IdCardNumber { get; set; } = string.Empty;
            public DateTime? IdCardIssueDate { get; set; }
            public string IdCardPlace { get; set; } = string.Empty;
            public string TaxCode { get; set; } = string.Empty;
            public string BankAccount { get; set; } = string.Empty;
            public string BankName { get; set; } = string.Empty;
            public int? NationalityId { get; set; }
            public string Nationality { get; set; } = "Việt Nam";
            public int? EthnicityId { get; set; }
            public string Ethnicity { get; set; } = "Kinh";
            public int? ReligionId { get; set; }
            public string Religion { get; set; } = "Không";
            public int? MaritalStatusId { get; set; }
            public string MaritalStatus { get; set; } = "Độc thân";
            public string PlaceOfOrigin { get; set; } = string.Empty;
            public string Residence { get; set; } = string.Empty;
            public string SocialInsuranceId { get; set; } = string.Empty;
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public List<DependentDetailDto> Dependents { get; set; } = new();
        }

        private class EmergencyContactCommandDto {
            public string Name { get; set; } = string.Empty;
            public string Relation { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
        }

        // Helper to convert to the type expected by IEmployeeService (Entities.Employee) 
        // using simple reflection or manual mapping to maintain backward compatibility with the service interface
        private Entities.Employee MapToLegacyEntity(CreateEmployeeCommandDto dto)
        {
            return new Entities.Employee
            {
                // Code is generated by the service, not passed from here
                FullName = dto.FullName,
                TimekeepingCode = dto.TimekeepingCode,
                Email = dto.Email,
                CompanyEmail = dto.CompanyEmail,
                Phone = dto.Phone,
                AvatarUrl = dto.AvatarUrl,
                DepartmentId = IsValidObjectId(dto.DepartmentId) ? dto.DepartmentId : null,
                PositionId = IsValidObjectId(dto.PositionId) ? dto.PositionId : null,
                ReportToId = IsValidObjectId(dto.ReportToId) ? dto.ReportToId : null,
                StatusId = dto.StatusId,
                ContractTypeId = dto.ContractTypeId,
                JoinDate = dto.JoinDate,
                Salary = dto.Salary,
                ContractEndDate = dto.ContractEndDate,
                Workplace = dto.Workplace,
                Education = dto.Education ?? new List<EducationDetailDto>(),
                Experience = dto.Experience ?? new List<ExperienceDetailDto>(),
                // IsAccountActive is not part of the DTO, assuming default or handled by service
                
                PersonalDetails = new Entities.PersonalInfo
                {
                    DOB = dto.PersonalDetails.DOB,
                    GenderId = dto.PersonalDetails.GenderId,
                    Gender = dto.PersonalDetails.Gender,
                    Address = dto.PersonalDetails.Address,
                    Hometown = dto.PersonalDetails.Hometown,
                    IdCardNumber = dto.PersonalDetails.IdCardNumber,
                    IdCardIssueDate = dto.PersonalDetails.IdCardIssueDate,
                    IdCardPlace = dto.PersonalDetails.IdCardPlace,
                    TaxCode = dto.PersonalDetails.TaxCode,
                    BankAccount = dto.PersonalDetails.BankAccount,
                    BankName = dto.PersonalDetails.BankName,
                    NationalityId = dto.PersonalDetails.NationalityId,
                    Nationality = dto.PersonalDetails.Nationality,
                    EthnicityId = dto.PersonalDetails.EthnicityId,
                    Ethnicity = dto.PersonalDetails.Ethnicity,
                    ReligionId = dto.PersonalDetails.ReligionId,
                    Religion = dto.PersonalDetails.Religion,
                    MaritalStatusId = dto.PersonalDetails.MaritalStatusId,
                    MaritalStatus = dto.PersonalDetails.MaritalStatus,
                    PlaceOfOrigin = dto.PersonalDetails.PlaceOfOrigin,
                    Residence = dto.PersonalDetails.Residence,
                    SocialInsuranceId = dto.PersonalDetails.SocialInsuranceId,
                    Latitude = dto.PersonalDetails.Latitude,
                    Longitude = dto.PersonalDetails.Longitude,
                    Dependents = dto.PersonalDetails.Dependents ?? new List<DependentDetailDto>()
                },
                EmergencyContact = new Entities.EmergencyContact
                {
                    Name = dto.EmergencyContact.Name,
                    Relation = dto.EmergencyContact.Relation,
                    Phone = dto.EmergencyContact.Phone
                },
                IsCreateAccount = dto.IsCreateAccount,
                Username = dto.Username,
                Password = dto.Password,
                Role = dto.Role,
                Roles = dto.Roles ?? new List<string>(),
                IsAccountActive = dto.IsAccountActive
            };
        }

        private bool IsValidObjectId(string? id)
        {
            if (string.IsNullOrWhiteSpace(id)) return false;
            if (id.Length != 24) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(id, @"^[0-9a-fA-F]{24}$");
        }

        private decimal? ParseSalary(string? salaryStr)
        {
            if (string.IsNullOrWhiteSpace(salaryStr)) return null;
            var cleaned = new string(salaryStr.Where(c => char.IsDigit(c) || c == '.' || c == ',').ToArray());
            cleaned = cleaned.Replace(",", ".");
            if (cleaned.Count(c => c == '.') > 1) cleaned = new string(cleaned.Where(char.IsDigit).ToArray());
            return decimal.TryParse(cleaned, out var result) ? result : null;
        }
    }
}
