using System.ComponentModel.DataAnnotations;

namespace CloudCore.Contracts.Requests
{
    public record ResetPasswordRequest
    {
        [Required(ErrorMessage = "Reset token is required")]
        public string Token { get; init; } = string.Empty;

        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*\d).{8,}$",
            ErrorMessage = "Password must contain at least one lowercase letter and one number")]
        public string NewPassword { get; init; } = string.Empty;
    }
}
