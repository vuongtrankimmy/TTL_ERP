using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Shared.Pages.Auth
{
    public partial class Register
    {
        [Inject] public required IJSRuntime JS { get; set; }
        [Inject] public required IAuthService AuthService { get; set; }
        [Inject] public required NavigationManager Navigation { get; set; }

        private string _fullName = "";
        private string _email = "";
        private string _username = "";
        private string _password = "";
        private string _confirmPassword = "";
        private bool _isLoading = false;
        private bool _showPassword = false;
        private bool _showConfirmPassword = false;

        private async Task HandleRegister()
        {
            if (_isLoading) return;

            if (string.IsNullOrEmpty(_fullName) || string.IsNullOrEmpty(_email) || string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
            {
                try {
                    await JS.InvokeVoidAsync("toastr.warning", "Vui lòng nhập đầy đủ thông tin!");
                } catch { }
                return;
            }

            if (_password != _confirmPassword)
            {
                try {
                    await JS.InvokeVoidAsync("toastr.error", "Mật khẩu xác nhận không khớp!");
                } catch { }
                return;
            }

            _isLoading = true;
            StateHasChanged();

            try
            {
                var response = await AuthService.RegisterAsync(new RegisterRequest
                {
                    FullName = _fullName,
                    Email = _email,
                    Username = _username,
                    Password = _password
                });

                if (response.Success)
                {
                    try {
                        await JS.InvokeVoidAsync("toastr.success", response.Message ?? "Đăng ký thành công! Vui lòng đăng nhập.");
                    } catch { }
                    Navigation.NavigateTo("/auth/login");
                }
                else
                {
                    try {
                        await JS.InvokeVoidAsync("toastr.error", response.Message ?? "Đăng ký thất bại. Vui lòng kiểm tra lại thông tin.");
                    } catch { }
                }
            }
            catch (Exception)
            {
                try {
                    await JS.InvokeVoidAsync("toastr.error", "Có lỗi xảy ra trong quá trình đăng ký!");
                } catch { }
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }
    }
}
