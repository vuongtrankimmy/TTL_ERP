using Microsoft.AspNetCore.Components;
using TTL.HR.Application.Modules.Common.Interfaces;

namespace TTL.HR.Shared.Pages.Auth
{
    public partial class ResetPassword
    {
        [Inject] public IAuthService AuthService { get; set; } = default!;
        [Inject] public NavigationManager Navigation { get; set; } = default!;

        [SupplyParameterFromQuery]
        public string? Token { get; set; }

        private string _newPassword = string.Empty;
        private string _confirmPassword = string.Empty;
        private string? _errorMessage;
        private bool _isProcessing = false;
        private bool _isSuccess = false;
        private bool _showPassword = false;
        private bool _showConfirmPassword = false;

        protected override void OnInitialized()
        {
            if (string.IsNullOrEmpty(Token))
            {
                _errorMessage = "Token không hợp lệ hoặc đã hết hạn.";
            }
        }

        private async Task HandleResetPassword()
        {
            if (string.IsNullOrEmpty(_newPassword))
            {
                _errorMessage = "Vui lòng nhập mật khẩu mới.";
                return;
            }

            if (_newPassword != _confirmPassword)
            {
                _errorMessage = "Mật khẩu xác nhận không khớp.";
                return;
            }

            if (_newPassword.Length < 8)
            {
                _errorMessage = "Mật khẩu phải có ít nhất 8 ký tự.";
                return;
            }

            _isProcessing = true;
            _errorMessage = null;
            StateHasChanged();

            try
            {
                var result = await AuthService.ResetPasswordAsync(Token!, _newPassword);
                if (result.Success)
                {
                    _isSuccess = true;
                }
                else
                {
                    _errorMessage = result.Message;
                }
            }
            catch (Exception ex)
            {
                _errorMessage = "Đã xảy ra lỗi: " + ex.Message;
            }
            finally
            {
                _isProcessing = false;
                StateHasChanged();
            }
        }

        private void TogglePasswordVisibility() => _showPassword = !_showPassword;
        private void ToggleConfirmPasswordVisibility() => _showConfirmPassword = !_showConfirmPassword;

        private void GoToLogin()
        {
            Navigation.NavigateTo("/auth/login");
        }
    }
}
