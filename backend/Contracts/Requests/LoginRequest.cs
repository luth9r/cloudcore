using System.ComponentModel.DataAnnotations;

namespace CloudCore.Contracts.Requests
{
    public record LoginRequest
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Username must be 5-50 characters")]
        public string Username { get; init; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string Password { get; init; } = null!;
    }
}