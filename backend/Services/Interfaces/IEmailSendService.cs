namespace CloudCore.Services.Interfaces
{
    public interface IEmailSendService
    {
        /// <summary>
        /// Sends an email verification link to the user's email address
        /// </summary>
        /// <param name="toEmail">Recipient email address</param>
        /// <param name="verifyUrl">Email verification URL containing the token</param>
        /// <param name="subject">Email subject line</param>
        /// <param name="cancellationToken">Token to cancel the operation if client disconnects</param>
        Task SendEmailVerificationAsync(string toEmail, string verifyUrl, string subject, CancellationToken cancellationToken);

        /// <summary>
        /// Sends a password reset link to the user's email address
        /// </summary>
        /// <param name="email">Recipient email address</param>
        /// <param name="resetUrl">Password reset URL containing the token</param>
        /// <param name="subject">Email subject line</param>
        /// <param name="cancellationToken">Token to cancel the operation if client disconnects</param>
        Task SendPasswordResetAsync(string email, string resetUrl, string subject, CancellationToken cancellationToken);
    }
}
