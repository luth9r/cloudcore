using System.ComponentModel.DataAnnotations;

namespace CloudCore.Contracts.Requests
{
    public record TokenRequest
    {
        [Required(ErrorMessage = "Token is required")]
        [MinLength(1, ErrorMessage = "Token cannot be empty")]
        public string Token { get; init; } = null!;
    }
}
