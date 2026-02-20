using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.HumanResource.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Models;

namespace TTL.HR.Shared.Pages.User
{
    public partial class UserProfile
    {
        [Inject] private IAuthService AuthService { get; set; } = default!;
        [Inject] private IEmployeeService EmployeeService { get; set; } = default!;

        private UserDto? currentUser;
        private EmployeeModel? currentEmployee;
        private string? employeeId;

        private string activeTab = "overview";
        private bool isEditModalOpen = false;
        private bool isPasswordModalOpen = false;
        private bool _showCurrentPassword = false;
        private bool _showNewPassword = false;
        private bool _showConfirmNewPassword = false;
        private bool _isLoadingProfile = true;

        protected override async Task OnInitializedAsync()
        {
            currentUser = await AuthService.GetCurrentUserAsync();
            currentEmployee = await EmployeeService.GetMyEmployeeAsync();
            employeeId = currentEmployee?.Id ?? currentUser?.Id;
            _isLoadingProfile = false;
        }

        private void SetActiveTab(string tab)
        {
            activeTab = tab;
        }

        private void ShowEditProfileModal()
        {
            isEditModalOpen = true;
            isPasswordModalOpen = false;
        }

        private void ShowChangePasswordModal()
        {
            isPasswordModalOpen = true;
            isEditModalOpen = false;
        }

        private void CloseModals()
        {
            isEditModalOpen = false;
            isPasswordModalOpen = false;
        }

        private void HandleSaveProfile()
        {
            CloseModals();
        }

        private void HandleChangePassword()
        {
            CloseModals();
        }
    }
}
