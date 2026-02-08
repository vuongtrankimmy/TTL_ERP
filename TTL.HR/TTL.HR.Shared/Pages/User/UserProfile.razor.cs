using System;
using Microsoft.AspNetCore.Components;
using TTL.HR.Shared.Pages.User.Components;

namespace TTL.HR.Shared.Pages.User
{
    public partial class UserProfile
    {
        private string activeTab = "overview";
        private bool isEditModalOpen = false;
        private bool isPasswordModalOpen = false;

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
            // Thực hiện logic lưu ở đây (gọi API...)
            CloseModals();
        }

        private void HandleChangePassword()
        {
            // Thực hiện logic đổi mật khẩu ở đây
            CloseModals();
        }
    }
}
