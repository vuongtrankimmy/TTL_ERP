using System;
using System.Globalization;
using TTL.HR.Application.Modules.Common.Interfaces;

namespace TTL.HR.Application.Modules.Common.Services
{
    public class FormatService : IFormatService
    {
        private readonly ISettingsService _settingsService;
        private readonly CultureInfo _vnCulture = new CultureInfo("vi-VN");

        public FormatService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public string FormatCurrency(decimal amount)
        {
            var currency = _settingsService.CachedSettings?.Currency ?? "VND";
            if (currency == "VND")
            {
                return amount.ToString("N0", _vnCulture) + " ₫";
            }
            return amount.ToString("C", CultureInfo.CurrentCulture);
        }

        public string FormatDate(DateTime? date)
        {
            if (!date.HasValue) return string.Empty;
            var format = _settingsService.CachedSettings?.DateFormat ?? "dd/MM/yyyy";
            return date.Value.ToString(format);
        }

        public string FormatDateTime(DateTime? date)
        {
            if (!date.HasValue) return string.Empty;
            var format = _settingsService.CachedSettings?.DateFormat ?? "dd/MM/yyyy";
            return date.Value.ToString($"{format} HH:mm");
        }

        public string FormatNumber(decimal value, int decimals = 0)
        {
            return value.ToString($"N{decimals}", _vnCulture);
        }
        
        public string FormatNumber(double value, int decimals = 0)
        {
            return value.ToString($"N{decimals}", _vnCulture);
        }

        public string FormatNumber(int value, int decimals = 0)
        {
            return value.ToString($"N{decimals}", _vnCulture);
        }



        public string FormatPercent(double value)
        {
            return (value / 100).ToString("P1", _vnCulture);
        }

        public DateTime? ToLocalTime(DateTime? utcDate)
        {
            if (!utcDate.HasValue) return null;
            
            try
            {
                var timeZoneId = _settingsService.CachedSettings?.TimeZone ?? "SE Asia Standard Time";
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                return TimeZoneInfo.ConvertTimeFromUtc(utcDate.Value, timeZone);
            }
            catch
            {
                return utcDate;
            }
        }
    }
}
