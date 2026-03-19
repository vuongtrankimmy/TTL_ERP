using System;

namespace TTL.HR.Application.Modules.Common.Models
{
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public UserDto User { get; set; } = new();
    }

    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string IdCardNumber { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string Hometown { get; set; } = string.Empty;
        public int? CountryId { get; set; }
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
        public int? WardId { get; set; }
        public int? StreetId { get; set; }
        public string? Street { get; set; }
        public string BankName { get; set; } = string.Empty;
        public string BankAccount { get; set; } = string.Empty;
        public int? GenderId { get; set; }
        public string Gender { get; set; } = string.Empty;
        public int? MaritalStatusId { get; set; }
        public string MaritalStatus { get; set; } = string.Empty;
    }
}
