using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Assets.Interfaces;
using TTL.HR.Application.Modules.Assets.Models;
using TTL.HR.Application.Modules.HumanResource.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Models;

namespace TTL.HR.Shared.Pages.Assets
{
    public partial class AssetAllocation
    {
        [Inject] public NavigationManager Navigation { get; set; } = default!;
        [Inject] public IAssetService AssetService { get; set; } = default!;
        [Inject] public IEmployeeService EmployeeService { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;

        private List<AllocationViewModel> ActiveAllocations = new();
        private List<AllocationViewModel> HistoryAllocations = new();
        private List<EmployeeModel> Employees = new();
        private List<AssetModel> AvailableAssets = new();
        
        private bool _isLoading = true;

        private bool IsAllocationModalOpen = false;
        private bool IsReturnModalOpen = false;
        
        private AllocationRequest NewAllocation = new();
        private AllocationViewModel? AllocationToReturn;

        private DateTime ReturnDate = DateTime.Today;
        private string ReturnCondition = "Bình thường";
        private string ReturnNote = "";

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            _isLoading = true;
            try
            {
                var assetsTask = AssetService.GetAssetsAsync();
                var employeesTask = EmployeeService.GetEmployeesAsync();

                await Task.WhenAll(assetsTask, employeesTask);

                var assets = await assetsTask;
                Employees = await employeesTask;

                if (assets != null)
                {
                    ActiveAllocations = assets
                        .Where(a => a.Status == "Assigned")
                        .Select(a => new AllocationViewModel
                        {
                            Id = a.Id,
                            AssetName = a.Name,
                            AssetCode = a.Code,
                            AssetType = a.Type,
                            EmployeeName = a.AssignedToName ?? "N/A",
                            Department = "Kỹ thuật", // API might need to provide this
                            EmployeeAvatar = "", // API might need to provide this
                            AssignedDate = a.PurchaseDate, // Should ideally be AssignedDate from a handover record
                            Condition = "Tốt"
                        }).ToList();

                    HistoryAllocations = assets
                        .Where(a => a.Status == "Broken" || a.Status == "Lost")
                        .Select(a => new AllocationViewModel
                        {
                            Id = a.Id,
                            AssetName = a.Name,
                            AssetCode = a.Code,
                            AssetType = a.Type,
                            EmployeeName = a.AssignedToName ?? "N/A",
                            Department = "Nghỉ việc/Trả máy",
                            EmployeeAvatar = "",
                            AssignedDate = a.PurchaseDate,
                            ReturnedDate = DateTime.Today,
                            ReturnCondition = a.Status == "Broken" ? "Hỏng" : "Bình thường",
                            Condition = "Cũ"
                        }).ToList();

                    AvailableAssets = assets.Where(a => a.Status == "Available").ToList();
                }
            }
            catch (Exception)
            {
                await JS.InvokeVoidAsync("toastr.error", "Lỗi tải dữ liệu cấp phát.");
            }
            finally
            {
                _isLoading = false;
            }
            
            NewAllocation.AssignedDate = DateTime.Today;
        }

        private string GetAssetIcon(string type) => type switch
        {
            "Laptop" or "Máy tính" => "ki-outline ki-laptop",
            "Mobile" or "Điện thoại" => "ki-outline ki-phone",
            "Vehicle" or "Xe" => "ki-outline ki-car",
            "Furniture" or "Nội thất" => "ki-outline ki-home",
            "Tablet" => "ki-outline ki-tablet",
            _ => "ki-outline ki-briefcase"
        };

        private void OpenAllocationModal() 
        { 
            NewAllocation = new AllocationRequest { AssignedDate = DateTime.Today }; 
            IsAllocationModalOpen = true; 
        }
        
        private void CloseAllocationModal() => IsAllocationModalOpen = false;

        private async Task SaveAllocation()
        {
            if (string.IsNullOrEmpty(NewAllocation.AssetId) || string.IsNullOrEmpty(NewAllocation.EmployeeId)) return;

            var success = await AssetService.AssignAssetAsync(NewAllocation.AssetId, NewAllocation.EmployeeId);
            if (success)
            {
                await JS.InvokeVoidAsync("toastr.success", "Cấp phát tài sản thành công!");
                await LoadData();
                CloseAllocationModal();
            }
            else
            {
                await JS.InvokeVoidAsync("toastr.error", "Có lỗi xảy ra khi cấp phát.");
            }
        }

        private void OpenReturnModal(AllocationViewModel item) 
        { 
            AllocationToReturn = item; 
            ReturnDate = DateTime.Today; 
            ReturnCondition = "Bình thường";
            ReturnNote = "";
            IsReturnModalOpen = true; 
        }
        
        private void CloseReturnModal() { IsReturnModalOpen = false; AllocationToReturn = null; }

        private async Task ConfirmReturn()
        {
            if (AllocationToReturn != null)
            {
                var success = await AssetService.ReturnAssetAsync(AllocationToReturn.Id, ReturnCondition, ReturnNote);
                if (success)
                {
                    await JS.InvokeVoidAsync("toastr.success", "Thu hồi tài sản thành công!");
                    await LoadData();
                    CloseReturnModal();
                }
                else
                {
                    await JS.InvokeVoidAsync("toastr.error", "Có lỗi xảy ra khi thu hồi.");
                }
            }
        }

        public class AllocationViewModel
        {
            public string Id { get; set; } = "";
            public string AssetName { get; set; } = "";
            public string AssetCode { get; set; } = "";
            public string AssetType { get; set; } = "";
            public string EmployeeName { get; set; } = "";
            public string Department { get; set; } = "";
            public string EmployeeAvatar { get; set; } = "";
            public DateTime? AssignedDate { get; set; }
            public DateTime? ReturnedDate { get; set; }
            public string Condition { get; set; } = "Tốt";
            public string ReturnCondition { get; set; } = "Bình thường";
        }

        public class AllocationRequest
        {
            [Required(ErrorMessage = "Vui lòng chọn nhân viên")]
            public string EmployeeId { get; set; } = "";
            [Required(ErrorMessage = "Vui lòng chọn tài sản")]
            public string AssetId { get; set; } = "";
            public DateTime AssignedDate { get; set; }
            public string Note { get; set; } = "";
        }
    }
}
