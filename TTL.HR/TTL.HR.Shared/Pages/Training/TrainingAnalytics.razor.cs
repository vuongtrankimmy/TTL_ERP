using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Training.Interfaces;
using TTL.HR.Application.Modules.Training.Models;

namespace TTL.HR.Shared.Pages.Training
{
    public partial class TrainingAnalytics : ComponentBase
    {
        [Inject]
        private ITrainingService TrainingService { get; set; } = default!;

        private TrainingAnalyticsModel? _analytics;
        private bool _isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            _isLoading = true;
            StateHasChanged();

            try
            {
                _analytics = await TrainingService.GetAnalyticsAsync();
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }
    }
}
