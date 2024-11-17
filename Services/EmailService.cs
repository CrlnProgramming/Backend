using Backend.Models;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace Backend.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task SendEmailAsync(EmailDto emailDto)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse("info@softbox-solution.ru"));
                email.To.Add(MailboxAddress.Parse(emailDto.To));
                email.Subject = emailDto.Subject;
                email.Body = new TextPart(TextFormat.Text) { Text = emailDto.Body };
                
                _logger.LogInformation("Prepare to send email to {TO} with subject {Subject}",emailDto.To, emailDto.Subject);

                using (var smtp = new SmtpClient())
                {
                    _logger.LogInformation("Connecting to SMTP server {EmailHost}", _configuration["EmailHost"]);
                    await smtp.ConnectAsync(_configuration["EmailHost"], 465, true);
                    
                    _logger.LogInformation("Authenticating with SMTP server");
                    await smtp.AuthenticateAsync(_configuration["EmailUsername"], _configuration["EmailPassword"]);
                    
                    _logger.LogInformation("Sending email");
                    await smtp.SendAsync(email);
                    
                    _logger.LogInformation("Disconnecting from SMTP server");
                    await smtp.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {TO}", emailDto.To);
            }
        }
    }
}
