using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Common.Interfaces;

namespace TTL.HR.Shared.Pages.Auth
{
    public partial class ForgotPassword
    {
        [Inject] private IAuthService AuthService { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        private string _email = string.Empty;
        private bool _isLoading = false;
        private bool _isSuccess = false;

        private async Task HandleSubmit()
        {
            if (string.IsNullOrWhiteSpace(_email))
            {
                await JSRuntime.InvokeVoidAsync("toastr.error", "Vui lòng nhập địa chỉ email");
                return;
            }

            _isLoading = true;
            StateHasChanged();

            try
            {
                var result = await AuthService.RequestPasswordResetAsync(_email);
                if (result.Success)
                {
                    _isSuccess = true;
                    await JSRuntime.InvokeVoidAsync("toastr.success", "Yêu cầu đã được gửi thành công");
                }
                else
                {
                    await JSRuntime.InvokeVoidAsync("toastr.error", result.Message ?? "Gửi yêu cầu thất bại. Vui lòng thử lại.");
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("toastr.error", "Lỗi kết nối hệ thống. Vui lòng thử lại sau.");
                Console.WriteLine($"[ForgotPasswordError]: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }
    }
}
