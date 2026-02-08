using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace TTL.HR.Shared.Pages.Auth
{
    public partial class Logout
    {
        protected override async Task OnInitializedAsync()
        {
            // Giả lập xử lý đăng xuất (xóa token, clear session, vv)
            await Task.Delay(1000);
            
            // Điều hướng về trang login
            Navigation.NavigateTo("/auth/login");
        }
    }
}
