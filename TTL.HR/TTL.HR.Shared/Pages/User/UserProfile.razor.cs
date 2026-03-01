using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.HumanResource.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Models;

namespace TTL.HR.Shared.Pages.User
{
    public partial class UserProfile : IDisposable
    {
        [Inject] private IAuthService AuthService { get; set; } = default!;
        [Inject] private IEmployeeService EmployeeService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        private UserDto? currentUser;
        private EmployeeModel? currentEmployee;
        private string activeTab = "overview";
        private string? employeeId;
        private bool _isLoadingProfile = true;
        private List<EmployeeDto>? _allEmployees;
        private string? _selectedEmployeeToLink;
        private string? _linkMessage;

        // Form binds
        private string? _editFullName;
        private string? _editPhone;
        private string? _editEmail;
        private string? _editIdCard;
        private string? _editHometown;
        private string? _editBankAccount;
        private string? _editBankName;
        private int _editDependents;
        
        private string? _currentPassword;
        private string? _newPassword;
        private string? _confirmPassword;
        private string? _modalError;
        private string? _modalSuccess;

        [SupplyParameterFromQuery(Name = "tab")] public string? TabFromUrl { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Navigation.LocationChanged += HandleLocationChanged;
                
                currentUser = await AuthService.GetCurrentUserAsync();
                currentEmployee = await EmployeeService.GetMyEmployeeAsync();
                employeeId = currentEmployee?.Id;
                
                UpdateActiveTabFromUrl();

                // Sync form data
                _editFullName = currentEmployee?.FullName ?? currentUser?.FullName;
                _editPhone = currentEmployee?.Phone;
                _editEmail = currentEmployee?.Email ?? currentUser?.Email;
                _editIdCard = currentEmployee?.IdCard;
                _editHometown = currentEmployee?.Hometown;
                _editBankAccount = currentEmployee?.BankAccountNumber;
                _editBankName = currentEmployee?.BankName;
                _editDependents = currentEmployee?.NumberOfDependents ?? 0;

                try
                {
                    _allEmployees = await EmployeeService.GetEmployeesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[UserProfile] Error loading employee list: {ex.Message}");
                    _allEmployees = new List<EmployeeDto>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserProfile] Critical Init Error: {ex.Message}");
            }
            finally
            {
                _isLoadingProfile = false;
            }
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
                // Simple manual parse for "tab=xxx"
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
            var success = await EmployeeService.LinkIdentityAsync(_selectedEmployeeToLink);
            if (success)
            {
                _linkMessage = "Đã kết nối tài khoản thành công! Trang sẽ tải lại...";
                StateHasChanged();
                await Task.Delay(1500);
                currentEmployee = await EmployeeService.GetMyEmployeeAsync();
                employeeId = currentEmployee?.Id;
                _editFullName = currentEmployee?.FullName ?? currentUser?.FullName;
                _editPhone = currentEmployee?.Phone;
                _editEmail = currentEmployee?.Email ?? currentUser?.Email;
                _editIdCard = currentEmployee?.IdCard;
                _editHometown = currentEmployee?.Hometown;
                _editBankAccount = currentEmployee?.BankAccountNumber;
                _editBankName = currentEmployee?.BankName;
                _editDependents = currentEmployee?.NumberOfDependents ?? 0;
                SetActiveTab("overview");
                _linkMessage = null;
                StateHasChanged();
            }
        }

        private void SetActiveTab(string tab)
        {
            activeTab = tab;
            var url = Navigation.GetUriWithQueryParameter("tab", tab);
            Navigation.NavigateTo(url, replace: true);
        }

        private void ShowEditProfileModal()
        {
            _modalError = null;
            _modalSuccess = null;
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
            if (currentEmployee == null) return;
            var request = new UpdateEmployeeRequest 
            { 
                FullName = _editFullName ?? "", 
                Phone = _editPhone ?? "", 
                Email = _editEmail ?? "",
                PersonalDetails = new PersonalDetailsUpdateDto
                {
                    IdCardNumber = _editIdCard ?? "",
                    Hometown = _editHometown ?? "",
                    BankAccount = _editBankAccount ?? "",
                    BankName = _editBankName ?? "",
                    // Preserve other fields that aren't editable here but might be in the model
                    Gender = currentEmployee.PersonalDetails?.Gender ?? "",
                    Address = currentEmployee.PersonalDetails?.Address ?? "",
                    TaxCode = currentEmployee.PersonalDetails?.TaxCode ?? "",
                    Nationality = currentEmployee.PersonalDetails?.Nationality ?? "Việt Nam",
                    Ethnicity = currentEmployee.PersonalDetails?.Ethnicity ?? "Kinh",
                    Religion = currentEmployee.PersonalDetails?.Religion ?? "Không",
                    MaritalStatus = currentEmployee.PersonalDetails?.MaritalStatus ?? "Độc thân",
                    SocialInsuranceId = currentEmployee.PersonalDetails?.SocialInsuranceId ?? ""
                }
            };
            var error = await EmployeeService.UpdateEmployeeAsync(currentEmployee.Id, request);
            if (string.IsNullOrEmpty(error))
            {
                _modalSuccess = "Cập nhật thành công!";
                currentEmployee = await EmployeeService.GetMyEmployeeAsync();
                StateHasChanged();
                await Task.Delay(1000);
                CloseModals();
            }
            else _modalError = error;
        }

        private async Task HandleChangePassword()
        {
            _modalError = null;
            _modalSuccess = null;
            if (string.IsNullOrEmpty(_currentPassword) || string.IsNullOrEmpty(_newPassword)) { _modalError = "Vui lòng nhập đầy đủ."; return; }
            if (_newPassword != _confirmPassword) { _modalError = "Mật khẩu không khớp."; return; }

            var result = await AuthService.ChangePasswordAsync(_currentPassword, _newPassword);
            if (result.Success)
            {
                _modalSuccess = "Đổi mật khẩu thành công!";
                StateHasChanged();
                await Task.Delay(1000);
                CloseModals();
            }
            else _modalError = result.Message ?? "Thất bại.";
        }

        private bool isEditModalOpen = false;
        private bool isPasswordModalOpen = false;
    }
}
