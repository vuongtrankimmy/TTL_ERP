using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Shared.Pages.Auth
{
    public partial class Login
    {
        [Inject] public required IJSRuntime JS { get; set; }
        [Inject] public required IAuthService AuthService { get; set; }
        [Inject] public required NavigationManager Navigation { get; set; }

        private string _username = "admin";
        private string _password = "admin123";
        private bool _isLoading = false;
        private bool _showPassword = false;

        public async Task HandleLogin()
        {
            if (_isLoading) return;

            if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
            {
                try {
                    await JS.InvokeVoidAsync("toastr.warning", "Vui lòng nhập đầy đủ tài khoản và mật khẩu!");
                } catch { }
                return;
            }

            _isLoading = true;
            StateHasChanged();

            try
            {
                var response = await AuthService.LoginAsync(new LoginRequest 
                { 
                    Username = _username, 
                    Password = _password 
                });

                if (response.Success)
                {
                    // Thông báo thành công
                    try {
                        await JS.InvokeVoidAsync("toastr.success", response.Message ?? "Đăng nhập thành công!");
                    } catch { }

                    // Điều hướng về dashboard
                    Navigation.NavigateTo("/dashboard");
                }
                else
                {
                    try {
                        await JS.InvokeVoidAsync("toastr.error", response.Message ?? "Đăng nhập thất bại. Vui lòng kiểm tra lại tài khoản!");
                    } catch { }
                }
            }
            catch (Exception)
            {
                try {
                    await JS.InvokeVoidAsync("toastr.error", "Có lỗi xảy ra trong quá trình kết nối!");
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
