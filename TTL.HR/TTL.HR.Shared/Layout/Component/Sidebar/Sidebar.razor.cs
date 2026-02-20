using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace TTL.HR.Shared.Layout.Component.Sidebar
{
    public partial class Sidebar : IDisposable
    {
        [Inject] public IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] public TTL.HR.Application.Modules.Common.Interfaces.ISettingsService SettingsService { get; set; } = default!;

        protected override void OnInitialized()
        {
            SettingsService.OnSettingsUpdated += HandleSettingsUpdated;
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

