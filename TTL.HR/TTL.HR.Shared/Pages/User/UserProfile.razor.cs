using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.HumanResource.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Models;
using TTL.HR.Application.Modules.Organization.Interfaces;
using TTL.HR.Application.Modules.Organization.Models;
using TTL.HR.Application.Modules.Attendance.Models;
using TTL.HR.Application.Modules.Leave.Models;
using TTL.HR.Application.Modules.Payroll.Models;
using TTL.HR.Application.Modules.Attendance.Interfaces;
using TTL.HR.Application.Modules.Leave.Interfaces;
using TTL.HR.Application.Modules.Payroll.Interfaces;

namespace TTL.HR.Shared.Pages.User
{
    public partial class UserProfile : IDisposable
    {
        [Inject] private IAuthService AuthService { get; set; } = default!;
        [Inject] private IEmployeeService EmployeeService { get; set; } = default!;
        [Inject] private IPositionService PositionService { get; set; } = default!;
        [Inject] private IBankService BankService { get; set; } = default!;
        [Inject] private IMasterDataService MasterDataService { get; set; } = default!;
        [Inject] private IContractService ContractService { get; set; } = default!;
        [Inject] private IAttendanceService AttendanceService { get; set; } = default!;
        [Inject] private ILeaveService LeaveService { get; set; } = default!;
        [Inject] private IPayrollService PayrollService { get; set; } = default!;
        [Inject] private IAuditService AuditService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        private UserDto? currentUser;
        private EmployeeModel? currentEmployee;
        private string activeTab = "overview";
        private string? employeeId;
        private bool _isLoadingProfile = true;
        private List<EmployeeDto>? _allEmployees;
        private List<PositionModel>? _positions;
        private List<BankDto>? _banks;
        private List<CountryModel>? _countries;
        private List<LookupModel>? _provinces;
        private List<LookupModel>? _wards;
        private List<LookupModel>? _genders;
        private List<LookupModel>? _maritalStatuses;
        private string? _selectedEmployeeToLink;
        private string? _linkMessage;
        private bool _isLinking = false;
        
        // Tab Data
        private List<EmployeeContractModel>? _contracts;
        private List<PayrollModel>? _payrolls;
        private DigitalProfileModel? _digitalProfile;
        private List<AttendanceDetailModel>? _attendanceDetails;
        private LeaveBalanceModel? _leaveBalance;
        private List<LeaveRequestModel>? _leaveRequests;
        private List<AuditLogModel>? _auditLogs;
        private int _selectedYear = DateTime.Now.Year;
        private int _selectedMonth = DateTime.Now.Month;
        
        // Form binds
        private string? _editFullName;
        private string? _editPhone;
        private string? _editEmail;
        private string? _editJobTitle;
        private string? _editIdCard;
        private string? _editHometown;
        private int? _editCountryId;
        private int? _editProvinceId;
        private int? _editDistrictId;
        private int? _editWardId;
        private int? _editStreetId;
        private string? _editStreet;
        private int? _editGenderId;
        private int? _editMaritalStatusId;
        private string? _editBankAccount;
        private string? _editBankName;
        private int? _editDependents;

        private string? _currentPassword;
        private string? _newPassword;
        private string? _confirmPassword;
        private string? _modalError;
        private string? _modalSuccess;
        private bool _isSaving = false;
        private bool isEditModalOpen = false;
        private bool isPasswordModalOpen = false;

        [SupplyParameterFromQuery(Name = "tab")] public string? TabFromUrl { get; set; }

        private string? currentPhone => currentEmployee?.Phone ?? currentUser?.Phone;

        protected override async Task OnInitializedAsync()
        {
            Console.WriteLine("[UserProfile] OnInitializedAsync Started");
            try
            {
                Navigation.LocationChanged += HandleLocationChanged;
                await LoadData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserProfile] TOP LEVEL INIT ERROR: {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                _isLoadingProfile = false;
                Console.WriteLine("[UserProfile] OnInitializedAsync Completed");
            }
        }

