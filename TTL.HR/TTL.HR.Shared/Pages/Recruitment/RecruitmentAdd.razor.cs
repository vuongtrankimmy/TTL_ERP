using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using TTL.HR.Application.Modules.Recruitment;
using TTL.HR.Application.Modules.Recruitment.Models;
using Microsoft.JSInterop;

namespace TTL.HR.Shared.Pages.Recruitment
#pragma warning disable CS0169, CS0414
{
    public partial class RecruitmentAdd
    {
        [Inject] public IRecruitmentApplication RecruitmentApp { get; set; } = default!;
        [Inject] public TTL.HR.Application.Modules.Organization.Interfaces.IDepartmentService DepartmentService { get; set; } = default!;
        [Inject] public NavigationManager Navigation { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;

        [Parameter, SupplyParameterFromQuery]
        public string? Id { get; set; }

        private JobDetail Model = new();
        private List<TTL.HR.Application.Modules.Organization.Models.DepartmentModel> _departments = new();
        private bool _isEdit => !string.IsNullOrEmpty(Id);
        private bool _isLoading = false;
        private bool _isSaving = false;
        private System.Collections.Generic.Dictionary<string, string> _validationErrors = new();

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;
            try
            {
                _departments = await DepartmentService.GetDepartmentsAsync();

                if (_isEdit)
                {
                    var result = await RecruitmentApp.GetJobDetailsAsync(Id!);
                    if (result != null)
                    {
                        Model = result;
                    }
                    else
                    {
                        await JS.InvokeVoidAsync("toastr.error", "Không tìm thấy tin tuyển dụng.");
                        Navigation.NavigateTo("/recruitment");
                    }
                }
                else
                {
                    Model = new JobDetail { EndDate = DateTime.Today.AddMonths(1) };
                }
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("toastr.error", $"Lỗi tải dữ liệu: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task SaveRecruitment()
        {
            _isSaving = true;
            _validationErrors.Clear();
            StateHasChanged();

            try 
            {
                // Basic client-side validation
                if (string.IsNullOrWhiteSpace(Model.Title)) _validationErrors["Title"] = "Tiêu đề tuyển dụng không được để trống.";
                if (string.IsNullOrWhiteSpace(Model.Code)) _validationErrors["Code"] = "Mã tin không được để trống.";
                if (string.IsNullOrWhiteSpace(Model.DepartmentId)) _validationErrors["Department"] = "Vui lòng chọn phòng ban.";
                if (Model.Quantity <= 0) _validationErrors["Quantity"] = "Số lượng tuyển phải lớn hơn 0.";
                if (Model.EndDate == default || Model.EndDate < DateTime.Today) _validationErrors["EndDate"] = "Hạn nộp hồ sơ không hợp lệ.";

                if (_validationErrors.Any())
                {
                    await JS.InvokeVoidAsync("toastr.error", "Vui lòng kiểm tra lại các trường thông tin.");
                    await FocusFirstError();
                    return;
                }

                // Set DepartmentName based on selected ID
                var dept = _departments.FirstOrDefault(d => d.Id == Model.DepartmentId);
                if (dept != null) Model.DepartmentName = dept.Name;

                bool success;
                if (_isEdit)
                {
                    success = await RecruitmentApp.UpdateJobPostingAsync(Id!, Model);
                }
                else
                {
                    success = await RecruitmentApp.PostNewJobAsync(Model);
                }

                if (success)
                {
                    await JS.InvokeVoidAsync("toastr.success", "Đã lưu tin tuyển dụng thành công!");
                    Navigation.NavigateTo("/recruitment");
                }
                else
                {
                    _validationErrors["General"] = "Có lỗi xảy ra khi lưu tin tuyển dụng. Vui lòng thử lại sau.";
                    await JS.InvokeVoidAsync("toastr.error", "Lưu thất bại.");
                }
            }
            catch (Exception ex)
            {
                _validationErrors["General"] = $"Lỗi hệ thống: {ex.Message}";
                _validationErrors["DebugInfo"] = ex.StackTrace ?? "";
                await JS.InvokeVoidAsync("toastr.error", "Có lỗi hệ thống.");
            }
            finally
            {
                _isSaving = false;
                StateHasChanged();
            }
        }

        private async Task FocusFirstError()
        {
            try
            {
                var firstField = _validationErrors.Keys.FirstOrDefault(k => k != "General");
                if (!string.IsNullOrEmpty(firstField))
                {
                    var elementId = firstField.ToLower() switch
                    {
                        "title" => "recruitment_title",
                        "code" => "recruitment_code",
                        "department" => "recruitment_dept",
                        "quantity" => "recruitment_qty",
                        "enddate" => "recruitment_enddate",
                        _ => firstField
                    };
                    await JS.InvokeVoidAsync("eval", $"document.getElementById('{elementId}')?.focus()");
                }
            }
            catch { }
        }
    }
}
