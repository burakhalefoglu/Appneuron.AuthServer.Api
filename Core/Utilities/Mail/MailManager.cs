using System;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Core.Utilities.Mail
{
    public class MailManager : IMailService
    {
        private readonly IConfiguration _configuration;

        public MailManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Send(EmailMessage emailMessage)
        {
            var message = new MimeMessage();
            message.To.AddRange(emailMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
            message.From.AddRange(emailMessage.FromAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
            message.Subject = emailMessage.Subject;

            var builder = new BodyBuilder
            {
                HtmlBody = emailMessage.Content.ToString()
            };
            message.Body = builder.ToMessageBody();

            using var emailClient = new SmtpClient();
            await emailClient.ConnectAsync(
                _configuration.GetSection("EmailConfiguration").GetSection("SmtpServer").Value,
                Convert.ToInt32(_configuration.GetSection("EmailConfiguration").GetSection("SmtpPort").Value),
                true);
            emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
            await emailClient.AuthenticateAsync(
                _configuration.GetSection("EmailConfiguration").GetSection("SmtpUsername").Value,
                _configuration.GetSection("EmailConfiguration").GetSection("SmtpPassword").Value);
            await emailClient.SendAsync(message);
            await emailClient.DisconnectAsync(true);
        }
    }
}