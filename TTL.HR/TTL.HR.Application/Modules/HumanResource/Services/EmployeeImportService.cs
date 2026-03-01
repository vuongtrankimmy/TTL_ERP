using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.HumanResource.Entities;
using TTL.HR.Application.Modules.HumanResource.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Models;

namespace TTL.HR.Application.Modules.HumanResource.Services
{
    public class EmployeeImportService : IEmployeeImportService
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeImportService(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<byte[]> ExportEmployeeTemplateAsync()
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Template");

            // Define Headers
            string[] headers = {
                "Mã nhân viên (*)", "Họ và tên (*)", "Email cá nhân", "Số điện thoại", 
                "Ngày sinh (dd/mm/yyyy)", "Giới tính", "Số CMND/CCCD", "Ngày cấp", "Nơi cấp",
                "Mã số thuế", "Số tài khoản", "Ngân hàng", "Ngày vào làm (dd/mm/yyyy)",
                "Bộ phận", "Chức vụ", "Địa chỉ", "Quê quán"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightGray;
            }

            // Fill sample data
            worksheet.Cell(2, 1).Value = "NV001";
            worksheet.Cell(2, 2).Value = "Nguyễn Văn A";
            worksheet.Cell(2, 3).Value = "vana@gmail.com";
            worksheet.Cell(2, 4).Value = "0901234567";
            worksheet.Cell(2, 5).Value = "01/01/1990";
            worksheet.Cell(2, 6).Value = "Nam";
            
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return await Task.FromResult(stream.ToArray());
        }

        public async Task<ImportResultModel> ImportEmployeesFromExcelAsync(byte[] fileContent)
        {
            var result = new ImportResultModel();
            using var stream = new MemoryStream(fileContent);
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.FirstOrDefault();

            if (worksheet == null)
            {
                result.Errors.Add("Không tìm thấy worksheet trong file Excel.");
                return result;
            }

            var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header
            foreach (var row in rows)
            {
                try
                {
                    var code = row.Cell(1).GetValue<string>();
                    var fullName = row.Cell(2).GetValue<string>();

                    if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(fullName))
                    {
                        result.FailureCount++;
                        result.Errors.Add($"Dòng {row.RowNumber()}: Thiếu cột bắt buộc (Mã hoặc Họ tên).");
                        continue;
                    }

                    var employee = new Employee
                    {
                        Code = code,
                        FullName = fullName,
                        Email = row.Cell(3).GetValue<string>(),
                        Phone = row.Cell(4).GetValue<string>(),
                        JoinDate = TryParseDate(row.Cell(13).GetValue<string>()) ?? DateTime.Today,
                        PersonalDetails = new PersonalInfo
                        {
                            DOB = TryParseDate(row.Cell(5).GetValue<string>()),
                            Gender = row.Cell(6).GetValue<string>(),
                            IdCardNumber = row.Cell(7).GetValue<string>(),
                            TaxCode = row.Cell(10).GetValue<string>(),
                            BankAccount = row.Cell(11).GetValue<string>(),
                            BankName = row.Cell(12).GetValue<string>(),
                            Address = row.Cell(16).GetValue<string>(),
                            Hometown = row.Cell(17).GetValue<string>()
                        },
                        IsAccountActive = true,
                        IsCreateAccount = true // Default auto-create account
                    };

                    var createResult = await _employeeService.CreateEmployeeAsync(employee);
                    if (createResult == null) // Success in IEmployeeService convention (null error = success)
                    {
                        result.SuccessCount++;
                    }
                    else
                    {
                        result.FailureCount++;
                        result.Errors.Add($"Dòng {row.RowNumber()}: {createResult}");
                    }
                }
                catch (Exception ex)
                {
                    result.FailureCount++;
                    result.Errors.Add($"Dòng {row.RowNumber()}: Lỗi không xác định - {ex.Message}");
                }
            }

            return result;
        }

        public async Task<byte[]> ExportEmployeeListAsync(IEnumerable<EmployeeModel> employees)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Employees");

            string[] headers = { "Mã NV", "Họ tên", "Email", "SĐT", "Ngày vào làm", "Bộ phận", "Chức vụ", "Trạng thái" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
            }

            int rowIdx = 2;
            foreach (var emp in employees)
            {
                worksheet.Cell(rowIdx, 1).Value = emp.Code;
                worksheet.Cell(rowIdx, 2).Value = emp.FullName;
                worksheet.Cell(rowIdx, 3).Value = emp.Email;
                worksheet.Cell(rowIdx, 4).Value = emp.Phone;
                worksheet.Cell(rowIdx, 5).Value = emp.JoinDate?.ToString("dd/MM/yyyy") ?? "";
                worksheet.Cell(rowIdx, 6).Value = emp.DepartmentName;
                worksheet.Cell(rowIdx, 7).Value = emp.PositionName;
                worksheet.Cell(rowIdx, 8).Value = emp.StatusName;
                rowIdx++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return await Task.FromResult(stream.ToArray());
        }

        private DateTime? TryParseDate(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            if (DateTime.TryParse(input, out var date)) return date;
            
            // Try specific formats
            string[] formats = { "dd/MM/yyyy", "MM/dd/yyyy", "yyyy-MM-dd" };
            if (DateTime.TryParseExact(input, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
            {
                return date;
            }
            
            return null;
        }
    }
}
