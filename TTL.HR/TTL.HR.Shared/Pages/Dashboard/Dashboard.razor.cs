using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Dashboard.Interfaces;
using TTL.HR.Application.Modules.Dashboard.Models;

namespace TTL.HR.Shared.Pages.Dashboard
{
    public partial class Dashboard
    {
        [Inject] public IDashboardService DashboardService { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;
        
        private DashboardOverviewModel? _overview;
        private bool _isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _overview = await DashboardService.GetOverviewAsync();
            }
            catch (System.Exception ex)
            {
                // Logic to handle error will be shown only after first render via toastr
                // or via the UI template defined in .razor
                System.Console.WriteLine($"Dashboard Error: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && _overview == null && !_isLoading)
            {
                try {
                    await JS.InvokeVoidAsync("toastr.error", "Không thể tải dữ liệu tổng quan. Vui lòng kiểm tra kết nối API.");
                } catch { }
            }
        }
    }
}
