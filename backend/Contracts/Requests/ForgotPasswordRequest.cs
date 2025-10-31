using System.ComponentModel.DataAnnotations;

namespace CloudCore.Contracts.Requests
{
    public record ForgotPasswordRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(255, ErrorMessage = "Email must not exceed 255 characters")]
        public string Email { get; init; } = string.Empty;
    }
}

