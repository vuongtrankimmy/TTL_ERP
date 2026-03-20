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
#pragma warning disable CS0169, CS0414
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
        [Inject] public IBankService BankService { get; set; } = default!;
        [Inject] public ISettingsService SettingsService { get; set; } = default!;

        [Parameter] public string? Id { get; set; }
        [Parameter] public bool IsReadOnly { get; set; }
        public bool IsEditMode => !string.IsNullOrEmpty(Id);
        private bool IsEmailValid => !string.IsNullOrEmpty(newEmployee.Email) && FormatService.IsValidEmail(newEmployee.Email);

        private List<LookupModel> genderLookups = new();
        private List<LookupModel> maritalStatusLookups = new();
        private List<LookupModel> employeeStatusLookups = new();
        private List<LookupModel> contractTypeLookups = new();
        private List<LookupModel> workplaceLookups = new();
        private List<CountryModel> nationalityLookups = new();
        private List<LookupModel> ethnicityLookups = new();
        private List<LookupModel> religionLookups = new();
        private List<LookupModel> schoolLookups = new();
        private List<LookupModel> degreeLookups = new();
        private List<LookupModel> majorLookups = new();
        private List<LookupModel> provinceLookups = new();
        private List<DepartmentModel> departments = new();
        private List<PositionModel> positions = new();
        private List<EmployeeDto> allEmployees = new();
        private List<RoleModel> availableRoles = new();
        private List<BankDto> activeBanks = new();
        private bool _isProcessing = false;
        private bool _isLoading = false;
        private bool _isLoadingData = false;
        private bool _showPassword = false;
        private string? _loadingError;
        private string? _currentLoadingStage;
        private Dictionary<string, string> errorsMap = new();

        private EmployeeModel newEmployee = new()
        {
            IsActive = true,
            JoinDate = DateTime.Today,
            DOB = DateTime.Today.AddYears(-14)
        };
        
        private string[] BindRoles
        {
            get => newEmployee.Roles?.ToArray() ?? Array.Empty<string>();
            set => newEmployee.Roles = value?.ToList() ?? new List<string>();
        }

        private void ToggleRole(string roleId)
        {
            if (!newEmployee.IsCreateAccount) return;
            if (newEmployee.Roles == null) newEmployee.Roles = new List<string>();
            
            if (newEmployee.Roles.Contains(roleId))
            {
                newEmployee.Roles.Remove(roleId);
            }
            else
            {
                newEmployee.Roles.Add(roleId);
            }
            StateHasChanged();
        }

        private List<EmployeeDocumentModel> uploadedDocuments = new();
        private bool _isDataLoaded = false;
        private bool _isInitialized = false;

        protected override async Task OnInitializedAsync()
        {
            _isInitialized = true;
            try 
            {
                await SettingsService.InitializeAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing settings: {ex.Message}");
            }
        }

        private string? _lastLoadedId = null;
        protected override async Task OnParametersSetAsync()
        {
            // If already rendered, handle ID changes (e.g. Navigating from Edit NV001 to Edit NV002)
            if (_isFirstRenderDone && IsEditMode && Id != _lastLoadedId)
            {
                _lastLoadedId = Id;
                await LoadAllDataAsync();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _isFirstRenderDone = true;
                _lastLoadedId = Id;
                await LoadAllDataAsync();
            }
        }

        private async Task LoadAllDataAsync()
        {
            if (_isLoadingData) return;
            
            _isLoadingData = true;
            try 
            {
                await InvokeAsync(() => {
                    _isLoading = true;
                    _loadingError = null;
                    StateHasChanged();
                });

                // Safe JS logging - only if render is done
                if (_isFirstRenderDone) {
                    try { await JSRuntime.InvokeVoidAsync("console.log", $"[EmployeeAdd] Loading data. Mode: {(IsEditMode ? "Edit" : "Id=" + Id)}"); } catch {}
                }

                var lang = SettingsService.CachedSettings?.DefaultLanguage ?? "vi-VN";
                Console.WriteLine($"[EmployeeAdd] Starting LoadAllData. Mode: {(IsEditMode ? "Edit" : "Add")}, ID: {Id}");

                _currentLoadingStage = "Đang tải danh mục hệ thống...";
                await InvokeAsync(StateHasChanged);

                // Step 1: Lookups (Essential)
                try 
                {
                    genderLookups = (await MasterDataService.GetCachedLookupsAsync("Gender", lang) ?? new()).DistinctBy(l => l.LookupID).ToList();
                    maritalStatusLookups = (await MasterDataService.GetCachedLookupsAsync("MaritalStatus", lang) ?? new()).DistinctBy(l => l.LookupID).ToList();
                    employeeStatusLookups = (await MasterDataService.GetCachedLookupsAsync("EmployeeStatus", lang) ?? new()).DistinctBy(l => l.LookupID).ToList();
                    contractTypeLookups = (await MasterDataService.GetCachedLookupsAsync("ContractType", lang) ?? new()).DistinctBy(l => l.LookupID).ToList();
                    workplaceLookups = (await MasterDataService.GetCachedLookupsAsync("Workplace", lang) ?? new()).DistinctBy(l => l.LookupID).ToList();
                    nationalityLookups = (await MasterDataService.GetCachedCountriesAsync(lang) ?? new()).OrderBy(c => c.Name).ToList();
                    ethnicityLookups = (await MasterDataService.GetCachedLookupsAsync("Ethnicity", lang) ?? new()).DistinctBy(l => l.LookupID).ToList();
                    religionLookups = (await MasterDataService.GetCachedLookupsAsync("Religion", lang) ?? new()).DistinctBy(l => l.LookupID).ToList();
                    
                    // Additional suggestions for Education & Location
                    provinceLookups = await MasterDataService.GetProvincesAsync(lang) ?? new();
                    schoolLookups = (await MasterDataService.GetCachedLookupsAsync("Education_School", lang) ?? new()).DistinctBy(l => l.Name).ToList();
                    if (!schoolLookups.Any()) schoolLookups = (await MasterDataService.GetCachedLookupsAsync("School", lang) ?? new()).DistinctBy(l => l.Name).ToList();
                    
                    degreeLookups = (await MasterDataService.GetCachedLookupsAsync("Education_Level", lang) ?? new()).DistinctBy(l => l.Name).ToList();
                    if (!degreeLookups.Any()) degreeLookups = (await MasterDataService.GetCachedLookupsAsync("Degree", lang) ?? new()).DistinctBy(l => l.Name).ToList();
                    
                    majorLookups = (await MasterDataService.GetCachedLookupsAsync("Education_Major", lang) ?? new()).DistinctBy(l => l.Name).ToList();
                    if (!majorLookups.Any()) majorLookups = (await MasterDataService.GetCachedLookupsAsync("Major", lang) ?? new()).DistinctBy(l => l.Name).ToList();
                }
                catch (Exception ex) { Console.WriteLine($"Step 1 failed: {ex.Message}"); }

                _currentLoadingStage = "Đang tải cơ cấu tổ chức...";
                await InvokeAsync(StateHasChanged);

                // Step 2: Org & Settings
                try 
                {
                    departments = await DepartmentService.GetDepartmentsAsync() ?? new();
                    positions = await PositionService.GetPositionsAsync() ?? new();
                    availableRoles = await PermissionService.GetRolesAsync() ?? new();
                    
                    var banksResult = await BankService.GetBanksAsync(new GetBanksRequest { Page = 1, PageSize = 1000 });
                    activeBanks = banksResult?.Items?.Where(b => b.IsActive).OrderByDescending(b => b.Priority).ThenBy(b => b.Code).ToList() ?? new();
                }
                catch (Exception ex) { 
                    Console.WriteLine($"Step 2 failed: {ex.Message}");
                    if (_isFirstRenderDone) {
                        try { await JSRuntime.InvokeVoidAsync("console.warn", $"Step 2 failed: {ex.Message}"); } catch {}
                    }
                }

                _isDataLoaded = true;
                Console.WriteLine($"[EmployeeAdd] Step 1&2 Done. _isDataLoaded={_isDataLoaded}");
                await InvokeAsync(StateHasChanged);

                // Step 3: Specific Employee
                if (IsEditMode)
                {
                    _currentLoadingStage = "Đang tải chi tiết hồ sơ nhân viên...";
                    await InvokeAsync(StateHasChanged);
                    try
                    {
                        Console.WriteLine($"[EmployeeAdd] Fetching employee {Id}...");
                        if (_isFirstRenderDone) {
                            try { await JSRuntime.InvokeVoidAsync("console.log", $"[EmployeeAdd] Fetching data for {Id}..."); } catch {}
                        }
                        
                        // Add a timeout of 15 seconds to the fetch to prevent hanging forever
                        var fetchTask = EmployeeService.GetEmployeeAsync(Id!);
                        var timeoutTask = Task.Delay(15000);
                        var completedTask = await Task.WhenAny(fetchTask, timeoutTask);
                        
                        if (completedTask == timeoutTask)
                        {
                            throw new TimeoutException("Hết thời gian chờ phản hồi từ máy chủ (15s).");
                        }
                        
                        var employee = await fetchTask;
                        if (employee != null)
                        {
                            Console.WriteLine($"[EmployeeAdd] Employee {Id} data received. Mapping properties to maintain reference stability...");
                            
                            // Map properties individually to maintain object reference for Blazor binding
                            newEmployee.Id = employee.Id;
                            newEmployee.Code = employee.Code;
                            newEmployee.FullName = employee.FullName;
                            newEmployee.TimekeepingCode = employee.TimekeepingCode;
                            newEmployee.Email = employee.Email;
                            newEmployee.CompanyEmail = employee.CompanyEmail;
                            newEmployee.Phone = employee.Phone;
                            newEmployee.AvatarUrl = employee.AvatarUrl;
                            newEmployee.StatusId = employee.StatusId;
                            newEmployee.ContractTypeId = employee.ContractTypeId;
                            newEmployee.JoinDate = employee.JoinDate;
                            newEmployee.DepartmentId = employee.DepartmentId;
                            newEmployee.PositionId = employee.PositionId;
                            newEmployee.ReportToId = employee.ReportToId;
                            newEmployee.WorkplaceId = employee.WorkplaceId;
                            newEmployee.Salary = employee.Salary;
                            newEmployee.ContractEndDate = employee.ContractEndDate;
                            newEmployee.Username = employee.Username;
                            newEmployee.IsAccountActive = employee.IsAccountActive;
                            newEmployee.IsCreateAccount = employee.IsCreateAccount;
                            newEmployee.IsTrainer = employee.IsTrainer;
                            newEmployee.Roles = employee.Roles ?? new();

                            // compatibility fields sync
                            newEmployee.Name = !string.IsNullOrEmpty(employee.FullName) ? employee.FullName : employee.Name;
                            newEmployee.DeptId = employee.DepartmentId;
                            newEmployee.Avatar = !string.IsNullOrEmpty(employee.AvatarUrl) ? employee.AvatarUrl : employee.Avatar;
                            newEmployee.OfficialJoinDate = employee.JoinDate;
                            if (employee.Salary.HasValue) newEmployee.SalaryDisplay = employee.Salary.Value.ToString("N0");

                            // Nested objects - merge/update
                            if (employee.PersonalDetails != null)
                            {
                                newEmployee.PersonalDetails ??= new();
                                var pd = employee.PersonalDetails;
                                
                                // Sync nested properties
                                newEmployee.PersonalDetails.DOB = pd.DOB;
                                newEmployee.PersonalDetails.GenderId = pd.GenderId;
                                newEmployee.PersonalDetails.IdCardNumber = pd.IdCardNumber;
                                newEmployee.PersonalDetails.TaxCode = pd.TaxCode;
                                newEmployee.PersonalDetails.BankAccount = pd.BankAccount;
                                newEmployee.PersonalDetails.BankName = pd.BankName;
                                newEmployee.PersonalDetails.Residence = pd.Residence;
                                newEmployee.PersonalDetails.Address = pd.Address;
                                newEmployee.PersonalDetails.Hometown = pd.Hometown;
                                newEmployee.PersonalDetails.PlaceOfOrigin = pd.PlaceOfOrigin;
                                newEmployee.PersonalDetails.SocialInsuranceId = pd.SocialInsuranceId;
                                newEmployee.PersonalDetails.Dependents = pd.Dependents ?? new();
                                newEmployee.PersonalDetails.CountryId = pd.CountryId;
                                newEmployee.PersonalDetails.ProvinceId = pd.ProvinceId;
                                newEmployee.PersonalDetails.DistrictId = pd.DistrictId;
                                newEmployee.PersonalDetails.WardId = pd.WardId;
                                newEmployee.PersonalDetails.Street = pd.Street;
                                newEmployee.PersonalDetails.IdCardIssueDate = pd.IdCardIssueDate;
                                newEmployee.PersonalDetails.IdCardPlace = pd.IdCardPlace;
                                newEmployee.PersonalDetails.NationalityId = pd.NationalityId;
                                newEmployee.PersonalDetails.EthnicityId = pd.EthnicityId;
                                newEmployee.PersonalDetails.ReligionId = pd.ReligionId;
                                newEmployee.PersonalDetails.MaritalStatusId = pd.MaritalStatusId;
                                newEmployee.PersonalDetails.Nationality = pd.Nationality;
                                newEmployee.PersonalDetails.Ethnicity = pd.Ethnicity;
                                newEmployee.PersonalDetails.Religion = pd.Religion;
                                newEmployee.PersonalDetails.MaritalStatus = pd.MaritalStatus;

                                // Sync compatibility top-level fields from nested source
                                newEmployee.DOB = pd.DOB;
                                newEmployee.GenderId = pd.GenderId;
                                newEmployee.IdCard = !string.IsNullOrEmpty(pd.IdCardNumber) ? pd.IdCardNumber : pd.IdCard;
                                newEmployee.TaxId = pd.TaxCode;
                                newEmployee.BankAccountNumber = pd.BankAccount;
                                newEmployee.BankName = pd.BankName;
                                newEmployee.Residence = pd.Residence ?? string.Empty;
                                newEmployee.Address = pd.Address;
                                newEmployee.Hometown = pd.Hometown;
                                newEmployee.PlaceOfOrigin = pd.PlaceOfOrigin ?? string.Empty;
                                newEmployee.SocialInsuranceId = pd.SocialInsuranceId ?? string.Empty;
                                newEmployee.CccdIssueDate = pd.IdCardIssueDate;
                                newEmployee.CccdIssuePlace = pd.IdCardPlace;
                                newEmployee.NationalityId = pd.NationalityId;
                                newEmployee.EthnicityId = pd.EthnicityId;
                                newEmployee.ReligionId = pd.ReligionId;
                                newEmployee.MaritalStatusId = pd.MaritalStatusId;
                                newEmployee.Nationality = pd.Nationality;
                                newEmployee.Ethnicity = pd.Ethnicity;
                                newEmployee.Religion = pd.Religion;
                                newEmployee.MaritalStatus = pd.MaritalStatus;
                            }

                            if (employee.EmergencyContact != null)
                            {
                                newEmployee.EmergencyContact ??= new();
                                var ec = employee.EmergencyContact;
                                newEmployee.EmergencyContact.Name = ec.Name;
                                newEmployee.EmergencyContact.Phone = ec.Phone;
                                newEmployee.EmergencyContact.Relation = ec.Relation;

                                // Sync top-level fields
                                newEmployee.EmergencyContactName = ec.Name;
                                newEmployee.EmergencyContactPhone = ec.Phone;
                                newEmployee.EmergencyContactRelation = ec.Relation;
                            }

                            newEmployee.Education = employee.Education ?? new();
                            newEmployee.Experience = employee.Experience ?? new();
                            newEmployee.AuditLogs = employee.AuditLogs ?? new();
                            newEmployee.ModulePermissions = employee.ModulePermissions ?? new();
                            newEmployee.AttendanceSummary = employee.AttendanceSummary ?? new();

                            if (newEmployee.Salary.HasValue && string.IsNullOrEmpty(newEmployee.SalaryDisplay))
                                newEmployee.SalaryDisplay = newEmployee.Salary.Value.ToString("N0");
                            
                            newEmployee.IsAccountActive = employee.IsAccountActive;
                            newEmployee.IsActive = employee.IsAccountActive;

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

                            // ... mapping compatibility ends ...

                            // Force a UI update here so the main form data is visible
                            _isLoading = false;
                            await InvokeAsync(StateHasChanged);

                            try {
                                var profileTask = EmployeeService.GetDigitalProfileAsync(Id!);
                                var profileTimeout = Task.Delay(3000); // Shorter 3s timeout for profile
                                if (await Task.WhenAny(profileTask, profileTimeout) == profileTask)
                                {
                                    var profile = await profileTask;
                                    if (profile != null) uploadedDocuments = profile.Documents ?? new();
                                    await InvokeAsync(StateHasChanged);
                                }
                            } catch { }
                        }
                        else
                        {
                            _loadingError = "Hồ sơ không tồn tại (404/Null).";
                        }
                    }
                    catch (Exception ex)
                    {
                        _loadingError = $"Lỗi dữ liệu: {ex.Message}";
                    }
                }
                else
                {
                    // New Mode Defaults
                    if (!newEmployee.GenderId.HasValue) newEmployee.GenderId = genderLookups?.FirstOrDefault()?.LookupID;
                    if (!newEmployee.StatusId.HasValue) newEmployee.StatusId = employeeStatusLookups?.FirstOrDefault()?.LookupID;
                    if (string.IsNullOrEmpty(newEmployee.Nationality)) newEmployee.Nationality = "Việt Nam";
                    if (string.IsNullOrEmpty(newEmployee.Role)) newEmployee.Role = "User";
                }
                
                if (_isFirstRenderDone) {
                    try { await JSRuntime.InvokeVoidAsync("console.log", "[EmployeeAdd] Data load sequence completed successfully."); } catch {}
                }
            }
            catch (Exception ex)
            {
                _loadingError = $"Lỗi tải trang: {ex.Message}";
                Console.WriteLine($"[EmployeeAdd] CRITICAL ERROR IN LOAD: {ex}");
                
                await JSRuntime.InvokeVoidAsync("Swal.fire", new 
                {
                    title = "Lỗi tải dữ liệu",
                    html = $@"<div class='text-start'>
                                <p>Không thể tải thông tin nhân viên hoặc các danh mục hỗ trợ:</p>
                                <hr/>
                                <p><b>Thông điệp:</b> {ex.Message}</p>
                                <pre class='bg-light p-2 border' style='font-size: 10px; max-height: 150px; overflow-y: auto;'>{ex.StackTrace}</pre>
                             </div>",
                    icon = "error",
                    confirmButtonText = "Quay lại",
                    width = "600px"
                });
                Navigation.NavigateTo("/employees");
            }
            finally
            {
                Console.WriteLine($"[EmployeeAdd] LoadAllData Finally. ID: {Id}");
                await InvokeAsync(() => {
                    _isLoading = false;
                    _isLoadingData = false;
                    StateHasChanged();
                });
            }
        }

        private bool _isFirstRenderDone = false;
        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                _isFirstRenderDone = true;
                StateHasChanged();
            }
        }

        private TTL.HR.Shared.Components.Common.CccdScanner cccdScanner = null!;

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

        private async Task SendCredentialsAction(string channel)
        {
            if (string.IsNullOrEmpty(Id))
            {
                await JSRuntime.InvokeVoidAsync("toastr.info", "Vui lòng lưu hồ sơ nhân viên trước khi gửi thông tin tài khoản.");
                return;
            }

            string target = channel switch
            {
                "EMAIL" => newEmployee.Email,
                _ => newEmployee.Phone
            };

            if (string.IsNullOrEmpty(target))
            {
                await JSRuntime.InvokeVoidAsync("toastr.warning", $"Nhân viên chưa có {(channel == "EMAIL" ? "Email" : "Số điện thoại")} để gửi thông tin.");
                return;
            }

            var confirmText = channel switch
            {
                "EMAIL" => $"Gửi thông tin tài khoản tới Email: {newEmployee.Email}?",
                "ZALO" => $"Gửi thông báo qua Zalo tới SĐT: {newEmployee.Phone}?",
                "SMS" => $"Gửi tin nhắn SMS tới SĐT: {newEmployee.Phone}?",
                _ => "Gửi thông tin tài khoản?"
            };

            var confirmElement = await JSRuntime.InvokeAsync<dynamic>("Swal.fire", new
            {
                title = "Xác nhận gửi",
                text = confirmText,
                icon = "question",
                showCancelButton = true,
                confirmButtonText = "Đồng ý, gửi ngay!",
                cancelButtonText = "Hủy",
                confirmButtonColor = channel == "ZALO" ? "#0068ff" : (channel == "EMAIL" ? "#009EF7" : "#50CD89")
            });

            if (confirmElement != null && confirmElement.isConfirmed == true)
            {
                _isProcessing = true;
                StateHasChanged();

                try
                {
                    var success = await EmployeeService.SendCredentialsAsync(Id, channel);
                    if (success)
                    {
                        await JSRuntime.InvokeVoidAsync("Swal.fire", "Thành công", $"Thông tin đã được gửi qua kênh {channel}.", "success");
                    }
                    else
                    {
                        await JSRuntime.InvokeVoidAsync("Swal.fire", "Thất bại", "Không thể gửi tin nhắn. Vui lòng kiểm tra lại cấu hình thông báo.", "error");
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

        private async Task RequestPasswordReset()
        {
            if (string.IsNullOrEmpty(newEmployee.Email))
            {
                await JSRuntime.InvokeVoidAsync("toastr.warning", "Nhân viên chưa có Email để nhận link đặt lại mật khẩu.");
                return;
            }

            var confirmElement = await JSRuntime.InvokeAsync<dynamic>("Swal.fire", new
            {
                title = "Xác nhận",
                text = $"Hệ thống sẽ gửi link đặt lại mật khẩu tới email: {newEmployee.Email}. Bạn có chắc chắn muốn thực hiện?",
                icon = "question",
                showCancelButton = true,
                confirmButtonText = "Đồng ý, gửi ngay!",
                cancelButtonText = "Hủy"
            });

            if (confirmElement != null && confirmElement.isConfirmed == true)
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
                using (var stream = file.OpenReadStream(2 * 1024 * 1024))
                {
                    var bytesRead = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length));
                }
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
            errorsMap.Clear();
            var errors = new List<string>();
            
            if (string.IsNullOrWhiteSpace(newEmployee.Name)) errorsMap["Name"] = "Họ tên nhân viên không hợp lệ";
            
            if (!string.IsNullOrWhiteSpace(newEmployee.Email) && !FormatService.IsValidEmail(newEmployee.Email)) 
            {
                errorsMap["Email"] = "Email không đúng định dạng (VD: example@mail.com)";
            }

            if (!string.IsNullOrWhiteSpace(newEmployee.Phone) && newEmployee.Phone.Length < 10) 
            {
                errorsMap["Phone"] = "Số điện thoại phải từ 10 số";
            }

            if (!string.IsNullOrWhiteSpace(newEmployee.IdCard) && newEmployee.IdCard.Length != 12 && newEmployee.IdCard.Length != 9) 
            {
                errorsMap["IdCard"] = "Số CCCD phải đủ 12 chữ số (hoặc 9 số đối với CMND cũ)";
            }

            if (!string.IsNullOrWhiteSpace(newEmployee.BankAccountNumber))
            {
                if (newEmployee.BankAccountNumber.Length < 6 || newEmployee.BankAccountNumber.Length > 20)
                {
                    errorsMap["BankAccountNumber"] = "Số tài khoản phải từ 6-20 chữ số";
                }
                if (string.IsNullOrEmpty(newEmployee.BankName))
                {
                    errorsMap["BankName"] = "Vui lòng chọn ngân hàng cho số tài khoản này";
                }
            }

            if (!string.IsNullOrWhiteSpace(newEmployee.TaxId))
            {
                // Regexp: 10 digits or 10 digits + hyphen + 3 digits
                if (!System.Text.RegularExpressions.Regex.IsMatch(newEmployee.TaxId, @"^\d{10}(-?\d{3})?$"))
                {
                    errorsMap["TaxId"] = "Mã số thuế không hợp lệ (10 hoặc 13 chữ số)";
                }
            }

            // Only require structural fields when CREATING, not when editing
            if (!IsEditMode)
            {
                if (string.IsNullOrEmpty(newEmployee.DeptId)) errorsMap["DeptId"] = "Vui lòng chọn Phòng ban";
                if (string.IsNullOrEmpty(newEmployee.PositionId)) errorsMap["PositionId"] = "Vui lòng chọn Chức vụ";
                if (!newEmployee.StatusId.HasValue) errorsMap["StatusId"] = "Vui lòng chọn Trạng thái";
                if (!newEmployee.WorkplaceId.HasValue) errorsMap["WorkplaceId"] = "Vui lòng chọn Nơi làm việc";
                if (!newEmployee.ContractTypeId.HasValue) errorsMap["ContractTypeId"] = "Vui lòng chọn Loại hợp đồng";
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
                    errorsMap["Username"] = "Không thể tự động tạo Tên đăng nhập. Vui lòng nhập thủ công.";
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
                        errorsMap["Username"] = "Tên đăng nhập không hợp lệ (phải chứa ít nhất 1 ký tự a-z hoặc 0-9)";
                    }
                }
            }

            if (errorsMap.Any())
            {
                await JSRuntime.InvokeVoidAsync("Swal.fire", "Thông tin chưa hợp lệ", 
                    "Vui lòng kiểm tra lại các trường thông tin được đánh dấu đỏ.", "warning");
                
                // Focus on first error
                var firstErrorKey = errorsMap.Keys.First();
                await JSRuntime.InvokeVoidAsync("LayoutHelper.scrollToElement", $"input_{firstErrorKey}");
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
                        StatusId = newEmployee.StatusId,
                        ContractTypeId = newEmployee.ContractTypeId,
                        
                        JoinDate = newEmployee.OfficialJoinDate ?? newEmployee.JoinDate ?? DateTime.Today,
                        Salary = newEmployee.Salary,
                        ContractEndDate = newEmployee.ContractExpiry ?? newEmployee.ContractEndDate,
                        WorkplaceId = newEmployee.WorkplaceId,
                        IsAccountActive = newEmployee.IsActive,
                        IsCreateAccount = newEmployee.IsCreateAccount,
                        IsTrainer = newEmployee.IsTrainer,
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
                            Nationality = newEmployee.Nationality,
                            EthnicityId = newEmployee.EthnicityId,
                            Ethnicity = ethnicityLookups.FirstOrDefault(x => x.LookupID == newEmployee.EthnicityId)?.Name ?? newEmployee.Ethnicity,
                            ReligionId = newEmployee.ReligionId,
                            Religion = religionLookups.FirstOrDefault(x => x.LookupID == newEmployee.ReligionId)?.Name ?? newEmployee.Religion,
                            MaritalStatusId = newEmployee.MaritalStatusId,
                            MaritalStatus = maritalStatusLookups.FirstOrDefault(x => x.LookupID == newEmployee.MaritalStatusId)?.Name ?? newEmployee.MaritalStatus,
                            PlaceOfOrigin = newEmployee.PlaceOfOrigin,
                            Residence = newEmployee.Residence,
                            SocialInsuranceId = newEmployee.SocialInsuranceId,
                            CountryId = newEmployee.PersonalDetails?.CountryId,
                            ProvinceId = newEmployee.PersonalDetails?.ProvinceId,
                            DistrictId = newEmployee.PersonalDetails?.DistrictId,
                            WardId = newEmployee.PersonalDetails?.WardId,
                            Street = newEmployee.PersonalDetails?.Street,
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
                        await HandleServerError(resultStr);
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
                        StatusId = newEmployee.StatusId,
                        ContractTypeId = newEmployee.ContractTypeId,
                        
                        JoinDate = newEmployee.OfficialJoinDate ?? newEmployee.JoinDate ?? DateTime.Today,
                        Salary = newEmployee.Salary,
                        ContractEndDate = newEmployee.ContractExpiry ?? newEmployee.ContractEndDate,
                        WorkplaceId = newEmployee.WorkplaceId,
                        IsTrainer = newEmployee.IsTrainer,
                        
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
                            Nationality = newEmployee.Nationality,
                            EthnicityId = newEmployee.EthnicityId,
                            Ethnicity = ethnicityLookups.FirstOrDefault(x => x.LookupID == newEmployee.EthnicityId)?.Name ?? newEmployee.Ethnicity,
                            ReligionId = newEmployee.ReligionId,
                            Religion = religionLookups.FirstOrDefault(x => x.LookupID == newEmployee.ReligionId)?.Name ?? newEmployee.Religion,
                            MaritalStatusId = newEmployee.MaritalStatusId,
                            MaritalStatus = maritalStatusLookups.FirstOrDefault(x => x.LookupID == newEmployee.MaritalStatusId)?.Name ?? newEmployee.MaritalStatus,
                            PlaceOfOrigin = newEmployee.PlaceOfOrigin,
                            Residence = newEmployee.Residence,
                            SocialInsuranceId = newEmployee.SocialInsuranceId,
                            CountryId = newEmployee.PersonalDetails?.CountryId,
                            ProvinceId = newEmployee.PersonalDetails?.ProvinceId,
                            DistrictId = newEmployee.PersonalDetails?.DistrictId,
                            WardId = newEmployee.PersonalDetails?.WardId,
                            StreetId = newEmployee.PersonalDetails?.StreetId,
                            Street = newEmployee.PersonalDetails?.Street,
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
                        await HandleServerError(result ?? "Không thể lưu hồ sơ. Vui lòng kiểm tra lại dữ liệu.");

                    }
                }

            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("Swal.fire", new
                {
                    title = "Lỗi hệ thống nghiêm trọng",
                    html = $@"<div class='text-start'>
                                <p><b>Thông điệp:</b> {ex.Message}</p>
                                <hr/>
                                <p><b>Chi tiết kỹ thuật (Stack Trace):</b></p>
                                <pre class='bg-light p-3 border rounded' style='font-size: 10px; max-height: 200px; overflow-y: auto;'>{ex.StackTrace}</pre>
                             </div>",
                    icon = "error",
                    width = "600px"
                });
                Console.WriteLine($"[EmployeeAdd] Error in HandleSaveEmployee: {ex}");
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
        
        private async Task HandleServerError(string errorMsg)
        {
            errorsMap.Clear();
            
            if (errorMsg.Contains("<!DOCTYPE html>") || errorMsg.Contains("<html"))
            {
                errorMsg = "Lỗi máy chủ (500 Internal Server Error). Vui lòng liên hệ bộ phận kỹ thuật.";
            }

            // Typical format: "Invalid data : Email: Email đã tồn tại..."
            if (errorMsg.Contains("Invalid data"))
            {
                var parts = errorMsg.Split(':', StringSplitOptions.TrimEntries);
                if (parts.Length >= 2)
                {
                    // parts[0] = "Invalid data"
                    // parts[1] = "Email"
                    // parts[2] = "Email đã tồn tại..."
                    
                    var fieldPart = parts[1].ToLower();
                    var msg = parts.Length > 2 ? string.Join(": ", parts.Skip(2)) : parts[1];
                    
                    bool found = false;
                    if (fieldPart.Contains("email")) { errorsMap["Email"] = msg; found = true; }
                    else if (fieldPart.Contains("fullname") || fieldPart.Contains("name")) { errorsMap["Name"] = msg; found = true; }
                    else if (fieldPart.Contains("code")) { errorsMap["Code"] = msg; found = true; }
                    else if (fieldPart.Contains("phone")) { errorsMap["Phone"] = msg; found = true; }
                    else if (fieldPart.Contains("idcard")) { errorsMap["IdCard"] = msg; found = true; }
                    else if (fieldPart.Contains("username")) { errorsMap["Username"] = msg; found = true; }
                    else if (fieldPart.Contains("bankaccount")) { errorsMap["BankAccountNumber"] = msg; found = true; }
                    else if (fieldPart.Contains("bankname")) { errorsMap["BankName"] = msg; found = true; }
                    else if (fieldPart.Contains("tax")) { errorsMap["TaxId"] = msg; found = true; }
                    else if (fieldPart.Contains("nationality")) { errorsMap["Nationality"] = msg; found = true; }
                    else if (fieldPart.Contains("ethnicity")) { errorsMap["EthnicityId"] = msg; found = true; }
                    else if (fieldPart.Contains("religion")) { errorsMap["ReligionId"] = msg; found = true; }
                    else if (fieldPart.Contains("dept") || fieldPart.Contains("department")) { errorsMap["DeptId"] = msg; found = true; }
                    else if (fieldPart.Contains("position")) { errorsMap["PositionId"] = msg; found = true; }
                    else if (fieldPart.Contains("status")) { errorsMap["StatusId"] = msg; found = true; }
                    else if (fieldPart.Contains("contract")) { errorsMap["ContractTypeId"] = msg; found = true; }
                    
                    if (found) {
                        errorMsg = msg; // Simplify message for Swal if we identified the field
                    }
                }
            }
            
            if (errorsMap.Any())
            {
                await JSRuntime.InvokeVoidAsync("Swal.fire", "Lỗi dữ liệu", errorMsg, "error");
            }
            else
            {
                // Unrecognized or complex error, show detail
                await JSRuntime.InvokeVoidAsync("Swal.fire", new
                {
                    title = "Lấy phản hồi từ máy chủ",
                    html = $@"<div class='text-start'>
                                <p>Có lỗi xảy ra khi thực hiện yêu cầu:</p>
                                <div class='bg-light p-3 border rounded' style='font-size: 11px; max-height: 250px; overflow-y: auto;'>
                                    {errorMsg}
                                </div>
                             </div>",
                    icon = "error",
                    width = "600px"
                });
            }
            
            if (errorsMap.Any())
            {
                StateHasChanged();
                await Task.Delay(300); // Wait for modal to show/animate
                var firstErrorKey = errorsMap.Keys.First();
                await JSRuntime.InvokeVoidAsync("LayoutHelper.scrollToElement", $"input_{firstErrorKey}");
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
            public string? ProvinceId { get; set; }
            public string? DistrictId { get; set; }
            public string? WardId { get; set; }
            public string? Street { get; set; }
            public string? PositionId { get; set; }
            public string? ReportToId { get; set; }
            public int? StatusId { get; set; }
            public int? ContractTypeId { get; set; }
            public DateTime JoinDate { get; set; }
            public decimal? Salary { get; set; }
            public DateTime? ContractEndDate { get; set; }
            public int? WorkplaceId { get; set; }
            public PersonalDetailsCommandDto PersonalDetails { get; set; } = new();
            public EmergencyContactCommandDto EmergencyContact { get; set; } = new();
            public List<EducationDetailDto> Education { get; set; } = new();
            public List<ExperienceDetailDto> Experience { get; set; } = new();
            public bool IsCreateAccount { get; set; }
            public bool IsTrainer { get; set; }
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
            /// <summary>Mã quốc gia ISO 3166-1 numeric (704 = Việt Nam)</summary>
            public int? CountryId { get; set; }
            /// <summary>Mã tỉnh/thành hành chính VN</summary>
            public int? ProvinceId { get; set; }
            /// <summary>Mã quận/huyện hành chính VN</summary>
            public int? DistrictId { get; set; }
            /// <summary>Mã phường/xã hành chính VN</summary>
            public int? WardId { get; set; }
            /// <summary>Số thứ tự đường (int lookup). Null nếu nhập tay.</summary>
            public int? StreetId { get; set; }
            public string? Street { get; set; }
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
                WorkplaceId = dto.WorkplaceId,
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
                    CountryId = dto.PersonalDetails.CountryId,
                    ProvinceId = dto.PersonalDetails.ProvinceId,
                    DistrictId = dto.PersonalDetails.DistrictId,
                    WardId = dto.PersonalDetails.WardId,
                    Street = dto.PersonalDetails.Street,
                    Dependents = dto.PersonalDetails.Dependents ?? new List<DependentDetailDto>()
                },
                EmergencyContact = new Entities.EmergencyContact
                {
                    Name = dto.EmergencyContact.Name,
                    Relation = dto.EmergencyContact.Relation,
                    Phone = dto.EmergencyContact.Phone
                },
                IsTrainer = dto.IsTrainer,
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

        private int? ParseInt(string? value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return int.TryParse(value, out var result) ? result : (int?)null;
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