        private async Task LoadData()
        {
            try
            {
                Console.WriteLine("[UserProfile] Loading currentUser...");
                currentUser = await AuthService.GetCurrentUserAsync();
                
                Console.WriteLine("[UserProfile] Loading currentEmployee...");
                currentEmployee = await EmployeeService.GetMyEmployeeAsync();
                employeeId = currentEmployee?.Id;
                
                UpdateActiveTabFromUrl();
                InitializeEditModel();
                await LoadTabData(activeTab);

                Console.WriteLine("[UserProfile] Loading dropdown data...");
                try { _allEmployees = await EmployeeService.GetEmployeesAsync(); } catch (Exception ex) { Console.WriteLine($"[UserProfile] Error loading employees: {ex.Message}"); }
                try { _positions = await PositionService.GetPositionsAsync(); } catch (Exception ex) { Console.WriteLine($"[UserProfile] Error loading positions: {ex.Message}"); }
                try 
                { 
                    var bankResponse = await BankService.GetBanksAsync(new GetBanksRequest { PageSize = 1000 });
                    _banks = bankResponse?.Items ?? new List<BankDto>();
                } catch (Exception ex) { Console.WriteLine($"[UserProfile] Error loading banks: {ex.Message}"); }
                
                try { _countries = await MasterDataService.GetCachedCountriesAsync(); } catch (Exception ex) { Console.WriteLine($"[UserProfile] Error loading countries: {ex.Message}"); }
                try { _provinces = await MasterDataService.GetProvincesAsync(); } catch (Exception ex) { Console.WriteLine($"[UserProfile] Error loading provinces: {ex.Message}"); }
                try { _genders = await MasterDataService.GetLookupsAsync("Gender"); } catch (Exception ex) { Console.WriteLine($"[UserProfile] Error loading genders: {ex.Message}"); }
                try { _maritalStatuses = await MasterDataService.GetLookupsAsync("MaritalStatus"); } catch (Exception ex) { Console.WriteLine($"[UserProfile] Error loading marital statuses: {ex.Message}"); }
                
                if (_editDistrictId.HasValue)
                {
                    try { _wards = await MasterDataService.GetWardsAsync(_editDistrictId.Value); } catch (Exception ex) { Console.WriteLine($"[UserProfile] Error loading wards: {ex.Message}"); }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserProfile] LoadData Error: {ex.Message}");
            }
            finally
            {
                EnsureCollectionsInitialized();
                StateHasChanged();
            }
        }

        private void InitializeEditModel()
        {
            _editFullName = currentEmployee?.FullName ?? currentUser?.FullName;
            _editPhone = currentEmployee?.Phone ?? currentUser?.Phone;
            _editEmail = currentEmployee?.Email ?? currentUser?.Email;
            _editJobTitle = currentEmployee?.PositionName ?? currentUser?.JobTitle;
            _editIdCard = currentEmployee?.PersonalDetails?.IdCardNumber ?? currentEmployee?.IdCard ?? currentUser?.IdCardNumber;
            _editHometown = currentEmployee?.PersonalDetails?.Hometown ?? currentEmployee?.Hometown ?? currentUser?.Hometown;
            _editCountryId = currentEmployee?.PersonalDetails?.CountryId ?? currentUser?.CountryId;
            _editProvinceId = currentEmployee?.PersonalDetails?.ProvinceId ?? currentUser?.ProvinceId;
            _editDistrictId = currentEmployee?.PersonalDetails?.DistrictId ?? currentUser?.DistrictId;
            _editWardId = currentEmployee?.PersonalDetails?.WardId ?? currentUser?.WardId;
            _editStreetId = currentEmployee?.PersonalDetails?.StreetId ?? currentUser?.StreetId;
            _editStreet = currentEmployee?.PersonalDetails?.Street ?? currentUser?.Street;
            _editGenderId = currentEmployee?.PersonalDetails?.GenderId ?? currentUser?.GenderId;
            _editMaritalStatusId = currentEmployee?.PersonalDetails?.MaritalStatusId ?? currentUser?.MaritalStatusId;
            _editBankAccount = currentEmployee?.PersonalDetails?.BankAccount ?? currentEmployee?.BankAccountNumber ?? currentUser?.BankAccount;
            _editBankName = currentEmployee?.PersonalDetails?.BankName ?? currentEmployee?.BankName ?? currentUser?.BankName;
            _editDependents = currentEmployee?.PersonalDetails?.NumberOfDependents ?? currentEmployee?.NumberOfDependents;
        }

        private void EnsureCollectionsInitialized()
        {
            _allEmployees ??= new List<EmployeeDto>();
            _positions ??= new List<PositionModel>();
            _banks ??= new List<BankDto>();
            _countries ??= new List<CountryModel>();
            _provinces ??= new List<LookupModel>();
            _wards ??= new List<LookupModel>();
            _genders ??= new List<LookupModel>();
            _maritalStatuses ??= new List<LookupModel>();
        }

