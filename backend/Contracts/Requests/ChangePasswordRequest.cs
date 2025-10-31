using System.ComponentModel.DataAnnotations;

namespace CloudCore.Contracts.Requests
{
    public record ChangePasswordRequest
    {
        [Required(ErrorMessage = "Current password is required")]
        [MinLength(1, ErrorMessage = "Current password cannot be empty")]
        public string CurrentPassword { get; init; } = null!;

        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*\d).{8,}$",
            ErrorMessage = "Password must contain at least one lowercase letter and one number")]
        public string NewPassword { get; init; } = null!;

        [Required(ErrorMessage = "Password confirmation is required")]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
        public string ConfirmNewPassword { get; init; } = null!;
    }
}
