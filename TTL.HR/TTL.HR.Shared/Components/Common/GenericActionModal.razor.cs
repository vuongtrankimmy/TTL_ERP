using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace TTL.HR.Shared.Components.Common
{
    public partial class GenericActionModal
    {
        [Parameter] public bool IsVisible { get; set; }
        [Parameter] public string Title { get; set; } = "Xác nhận hành động";
        [Parameter] public string Heading { get; set; } = "Bạn có chắc chắn?";
        [Parameter] public string Message { get; set; } = "";
        [Parameter] public string Color { get; set; } = "primary"; // primary, success, danger, warning
        [Parameter] public string Icon { get; set; } = "ki-outline ki-information-5";
        [Parameter] public string ConfirmLabel { get; set; } = "Xác nhận";
        [Parameter] public string CancelLabel { get; set; } = "Hủy bỏ";
        [Parameter] public string? RequiredVerificationText { get; set; }
        [Parameter] public bool ShowReasonInput { get; set; } = false;
        [Parameter] public string ReasonPlaceholder { get; set; } = "Nhập lý do...";
        [Parameter] public bool IsReasonRequired { get; set; } = false;
        
        [Parameter] public EventCallback<string> OnConfirmed { get; set; }
        [Parameter] public EventCallback OnCancelled { get; set; }

        private string _verificationInput = "";
        private string _reasonInput = "";

        private bool IsConfirmDisabled => 
            (!string.IsNullOrEmpty(RequiredVerificationText) && _verificationInput != RequiredVerificationText) ||
            (IsReasonRequired && string.IsNullOrWhiteSpace(_reasonInput));

        private async Task OnCancel()
        {
            _verificationInput = "";
            _reasonInput = "";
            if (OnCancelled.HasDelegate)
            {
                await OnCancelled.InvokeAsync();
            }
        }

        private async Task OnConfirm()
        {
            if (!IsConfirmDisabled)
            {
                var reason = _reasonInput;
                _verificationInput = "";
                _reasonInput = "";
                if (OnConfirmed.HasDelegate)
                {
                    await OnConfirmed.InvokeAsync(reason);
                }
            }
        }
    }
}
