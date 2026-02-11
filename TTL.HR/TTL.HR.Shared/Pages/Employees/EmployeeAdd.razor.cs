using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.HumanResource.Models;
using TTL.HR.Application.Modules.Organization.Models;
using TTL.HR.Shared.Components.Common;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Organization.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Entities = TTL.HR.Application.Modules.HumanResource.Entities;

namespace TTL.HR.Shared.Pages.Employees
{
    public partial class EmployeeAdd
    {
        [Inject] public IMasterDataService MasterDataService { get; set; } = default!;
        [Inject] public IDepartmentService DepartmentService { get; set; } = default!;
        [Inject] public IPositionService PositionService { get; set; } = default!;
        [Inject] public IEmployeeService EmployeeService { get; set; } = default!;
        [Inject] public IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] public NavigationManager Navigation { get; set; } = default!;

        private List<LookupModel> genderLookups = new();
        private List<LookupModel> maritalStatusLookups = new();
        private List<LookupModel> employeeStatusLookups = new();
        private List<LookupModel> contractTypeLookups = new();
        private List<DepartmentModel> departments = new();
        private List<PositionModel> positions = new();

        private EmployeeModel newEmployee = new()
        {
            IsActive = true,
            JoinDate = DateTime.Now,
            DOB = new DateTime(1995, 1, 1)
        };

        protected override async Task OnInitializedAsync()
        {
            genderLookups = await MasterDataService.GetCachedLookupsAsync("Gender");
            maritalStatusLookups = await MasterDataService.GetCachedLookupsAsync("MaritalStatus");
            employeeStatusLookups = await MasterDataService.GetCachedLookupsAsync("EmployeeStatus");
            contractTypeLookups = await MasterDataService.GetCachedLookupsAsync("ContractType");
            departments = await DepartmentService.GetDepartmentsAsync();
            positions = await PositionService.GetPositionsAsync();

            // Set defaults if lists are not empty
            if (newEmployee.Gender == string.Empty) newEmployee.Gender = genderLookups.FirstOrDefault()?.Name ?? "Nam";
            if (newEmployee.Status == string.Empty) newEmployee.Status = employeeStatusLookups.FirstOrDefault()?.Name ?? "Thử việc";
            if (newEmployee.ContractType == string.Empty) newEmployee.ContractType = contractTypeLookups.FirstOrDefault()?.Name ?? "Hợp đồng thử việc";
            if (newEmployee.DeptId == string.Empty) newEmployee.DeptId = departments.FirstOrDefault()?.Id ?? string.Empty;
            if (newEmployee.PositionId == string.Empty) newEmployee.PositionId = positions.FirstOrDefault()?.Id ?? string.Empty;
        }

        private CccdScanner cccdScanner;

        private async Task ScanCCCD()
        {
            var scannedData = await cccdScanner.ScanAsync();
            
            if (scannedData != null)
            {
                newEmployee.Name = scannedData.Name;
                newEmployee.Code = "NV" + new Random().Next(100, 999);
                newEmployee.IdCard = scannedData.IdCard;
                newEmployee.DOB = scannedData.DOB ?? DateTime.Now;
                newEmployee.Address = scannedData.Address;
                newEmployee.Hometown = scannedData.Hometown;
                newEmployee.Gender = scannedData.Gender;
                newEmployee.Nationality = scannedData.Nationality;
                newEmployee.Ethnicity = scannedData.Ethnicity;
                newEmployee.Religion = scannedData.Religion;
                newEmployee.CccdIssueDate = scannedData.IssueDate;
                newEmployee.CccdIssuePlace = scannedData.IssuePlace;
                newEmployee.PlaceOfOrigin = scannedData.Hometown;
                newEmployee.Residence = scannedData.Address;
                
                // Tự sinh email và điện thoại demo
                newEmployee.Email = scannedData.Name.Replace(" ", ".").ToLower() + "@gmail.com";
                newEmployee.Phone = "0" + new Random().Next(900000000, 999999999);
                
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
            try 
            {
                var employeeEntity = new Entities.Employee
                {
                    Code = newEmployee.Code,
                    FullName = newEmployee.Name,
                    Email = newEmployee.Email,
                    CompanyEmail = newEmployee.CompanyEmail,
                    Phone = newEmployee.Phone,
                    AvatarUrl = newEmployee.Avatar,
                    DepartmentId = newEmployee.DeptId,
                    PositionId = newEmployee.PositionId,
                    JoinDate = newEmployee.OfficialJoinDate ?? newEmployee.JoinDate,
                    Status = Enum.TryParse<Entities.EmployeeStatus>(newEmployee.Status, true, out var status) ? status : Entities.EmployeeStatus.Probation,
                    Type = Enum.TryParse<Entities.EmploymentType>(newEmployee.ContractType, true, out var type) ? type : Entities.EmploymentType.FullTime,
                    PersonalDetails = new Entities.PersonalInfo
                    {
                        DOB = newEmployee.DOB,
                        Gender = newEmployee.Gender,
                        Address = newEmployee.Address,
                        Hometown = newEmployee.Hometown,
                        IdCardNumber = newEmployee.IdCard,
                        IdCardPlace = newEmployee.CccdIssuePlace,
                        TaxCode = newEmployee.TaxId,
                        BankAccount = newEmployee.BankAccountNumber,
                        BankName = newEmployee.BankName
                    }
                };

                var created = await EmployeeService.CreateEmployeeAsync(employeeEntity);

                if (created != null)
                {
                    await JSRuntime.InvokeVoidAsync("Swal.fire", new
                    {
                        title = "Thành công!",
                        text = "Hồ sơ nhân viên " + created.FullName + " (" + created.Code + ") đã được lưu thành công.",
                        icon = "success",
                        confirmButtonText = "Đóng"
                    });
                    Navigation.NavigateTo("/employees");
                }
                else 
                {
                    await JSRuntime.InvokeVoidAsync("Swal.fire", "Lỗi", "Không thể lưu hồ sơ nhân viên. Vui lòng thử lại.", "error");
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("Swal.fire", "Lỗi hệ thống", ex.Message, "error");
            }
        }
    }
}
