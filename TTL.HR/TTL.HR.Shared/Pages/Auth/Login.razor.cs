using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

using Microsoft.JSInterop;

namespace TTL.HR.Shared.Pages.Auth
{
    public partial class Login
    {
        [Inject] public IJSRuntime JS { get; set; }

        private string _email = "admin@orgax.vn";
        private string _password = "admin";
        private bool _isLoading = false;

        public async Task HandleLogin()
        {
            if (_isLoading) return;

            _isLoading = true;
            StateHasChanged();

            // Giả lập quá trình đăng nhập
            await Task.Delay(1000);
            
            _isLoading = false;
            StateHasChanged();
            
            // Thông báo thành công
            try {
                await JS.InvokeVoidAsync("toastr.success", "Đăng nhập thành công!");
            } catch { /* Ignore if toastr not loaded */ }

            // Điều hướng về dashboard
            Navigation.NavigateTo("/dashboard");
        }
    }
}
