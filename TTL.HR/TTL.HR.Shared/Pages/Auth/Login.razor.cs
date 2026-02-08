using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace TTL.HR.Shared.Pages.Auth
{
    public partial class Login
    {
        private string _email = "admin@orgax.vn";
        private string _password = "admin";
        private bool _isLoading = false;

        private async Task HandleLogin()
        {
            _isLoading = true;
            // Giả lập quá trình đăng nhập
            await Task.Delay(800);
            _isLoading = false;
            
            // Điều hướng về dashboard
            Navigation.NavigateTo("/dashboard");
        }
    }
}