        private void HandleLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            UpdateActiveTabFromUrl();
            StateHasChanged();
        }

        private void UpdateActiveTabFromUrl()
        {
            var uri = new Uri(Navigation.Uri);
            var queryString = uri.Query;
            
            if (queryString.Contains("tab="))
            {
                var parts = queryString.Split(new[] { "tab=" }, StringSplitOptions.None);
                if (parts.Length > 1)
                {
                    var tabValue = parts[1].Split('&')[0];
                    activeTab = tabValue.ToLower();
                    return;
                }
            }
            
            activeTab = "overview";
        }

        public void Dispose()
        {
            Navigation.LocationChanged -= HandleLocationChanged;
        }

        private async Task HandleLinkIdentity()
        {
            if (string.IsNullOrEmpty(_selectedEmployeeToLink)) return;
            
            Console.WriteLine($"[UserProfile] Linking to employee ID: {_selectedEmployeeToLink}");
            _isLinking = true;
            StateHasChanged();
            
            try
            {
                var success = await EmployeeService.LinkIdentityAsync(_selectedEmployeeToLink);
                if (success)
                {
                    Console.WriteLine("[UserProfile] Link successful, reloading...");
                    await LoadData();
                    _linkMessage = "Kết nối thành công!";
                    await Task.Delay(1000);
                    _linkMessage = null;
                }
                else
                {
                    _linkMessage = "Lỗi khi kết nối tài khoản.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserProfile] Link error: {ex.Message}");
                _linkMessage = $"Lỗi hệ thống: {ex.Message}";
            }
            finally
            {
                _isLinking = false;
                StateHasChanged();
            }
        }

        private void SetActiveTab(string tab)
        {
            activeTab = tab;
            _ = LoadTabData(tab);
            var url = Navigation.GetUriWithQueryParameter("tab", tab);
            Navigation.NavigateTo(url, replace: true);
        }

