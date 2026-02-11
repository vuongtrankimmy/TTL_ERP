using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Dashboard.Interfaces;
using TTL.HR.Application.Modules.Dashboard.Models;

namespace TTL.HR.Shared.Pages.Dashboard
{
    public partial class Dashboard
    {
        [Inject] public IDashboardService DashboardService { get; set; } = default!;
        
        private DashboardOverviewModel? _overview;
        private bool _isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _overview = await DashboardService.GetOverviewAsync();
            }
            finally
            {
                _isLoading = false;
            }
        }
    }
}
