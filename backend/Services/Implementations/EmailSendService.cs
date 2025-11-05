using CloudCore.Services.Interfaces;
using FluentEmail.Core;

namespace CloudCore.Services.Implementations
{
    public class EmailSendService : IEmailSendService
    {
        private readonly IFluentEmail _fluentEmail;
        private readonly ILogger<EmailSendService> _logger;

        public EmailSendService(IFluentEmail fluentEmail, ILogger<EmailSendService> logger)
        {
            _fluentEmail = fluentEmail;
            _logger = logger;
        }

        public async Task SendEmailVerificationAsync(string toEmail, string verifyUrl, string subject, CancellationToken cancellationToken)
        {
            try
            {
                var contentRoot = AppContext.BaseDirectory;
                var templatePath = Path.Combine(contentRoot, "EmailTemplates", "VerifyEmail.cshtml");
                string template = await File.ReadAllTextAsync(templatePath, cancellationToken);
                string htmlBody = template.Replace("{{VerifyUrl}}", verifyUrl);

                await _fluentEmail
                    .To(toEmail)
                    .Subject(subject)
                    .Body(htmlBody, isHtml: true)
                    .SendAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {toEmail}");
                throw;
            }
        }

        public async Task SendPasswordResetAsync(string toEmail, string resetUrl, string subject, CancellationToken cancellationToken)
        {
            try
            {
                var contentRoot = AppContext.BaseDirectory;
                var templatePath = Path.Combine(contentRoot, "EmailTemplates", "ResetPassword.cshtml");
                string template = await File.ReadAllTextAsync(templatePath, cancellationToken);
                _logger.LogInformation($"Template length: {template.Length}");
                _logger.LogInformation($"Contains placeholder: {template.Contains("{{ResetUrl}}")}");

                string htmlBody = template.Replace("{{ResetUrl}}", resetUrl);

                await _fluentEmail
                    .To(toEmail)
                    .Subject(subject)
                    .Body(htmlBody, isHtml: true)
                    .SendAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {toEmail}");
                throw;
            }
        }
    }
}
