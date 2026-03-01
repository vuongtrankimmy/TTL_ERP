using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using TTL.HR.Application.Modules.Common.Interfaces;

namespace TTL.HR.Shared.Pages.Auth
{
    public partial class ConfirmEmail
    {
        [Inject] public IAuthService AuthService { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }

        [Parameter, SupplyParameterFromQuery]
        public string Token { get; set; }

        private bool _isProcessing = true;
        private bool _isSuccess = false;
        private string _errorMessage = "Token không hợp lệ hoặc đã hết hạn.";

        protected override async Task OnInitializedAsync()
        {
            if (string.IsNullOrEmpty(Token))
            {
                _isProcessing = false;
                _isSuccess = false;
                _errorMessage = "Không tìm thấy token xác thực.";
                return;
            }

            try
            {
                var response = await AuthService.ConfirmEmailAsync(Token);
                _isSuccess = response.Success;
                if (!_isSuccess)
                {
                    _errorMessage = response.Message ?? "Xác thực email thất bại. Token có thể đã hết hạn hoặc không tồn tại.";
                }
            }
            catch (Exception ex)
            {
                _isSuccess = false;
                _errorMessage = "Có lỗi xảy ra trong quá trình kết nối với hệ thống.";
            }
            finally
            {
                _isProcessing = false;
            }
        }
    }
}
