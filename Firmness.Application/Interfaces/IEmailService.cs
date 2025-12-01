namespace Firmness.Application.Interfaces;

public interface IEmailService
{
    /// <summary>
    /// Sends an email with optional attachment
    /// </summary>
    /// <param name="to">Recipient email address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="body">Email body (HTML supported)</param>
    /// <param name="attachmentPath">Optional path to file attachment</param>
    /// <param name="attachmentName">Optional custom name for the attachment</param>
    Task SendEmailAsync(string to, string subject, string body, string? attachmentPath = null, string? attachmentName = null);
    
    /// <summary>
    /// Sends an email to multiple recipients
    /// </summary>
    Task SendEmailToMultipleAsync(List<string> recipients, string subject, string body, string? attachmentPath = null, string? attachmentName = null);
}
