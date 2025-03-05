using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using BoletinServiceWorker.Data;
using BoletinServiceWorker.Entities;
using BoletinServiceWorker.Helpers;
using BoletinServiceWorker.Models;
using System.Net.Mail;
using System.Net;
using System.Net.Http.Headers;

namespace BoletinServiceWorker.Services;

public class EmailService
{
    private ILogger<EmailService> logger;
    private readonly SmtpClient smtpClient;
    private readonly EmailSettings settings;

    public EmailService(IOptions<EmailSettings> options, ILogger<EmailService> logger)
    {
        this.settings = options.Value;
        this.logger = logger;

        this.smtpClient = new SmtpClient(settings.Host, settings.Port)
        {
            Credentials = new NetworkCredential(settings.UserName, settings.Password),
            EnableSsl = true
        };
    }

    public async Task<MessageResult> SendEmail(string to, string subject, string body, bool isBodyHtml = false, List<string>? attachments = null)
    {
        try
        {
            using (var mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(settings.From);
                mailMessage.To.Add(to);
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = isBodyHtml;

                // Attach files if provided
                if (attachments != null && attachments.Any())
                {
                    foreach (var filePath in attachments)
                    {
                        if (File.Exists(filePath)) // Check if file exists before attaching
                        {
                            mailMessage.Attachments.Add(new Attachment(filePath));
                        }
                        else
                        {
                            this.logger.LogWarning("Warning: Attachment not found: {filePath}", filePath);
                        }
                    }
                }

                await smtpClient.SendMailAsync(mailMessage);
                return MessageResult.Success("Email sent successfully.");
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error sending email: {Message}", ex.Message);
            return MessageResult.Failure(ex.Message);
        }
    }

}
