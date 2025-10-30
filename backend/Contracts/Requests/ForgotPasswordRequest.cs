using System.ComponentModel.DataAnnotations;

namespace CloudCore.Contracts.Requests
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}

