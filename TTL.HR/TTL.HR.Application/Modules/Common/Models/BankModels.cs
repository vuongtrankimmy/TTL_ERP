using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace TTL.HR.Application.Modules.Common.Models
{
    public class BankDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Logo { get; set; }
        public bool IsActive { get; set; }
        public int Priority { get; set; }
        public string? SwiftCode { get; set; }
        public string? Note { get; set; }
    }

    public class CreateBankRequest
    {
        [Required(ErrorMessage = "Tên ngân hàng là bắt buộc")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã ngân hàng là bắt buộc")]
        public string Code { get; set; } = string.Empty;

        public string? Logo { get; set; }
        public bool IsActive { get; set; } = true;
        public int Priority { get; set; }
        public string? SwiftCode { get; set; }
        public string? Note { get; set; }
    }

    public class UpdateBankRequest
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên ngân hàng là bắt buộc")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã ngân hàng là bắt buộc")]
        public string Code { get; set; } = string.Empty;

        public string? Logo { get; set; }
        public bool IsActive { get; set; }
        public int Priority { get; set; }
        public string? SwiftCode { get; set; }
        public string? Note { get; set; }
    }

    public class GetBanksRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
    }
}
