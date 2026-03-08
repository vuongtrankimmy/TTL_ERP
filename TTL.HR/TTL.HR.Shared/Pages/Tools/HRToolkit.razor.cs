using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace TTL.HR.Shared.Pages.Tools;

public partial class HRToolkit
{
    // Gross to Net Logic
    private decimal grossSalary = 15000000;
    private int dependants = 0;
    private int insuranceRegion = 1;

    private decimal insuranceAmount => Math.Min(grossSalary, 20 * 1800000) * 0.105m; // Fake logic for demo
    private decimal pitAmount
    {
        get
        {
            decimal taxable = grossSalary - insuranceAmount - 11000000 - (dependants * 4400000);
            if (taxable <= 0) return 0;
            if (taxable <= 5000000) return taxable * 0.05m;
            return 250000 + (taxable - 5000000) * 0.1m;
        }
    }
    private decimal netSalary => grossSalary - insuranceAmount - pitAmount;

    // Working Days Logic
    private DateTime startDate = DateTime.Today.AddDays(-30);
    private DateTime endDate = DateTime.Today;
    private bool includeSaturdays = false;

    private int totalDays => (endDate - startDate).Days + 1;
    private int workDays
    {
        get
        {
            int count = 0;
            for (var d = startDate; d <= endDate; d = d.AddDays(1))
            {
                if (d.DayOfWeek != DayOfWeek.Sunday && (includeSaturdays || d.DayOfWeek != DayOfWeek.Saturday)) count++;
            }
            return count;
        }
    }
    private int weekendDays => totalDays - workDays;

    // CCCD Logic
    private string cccdNumber = "";
    private CccdResult? cccdInfo;

    private void ParseCCCD()
    {
        if (cccdNumber.Length != 12) return;

        var provinceCode = cccdNumber.Substring(0, 3);
        var genderCentury = cccdNumber[3];
        var year = cccdNumber.Substring(4, 2);

        cccdInfo = new CccdResult
        {
            ProvinceName = GetProvinceName(provinceCode),
            GenderCentury = GetGender(genderCentury),
            BirthYear = GetFullYear(genderCentury, year),
            UniqueId = cccdNumber.Substring(6)
        };
    }

    private string GetProvinceName(string code) => code switch
    {
        "001" => "TP. Hà Nội",
        "034" => "Tỉnh Thái Bình",
        "079" => "TP. Hồ Chí Minh",
        _ => "Tỉnh/Thành khác"
    };

    private string GetGender(char code) => code switch
    {
        '0' => "Nam (Thế kỷ 20)",
        '1' => "Nữ (Thế kỷ 20)",
        '2' => "Nam (Thế kỷ 21)",
        '3' => "Nữ (Thế kỷ 21)",
        _ => "Khác"
    };

    private string GetFullYear(char code, string yy)
    {
        int century = int.Parse(code.ToString());
        string prefix = (century / 2 == 0) ? "19" : "20";
        return prefix + yy;
    }

    private string FormatCurrency(decimal amount) => amount.ToString("N0", new CultureInfo("vi-VN"));

    public class CccdResult
    {
        public string ProvinceName { get; set; } = "";
        public string GenderCentury { get; set; } = "";
        public string BirthYear { get; set; } = "";
        public string UniqueId { get; set; } = "";
    }
}
