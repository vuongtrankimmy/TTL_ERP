using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Payroll.Models;
using TTL.HR.Application.Modules.HumanResource.Models;
using TTL.HR.Application.Modules.Common.Interfaces;

namespace TTL.HR.Application.Modules.Common.Services
{
    public class PdfService : IPdfService
    {
        static PdfService()
        {
            // Set QuestPDF license to Community
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> GenerateBatchPayslipsPdfAsync(IEnumerable<PayrollModel> payrolls)
        {
            var document = Document.Create(container =>
            {
                foreach (var payroll in payrolls)
                {
                    container.Page(page =>
                    {
                        ComposePayslipPage(page, payroll);
                    });
                }
            });

            return await Task.FromResult(document.GeneratePdf());
        }

        public async Task<byte[]> GeneratePayslipPdfAsync(PayrollModel payroll)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    ComposePayslipPage(page, payroll);
                });
            });

            return await Task.FromResult(document.GeneratePdf());
        }

        private void ComposePayslipPage(PageDescriptor page, PayrollModel payroll)
        {
            page.Size(PageSizes.A5.Landscape());
            page.Margin(1, Unit.Centimetre);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Verdana));

            page.Header().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("PHIẾU LƯƠNG NHÂN VIÊN").FontSize(16).SemiBold().FontColor(Colors.Blue.Medium);
                    col.Item().Text($"Tháng {payroll.Month}/{payroll.Year}").FontSize(12).Italic();
                });

                row.RelativeItem().AlignRight().Column(col =>
                {
                    col.Item().Text("CÔNG TY TÂN TÂN LỘC").FontSize(12).SemiBold();
                    col.Item().Text("TTL ERP System").FontSize(8).FontColor(Colors.Grey.Medium);
                });
            });

            page.Content().PaddingVertical(10).Column(col =>
            {
                // Employee Info
                col.Item().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).Row(row =>
                {
                    row.RelativeItem().Text(t =>
                    {
                        t.Span("Nhân viên: ").SemiBold();
                        t.Span(payroll.EmployeeName);
                    });
                    row.RelativeItem().Text(t =>
                    {
                        t.Span("Mã NV: ").SemiBold();
                        t.Span(payroll.EmployeeCode);
                    });
                    row.RelativeItem().Text(t =>
                    {
                        t.Span("Bộ phận: ").SemiBold();
                        t.Span(payroll.DepartmentName);
                    });
                });

                col.Item().PaddingTop(10).Row(row =>
                {
                    // Left Column: Income
                    row.RelativeItem().PaddingRight(10).Column(c =>
                    {
                        c.Item().Text("KHOẢN THU NHẬP").SemiBold().Underline();
                        c.Item().Row(r => { r.RelativeItem().Text("Lương cơ bản"); r.RelativeItem().AlignRight().Text(payroll.BasicSalary.ToString("N0") + " đ"); });
                        c.Item().Row(r => { r.RelativeItem().Text("Lương ngày công"); r.RelativeItem().AlignRight().Text(payroll.TotalWorkSalary.ToString("N0") + " đ"); });
                        c.Item().Row(r => { r.RelativeItem().Text("Lương tăng ca"); r.RelativeItem().AlignRight().Text(payroll.OvertimeSalary.ToString("N0") + " đ"); });
                        c.Item().Row(r => { r.RelativeItem().Text("Phụ cấp"); r.RelativeItem().AlignRight().Text(payroll.Allowance.ToString("N0") + " đ"); });
                        c.Item().Row(r => { r.RelativeItem().Text("Thưởng"); r.RelativeItem().AlignRight().Text(payroll.Bonus.ToString("N0") + " đ"); });

                        c.Item().PaddingTop(5).BorderTop(1).Row(r =>
                        {
                            r.RelativeItem().Text("TỔNG THU NHẬP (Gross)").SemiBold();
                            r.RelativeItem().AlignRight().Text(payroll.GrossSalary.ToString("N0") + " đ").SemiBold();
                        });
                    });

                    // Right Column: Deductions
                    row.RelativeItem().PaddingLeft(10).Column(c =>
                    {
                        c.Item().Text("KHOẢN KHẤU TRỪ").SemiBold().Underline();
                        c.Item().Row(r => { r.RelativeItem().Text("BHXH (8%)"); r.RelativeItem().AlignRight().Text(payroll.BhxhAmount.ToString("N0") + " đ"); });
                        c.Item().Row(r => { r.RelativeItem().Text("BHYT (1.5%)"); r.RelativeItem().AlignRight().Text(payroll.BhytAmount.ToString("N0") + " đ"); });
                        c.Item().Row(r => { r.RelativeItem().Text("BHTN (1%)"); r.RelativeItem().AlignRight().Text(payroll.BhtnAmount.ToString("N0") + " đ"); });
                        c.Item().Row(r => { r.RelativeItem().Text("Thuế TNCN"); r.RelativeItem().AlignRight().Text(payroll.TaxAmount.ToString("N0") + " đ"); });
                        
                        // Split Payments / Advances
                        if (payroll.PaymentDetails != null && payroll.PaymentDetails.Any())
                        {
                            foreach(var p in payroll.PaymentDetails)
                            {
                                var desc = string.IsNullOrEmpty(p.Description) ? "Tạm ứng/Thanh toán" : p.Description;
                                var dateStr = p.PaymentDate?.ToString("dd/MM") ?? "";
                                c.Item().Row(r => { 
                                    r.RelativeItem().Text($"{desc} ({dateStr})").FontColor(Colors.Red.Medium); 
                                    r.RelativeItem().AlignRight().Text(p.Amount.ToString("N0") + " đ").FontColor(Colors.Red.Medium); 
                                });
                            }
                        }
                        else if (payroll.AdvanceAmount > 0)
                        {
                            c.Item().Row(r => { r.RelativeItem().Text("Tạm ứng"); r.RelativeItem().AlignRight().Text(payroll.AdvanceAmount.ToString("N0") + " đ"); });
                        }

                        c.Item().Row(r => { r.RelativeItem().Text("Khác"); r.RelativeItem().AlignRight().Text(payroll.Deduction.ToString("N0") + " đ"); });

                        c.Item().PaddingTop(5).BorderTop(1).Row(r =>
                        {
                            r.RelativeItem().Text("TỔNG KHẤU TRỪ").SemiBold();
                            r.RelativeItem().AlignRight().Text(payroll.TotalDeduction.ToString("N0") + " đ").SemiBold();
                        });
                    });
                });

                // Summary
                col.Item().AlignRight().PaddingTop(20).Container().Background(Colors.Grey.Lighten4).Padding(10).Row(row =>
                {
                    row.RelativeItem().AlignRight().Text("THỰC LĨNH (NET):").FontSize(14).SemiBold();
                    row.RelativeItem(0.5f).AlignRight().Text(payroll.NetSalary.ToString("N0") + " VNĐ").FontSize(14).SemiBold().FontColor(Colors.Green.Medium);
                });
            });

            page.Footer().AlignCenter().Text(x =>
            {
                x.Span("Phiếu lương này được tạo tự động bởi hệ thống TTL ERP vào lúc ");
                x.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).Italic();
            });
        }

        public async Task<byte[]> GenerateEmployeeProfilePdfAsync(EmployeeModel employee)
        {
            // Basic implementation for now, can be expanded
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Text("HỒ SƠ CÁ NHÂN NHÂN VIÊN").FontSize(20).SemiBold().AlignCenter();

                    page.Content().Column(col =>
                    {
                        col.Item().PaddingTop(20).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text($"Họ và tên: {employee.FullName}").FontSize(14).SemiBold();
                                c.Item().Text($"Mã nhân viên: {employee.Code}");
                                c.Item().Text($"Chức vụ: {employee.PositionName}");
                                c.Item().Text($"Phòng ban: {employee.DepartmentName}");
                            });
                        });

                        col.Item().PaddingTop(20).Text("THÔNG TIN CHI TIẾT").SemiBold().Underline();
                        col.Item().Text($"Ngày sinh: {employee.DOB?.ToString("dd/MM/yyyy") ?? "N/A"}");
                        col.Item().Text($"Giới tính: {employee.Gender}");
                        col.Item().Text($"Số điện thoại: {employee.Phone}");
                        col.Item().Text($"Email: {employee.Email}");
                        col.Item().Text($"Địa chỉ: {employee.Address}");
                    });
                });
            });

            return await Task.FromResult(document.GeneratePdf());
        }
    }
}
