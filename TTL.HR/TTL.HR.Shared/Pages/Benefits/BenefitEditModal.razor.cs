using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Payroll.Interfaces;
using TTL.HR.Application.Modules.Payroll.Models;

namespace TTL.HR.Shared.Pages.Benefits
{
    public partial class BenefitEditModal
    {
        [Inject] public IBenefitService BenefitService { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;

        [Parameter] public EventCallback OnSaved { get; set; }

        public bool IsVisible { get; set; } = false;
        private BenefitModel Benefit { get; set; } = new();
        private bool IsEdit => !string.IsNullOrEmpty(Benefit.Id);
        private bool _isSaving = false;

        public async Task OpenAsync(string? id = null)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var data = await BenefitService.GetBenefitByIdAsync(id);
                if (data != null)
                {
                    Benefit = data;
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", "Không tìm thấy thông tin phúc lợi.");
                    return;
                }
            }
            else
            {
                Benefit = new BenefitModel 
                { 
                    IsActive = true, 
                    Type = "Monthly", 
                    Category = "Allowance",
                    TargetPerson = "Tất cả nhân viên" 
                };
            }
            IsVisible = true;
            StateHasChanged();
        }

        private void Close()
        {
            IsVisible = false;
            Benefit = new BenefitModel();
            StateHasChanged();
        }

        private async Task HandleValidSubmit()
        {
            _isSaving = true;
            try
            {
                bool success;
                if (IsEdit)
                {
                    success = await BenefitService.UpdateBenefitAsync(Benefit.Id, Benefit);
                }
                else
                {
                    // Generate code if empty
                    if (string.IsNullOrEmpty(Benefit.Code))
                    {
                        Benefit.Code = $"BEN-{DateTime.Now.Ticks.ToString().Substring(10)}";
                    }
                    success = await BenefitService.CreateBenefitAsync(Benefit);
                }

                if (success)
                {
                    await JS.InvokeVoidAsync("toastr.success", IsEdit ? "Cập nhật phúc lợi thành công." : "Thêm phúc lợi mới thành công.");
                    Close();
                    await OnSaved.InvokeAsync();
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", "Lưu thông tin thất bại.");
                }
            }
            catch (Exception)
            {
                await JS.InvokeVoidAsync("toastr.error", "Đã xảy ra lỗi khi lưu thông tin.");
            }
            finally
            {
                _isSaving = false;
            }
        }
    }
}
