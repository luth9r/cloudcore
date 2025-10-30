namespace CloudCore.Services.Interfaces
{
    public interface IEmailSendService
    {
        Task SendEmailVerificationAsync(string toEmail, string verifyUrl, string subject);

        Task SendPasswordResetAsync(string email, string resetUrl, string subject);
    }
}
