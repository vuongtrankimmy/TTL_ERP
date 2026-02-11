using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using TTL.HR.Application.Modules.Common.Interfaces;

namespace TTL.HR.Shared.Pages.Auth
{
    public partial class Logout
    {
        [Inject] public IAuthService AuthService { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await AuthService.LogoutAsync();
            
            // Điều hướng về trang login
            Navigation.NavigateTo("/auth/login");
        }
    }
}
