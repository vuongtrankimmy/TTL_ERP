using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace TTL.HR.Shared.Components.Common
{
    public partial class DeleteConfirmationModal
    {
        [Parameter] public bool IsVisible { get; set; }
        [Parameter] public string Title { get; set; } = "Xác nhận xóa";
        [Parameter] public string Message { get; set; } = "Bạn đang yêu cầu xóa";
        [Parameter] public string ItemName { get; set; } = "";
        [Parameter] public EventCallback OnConfirmed { get; set; }
        [Parameter] public EventCallback OnCancelled { get; set; }

        private string _confirmationInput = "";

        protected override void OnParametersSet()
        {
            // Reset input when modal becomes visible
            if (IsVisible && string.IsNullOrEmpty(_confirmationInput))
            {
                // Optionally reset input if needed logic here, 
                // but simpler is to reset when IsVisible changes to false -> true.
                // However, IsVisible is a parameter.
                // Let's rely on the parent or handle explicit Show logic.
                // Actually, a simpler way is to clear input when calling OnCancel or OnConfirm.
            }
        }

        private async Task OnCancel()
        {
            _confirmationInput = ""; // Clear input
            if (OnCancelled.HasDelegate)
            {
                await OnCancelled.InvokeAsync();
            }
        }

        private async Task OnConfirm()
        {
            if (_confirmationInput == ItemName)
            {
                _confirmationInput = ""; // Clear input for next time
                if (OnConfirmed.HasDelegate)
                {
                    await OnConfirmed.InvokeAsync();
                }
            }
        }
    }
}
