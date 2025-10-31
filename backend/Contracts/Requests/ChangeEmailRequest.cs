using System.ComponentModel.DataAnnotations;

namespace CloudCore.Contracts.Requests
{
    public record ChangeEmailRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(255, MinimumLength = 5, ErrorMessage = "Email must be between 5 and 255 characters")]
        public string NewEmail { get; init; } = null!;
    }
}
