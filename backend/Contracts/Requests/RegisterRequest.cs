using System.ComponentModel.DataAnnotations;

namespace CloudCore.Contracts.Requests
{
    public record RegisterRequest
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Username must be 5-50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$",
            ErrorMessage = "Username can only contain letters, numbers and underscore")]
        public string Username { get; init; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; init; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*\d).{8,}$",
            ErrorMessage = "Password must contain at least one lowercase letter and one number")]
        public string Password { get; init; } = null!;
    }
}