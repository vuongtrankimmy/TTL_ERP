using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Shared.Layout.Component.Sidebar
{
    public partial class Sidebar : IDisposable
    {
        [Inject] public IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] public ISettingsService SettingsService { get; set; } = default!;
        [Inject] public IAuthService AuthService { get; set; } = default!;
        [Inject] public INavigationService NavigationService { get; set; } = default!;

        private TTL.HR.Application.Modules.Common.Models.UserDto? _currentUser;

        private List<NavItem> _menuItems = new();

        protected override async Task OnInitializedAsync()
        {
            SettingsService.OnSettingsUpdated += HandleSettingsUpdated;
            _currentUser = await AuthService.GetCurrentUserAsync();
            _menuItems = await NavigationService.GetMenuItemsAsync();
        }



        private void HandleSettingsUpdated()
        {
            StateHasChanged();
        }

        private async Task ToggleSidebar()
        {
            await JSRuntime.InvokeVoidAsync("LayoutHelper.toggleSidebar");
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("KTMenu.createInstances");
                await JSRuntime.InvokeVoidAsync("KTApp.init");
            }
        }

        public void Dispose()
        {
            SettingsService.OnSettingsUpdated -= HandleSettingsUpdated;
        }
    }
}