        private async Task LoadTabData(string tab)
        {
            if (currentEmployee == null && tab != "overview") return;
            var empId = currentEmployee?.Id;

            try
            {
                switch (tab.ToLower())
                {
                    case "contracts":
                        if (_contracts == null && !string.IsNullOrEmpty(empId))
                        {
                            var result = await ContractService.GetEmployeeContractsAsync(employeeId: empId, pageSize: 50);
                            _contracts = result?.Items;
                        }
                        break;
                    case "salary":
                        if (_payrolls == null && !string.IsNullOrEmpty(empId))
                        {
                            var result = await PayrollService.GetMyPayrollsAsync(employeeId: empId, year: _selectedYear);
                            _payrolls = result?.Items;
                        }
                        break;
                    case "documents":
                        if (_digitalProfile == null && !string.IsNullOrEmpty(empId))
                        {
                            _digitalProfile = await EmployeeService.GetDigitalProfileAsync(empId);
                        }
                        break;
                    case "attendance":
                        if (_attendanceDetails == null && !string.IsNullOrEmpty(empId))
                        {
                            var date = new DateTime(_selectedYear, _selectedMonth, 1);
                            var result = await AttendanceService.GetAttendanceDetailsAsync(empId, date);
                            _attendanceDetails = result?.ToList();
                        }
                        break;
                    case "leave":
                        if (_leaveBalance == null && !string.IsNullOrEmpty(empId))
                        {
                            _leaveBalance = await LeaveService.GetLeaveBalanceAsync(empId, _selectedYear);
                            // For leave requests, we might need a MyLeaveRequests or search by name/code if employeeId isn't available
                            var requests = await LeaveService.GetLeaveRequestsAsync(pageSize: 50, searchTerm: currentEmployee?.Code);
                            _leaveRequests = requests?.Items?.Where(x => x.EmployeeId == empId).ToList();
                        }
                        break;
                    case "audit":
                        if (_auditLogs == null)
                        {
                            _auditLogs = await AuditService.GetMyAuditLogsAsync();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserProfile] Error loading tab {tab}: {ex.Message}");
            }
            finally
            {
                StateHasChanged();
            }
        }

        private void ShowEditProfileModal()
        {
            _modalError = null;
            _modalSuccess = null;
            InitializeEditModel();
            isEditModalOpen = true;
        }

        private void ShowChangePasswordModal()
        {
            _modalError = null;
            _modalSuccess = null;
            _currentPassword = null;
            _newPassword = null;
            _confirmPassword = null;
            isPasswordModalOpen = true;
        }

        private void CloseModals()
        {
            isEditModalOpen = false;
            isPasswordModalOpen = false;
        }

        private async Task HandleSaveProfile()
        {
            _isSaving = true;
            _modalError = null;
            _modalSuccess = null;
            StateHasChanged();

            try 
            {
                if (currentEmployee == null) 
                {
                    if (currentUser == null) return;
                    
                    var userUpdate = new UserDto
                    {
                        Id = currentUser.Id,
                        Username = currentUser.Username,
                        Email = _editEmail ?? currentUser.Email,
                        FullName = _editFullName ?? currentUser.FullName,
                        Role = currentUser.Role,
                        AvatarUrl = currentUser.AvatarUrl,
                        Phone = _editPhone ?? currentUser.Phone,
                        IdCardNumber = _editIdCard ?? currentUser.IdCardNumber,
                        JobTitle = _editJobTitle ?? currentUser.JobTitle,
                        Hometown = _editHometown ?? currentUser.Hometown,
                        CountryId = _editCountryId,
                        ProvinceId = _editProvinceId,
                        DistrictId = _editDistrictId,
                        WardId = _editWardId,
                        StreetId = _editStreetId,
                        Street = _editStreet,
                        GenderId = _editGenderId,
                        MaritalStatusId = _editMaritalStatusId,
                        BankName = _editBankName ?? currentUser.BankName,
                        BankAccount = _editBankAccount ?? currentUser.BankAccount
                    };

                    var response = await AuthService.UpdateProfileAsync(userUpdate);
                    if (response.Success)
                    {
                        _modalSuccess = "Cập nhật hồ sơ tài khoản thành công!";
                        await LoadData();
                        await Task.Delay(1000);
                        CloseModals();
                    }
                    else
                    {
                        _modalError = response.Message;
                    }
                }
                else
                {
                    // Update employee record using the correct model structure
                    var updateRequest = new UpdateEmployeeRequest
                    {
                        Id = currentEmployee.Id,
                        FullName = _editFullName ?? "",
                        Email = _editEmail ?? "",
                        Phone = _editPhone ?? "",
                        PositionId = currentEmployee.PositionId, // Keep existing if not changing
                        PersonalDetails = new PersonalDetailsUpdateDto
                        {
                            CountryId = _editCountryId,
                            ProvinceId = _editProvinceId,
                            DistrictId = _editDistrictId,
                            WardId = _editWardId,
                            StreetId = _editStreetId,
                            Street = _editStreet,
                            GenderId = _editGenderId,
                            MaritalStatusId = _editMaritalStatusId,
                            BankName = _editBankName ?? "",
                            BankAccount = _editBankAccount ?? "",
                            Hometown = _editHometown ?? "",
                            IdCardNumber = _editIdCard ?? ""
                        }
                    };

                    // Note: UpdateEmployeeAsync returns a string error message, or null on success
                    var error = await EmployeeService.UpdateEmployeeAsync(currentEmployee.Id, updateRequest);
                    if (string.IsNullOrEmpty(error))
                    {
                        _modalSuccess = "Cập nhật hồ sơ nhân sự thành công!";
                        await LoadData();
                        await Task.Delay(1000);
                        CloseModals();
                    }
                    else
                    {
                        _modalError = error;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserProfile] Save Error: {ex.Message}");
                _modalError = ex.Message;
            }
            finally
            {
                _isSaving = false;
                StateHasChanged();
            }
        }

        private async Task HandleChangePassword()
        {
            if (string.IsNullOrEmpty(_currentPassword) || string.IsNullOrEmpty(_newPassword))
            {
                _modalError = "Vui lòng nhập đầy đủ thông tin.";
                return;
            }

            if (_newPassword != _confirmPassword)
            {
                _modalError = "Mật khẩu mới không khớp.";
                return;
            }

            _isSaving = true;
            try
            {
                var response = await AuthService.ChangePasswordAsync(_currentPassword, _newPassword);
                if (response.Success)
                {
                    _modalSuccess = "Đổi mật khẩu thành công!";
                    await Task.Delay(1500);
                    CloseModals();
                }
                else
                {
                    _modalError = response.Message ?? "Đổi mật khẩu thất bại.";
                }
            }
            catch (Exception ex)
            {
                _modalError = ex.Message;
            }
            finally
            {
                _isSaving = false;
            }
        }
    }
}
