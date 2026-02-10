using System;
using Microsoft.AspNetCore.Components;

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
