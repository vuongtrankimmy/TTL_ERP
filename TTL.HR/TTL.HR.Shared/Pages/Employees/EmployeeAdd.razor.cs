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
        private List<EmployeeDto> allEmployees = new();

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
            var employees = await EmployeeService.GetEmployeesAsync();
            allEmployees = employees;

            // Set defaults if lists are not empty
            if (string.IsNullOrEmpty(newEmployee.Gender)) newEmployee.Gender = genderLookups.FirstOrDefault()?.Name ?? "Nam";
            if (string.IsNullOrEmpty(newEmployee.StatusId)) newEmployee.StatusId = employeeStatusLookups.FirstOrDefault()?.Id ?? string.Empty;
            if (string.IsNullOrEmpty(newEmployee.ContractTypeId)) newEmployee.ContractTypeId = contractTypeLookups.FirstOrDefault()?.Id ?? string.Empty;
            if (string.IsNullOrEmpty(newEmployee.DeptId)) newEmployee.DeptId = departments.FirstOrDefault()?.Id ?? string.Empty;
            if (string.IsNullOrEmpty(newEmployee.PositionId)) newEmployee.PositionId = positions.FirstOrDefault()?.Id ?? string.Empty;
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
                newEmployee.CccdIssueDate = DateTime.TryParse(scannedData.IssueDate, out var iDate) ? iDate : null;
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
            // 1. Client-side Validation
            var errors = new List<string>();
            if (string.IsNullOrEmpty(newEmployee.Name)) errors.Add("Họ tên nhân viên");
            if (string.IsNullOrEmpty(newEmployee.Email)) errors.Add("Email");
            if (string.IsNullOrEmpty(newEmployee.Phone)) errors.Add("Số điện thoại");
            if (string.IsNullOrEmpty(newEmployee.IdCard)) errors.Add("Số CCCD");
            if (string.IsNullOrEmpty(newEmployee.DeptId)) errors.Add("Phòng ban");
            if (string.IsNullOrEmpty(newEmployee.PositionId)) errors.Add("Chức vụ");
            if (string.IsNullOrEmpty(newEmployee.StatusId)) errors.Add("Trạng thái");
            if (string.IsNullOrEmpty(newEmployee.ContractTypeId)) errors.Add("Loại hợp đồng");

            if (errors.Any())
            {
                await JSRuntime.InvokeVoidAsync("Swal.fire", "Thiếu thông tin bắt buộc", 
                    $"Vui lòng nhập đầy đủ: {string.Join(", ", errors)}", "warning");
                return;
            }

            // 2. Show Loading Animation
            await JSRuntime.InvokeVoidAsync("Swal.fire", new
            {
                title = "Đang xử lý...",
                text = "Hệ thống đang khởi tạo hồ sơ nhân sự mới",
                allowOutsideClick = false,
                showConfirmButton = false,
                didOpen = "Swal.showLoading()" // Ensure loading spinner shows immediately
            });
            await JSRuntime.InvokeVoidAsync("Swal.showLoading"); // Fallback check
            await Task.Delay(500); // Give UI time to render loading state

            try 
            {
                // 3. Map Data to Entity
                var employeeEntity = new Entities.Employee
                {
                    Code = string.IsNullOrEmpty(newEmployee.Code) ? "NV" + new Random().Next(1000, 9999) : newEmployee.Code,
                    FullName = newEmployee.Name,
                    Email = newEmployee.Email,
                    CompanyEmail = newEmployee.CompanyEmail,
                    Phone = newEmployee.Phone,
                    AvatarUrl = newEmployee.Avatar,
                    
                    // Validate Foreign Keys
                    DepartmentId = IsValidObjectId(newEmployee.DeptId) ? newEmployee.DeptId : null,
                    PositionId = IsValidObjectId(newEmployee.PositionId) ? newEmployee.PositionId : null,
                    ReportToId = IsValidObjectId(newEmployee.ReportToId) ? newEmployee.ReportToId : null,
                    StatusId = IsValidObjectId(newEmployee.StatusId) ? newEmployee.StatusId : null,
                    ContractTypeId = IsValidObjectId(newEmployee.ContractTypeId) ? newEmployee.ContractTypeId : null,
                    
                    JoinDate = newEmployee.OfficialJoinDate ?? newEmployee.JoinDate ?? DateTime.Now,
                    Salary = ParseSalary(newEmployee.Salary),
                    ContractEndDate = newEmployee.ContractExpiry ?? newEmployee.ContractEndDate,
                    Workplace = newEmployee.Workplace,
                    IsAccountActive = newEmployee.IsActive,

                    PersonalDetails = new Entities.PersonalInfo
                    {
                        DOB = newEmployee.DOB,
                        Gender = newEmployee.Gender,
                        Address = newEmployee.Address,
                        Hometown = newEmployee.Hometown,
                        IdCardNumber = newEmployee.IdCard,
                        IdCardIssueDate = newEmployee.CccdIssueDate,
                        IdCardPlace = newEmployee.CccdIssuePlace,
                        TaxCode = newEmployee.TaxId,
                        BankAccount = newEmployee.BankAccountNumber,
                        BankName = newEmployee.BankName,
                        Nationality = string.IsNullOrEmpty(newEmployee.Nationality) ? "Việt Nam" : newEmployee.Nationality,
                        Ethnicity = string.IsNullOrEmpty(newEmployee.Ethnicity) ? "Kinh" : newEmployee.Ethnicity,
                        Religion = string.IsNullOrEmpty(newEmployee.Religion) ? "Không" : newEmployee.Religion,
                        MaritalStatus = string.IsNullOrEmpty(newEmployee.MaritalStatus) ? "Độc thân" : newEmployee.MaritalStatus,
                        PlaceOfOrigin = newEmployee.PlaceOfOrigin,
                        Residence = newEmployee.Residence,
                        SocialInsuranceId = newEmployee.SocialInsuranceId
                    },
                    
                    EmergencyContact = new Entities.EmergencyContact
                    {
                        Name = newEmployee.EmergencyContactName,
                        Relation = newEmployee.EmergencyContactRelation,
                        Phone = newEmployee.EmergencyContactPhone
                    }
                };

                // 4. Send API Request
                var result = await EmployeeService.CreateEmployeeAsync(employeeEntity);
                
                // Ensure loading is closed
                await JSRuntime.InvokeVoidAsync("Swal.close");

                // 5. Handle Response
                if (!string.IsNullOrEmpty(result) && IsValidObjectId(result))
                {
                    await JSRuntime.InvokeVoidAsync("Swal.fire", new
                    {
                        title = "Thành công!",
                        text = $"Hồ sơ nhân viên {newEmployee.Name} ({newEmployee.Code}) đã được tạo.",
                        icon = "success",
                        confirmButtonText = "Hoàn tất"
                    });
                    Navigation.NavigateTo("/employees");
                }
                else 
                {
                    var errorMsg = !string.IsNullOrEmpty(result) ? result : "Không thể lưu hồ sơ. Vui lòng kiểm tra lại dữ liệu hoặc thử lại.";
                    if (errorMsg.Length > 500) errorMsg = errorMsg.Substring(0, 500) + "..."; // Truncate long error messages (likely HTML)
                    await JSRuntime.InvokeVoidAsync("Swal.fire", "Lỗi", errorMsg, "error");
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("Swal.fire", "Lỗi hệ thống", ex.Message, "error");
            }
        }

        private bool IsValidObjectId(string? id)
        {
            if (string.IsNullOrWhiteSpace(id)) return false;
            if (id.Length != 24) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(id, @"^[0-9a-fA-F]{24}$");
        }

        private decimal? ParseSalary(string? salaryStr)
        {
            if (string.IsNullOrWhiteSpace(salaryStr)) return null;
            var cleaned = new string(salaryStr.Where(c => char.IsDigit(c) || c == '.' || c == ',').ToArray());
            cleaned = cleaned.Replace(",", ".");
            if (cleaned.Count(c => c == '.') > 1) cleaned = new string(cleaned.Where(char.IsDigit).ToArray());
            return decimal.TryParse(cleaned, out var result) ? result : null;
        }
    }
}
