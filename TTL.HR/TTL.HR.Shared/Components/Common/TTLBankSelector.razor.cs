using Microsoft.AspNetCore.Components;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.Common.Interfaces;

namespace TTL.HR.Shared.Components.Common
{
    public partial class TTLBankSelector
    {
        [Parameter] public string? BankCode { get; set; }
        [Parameter] public EventCallback<string?> BankCodeChanged { get; set; }
        
        [Parameter] public string? Label { get; set; } = "Ngân hàng";
        [Parameter] public bool IsRequired { get; set; }
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public bool IsReadOnly { get; set; }
        [Parameter] public bool IsInvalid { get; set; }
        [Parameter] public string? ErrorMessage { get; set; }

        private List<BankDto>? _banks;
        private bool _isLoading;

        protected override async Task OnInitializedAsync()
        {
            await LoadBanks();
        }

        private async Task LoadBanks()
        {
            _isLoading = true;
            try
            {
                var result = await BankService.GetBanksAsync(new GetBanksRequest { Page = 1, PageSize = 1000 });
                if (result?.Items != null)
                {
                    _banks = result.Items
                        .Where(b => b.IsActive)
                        .OrderByDescending(b => b.Priority)
                        .ThenBy(b => b.Code)
                        .ToList();
                }
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        private async Task HandleBankChanged(ChangeEventArgs e)
        {
            BankCode = e.Value?.ToString();
            await BankCodeChanged.InvokeAsync(BankCode);
        }
    }
}
