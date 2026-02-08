using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
using TTL.HR.Shared.Models;
using TTL.HR.Shared.Components.Common;

namespace TTL.HR.Shared.Pages.Employees
{
    public partial class EmployeeAdd
    {
        private EmployeeModel newEmployee = new()
        {
            Gender = "Nam",
            Dept = "Kỹ thuật",
            Status = "Thử việc",
            ContractType = "Hợp đồng thử việc",
            IsActive = true,
            JoinDate = DateTime.Now,
            DOB = new DateTime(1995, 1, 1)
        };

        private CccdScanner cccdScanner;

        private async Task ScanCCCD()
        {
            var scannedData = await cccdScanner.ScanAsync();
            
            if (scannedData != null)
            {
                newEmployee.Name = scannedData.Name;
                newEmployee.Id = "NV" + new Random().Next(100, 999);
                newEmployee.DOB = scannedData.DOB ?? DateTime.Now;
                newEmployee.IdCard = scannedData.IdCard;
                newEmployee.Address = scannedData.Address;
                newEmployee.Hometown = scannedData.Hometown;
                newEmployee.Gender = scannedData.Gender;
                // Tự sinh email demo
                newEmployee.Name = scannedData.Name.ToUpper();
                newEmployee.Email = "manh.tv@ttlloc.vn";
                newEmployee.Phone = "0984 555 666";
                
                await JSRuntime.InvokeVoidAsync("Swal.fire", new {
                    title = "Thành công!",
                    text = "Dữ liệu đã được điền tự động vào biểu mẫu.",
                    icon = "success",
                    timer = 1500,
                    showConfirmButton = false
                });
                
                StateHasChanged();
            }
        }

        private async Task Submit()
        {
            if (string.IsNullOrEmpty(newEmployee.Name))
            {
                await JSRuntime.InvokeVoidAsync("Swal.fire", "Thông báo", "Vui lòng nhập họ tên nhân viên", "warning");
                return;
            }

            await JSRuntime.InvokeVoidAsync("Swal.fire", new
            {
                title = "Đang xử lý...",
                text = "Hệ thống đang khởi tạo hồ sơ nhân sự mới",
                allowOutsideClick = false,
                showConfirmButton = false
            });
            await JSRuntime.InvokeVoidAsync("Swal.showLoading");
            await Task.Delay(1500);

            await JSRuntime.InvokeVoidAsync("Swal.fire", new
            {
                title = "Thành công!",
                text = "Hồ sơ nhân viên " + newEmployee.Name + " (" + newEmployee.Id + ") đã được lưu thành công.",
                icon = "success",
                confirmButtonText = "Đóng"
            });

            Navigation.NavigateTo("/employees");
        }
    }
}
