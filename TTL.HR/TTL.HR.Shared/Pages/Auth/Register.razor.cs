using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace TTL.HR.Shared.Pages.Auth
{
    public partial class Register
    {
        private bool _isLoading = false;

        private async Task HandleRegister()
        {
            _isLoading = true;
            // Giả lập quá trình đăng ký
            await Task.Delay(800);
            _isLoading = false;
            
            // Điều hướng về login or dashboard
            Navigation.NavigateTo("/auth/login");
        }
    }
}
