using System.ComponentModel.DataAnnotations;

namespace CloudCore.Contracts.Requests
{
    public record ChangeUsernameRequest
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Username must be 5-50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$",
            ErrorMessage = "Username can only contain letters, numbers and underscore")]
        public string NewUsername { get; init; } = null!;
    }
}
