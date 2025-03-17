using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.UserModels;
using Hublog.Repository.Interface;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Hublog.Repository.Repositories
{
    public class EmailRepository : IEmailRepository
    {
        private readonly EmailSettings emailSettings;
        public EmailRepository(IOptions<EmailSettings> options)
        {
            this.emailSettings = options.Value;
        }
        public async Task SendEmailAsync(Users users)
        {
            var email = new MimeMessage();
            email.Sender=MailboxAddress.Parse(emailSettings.Email);
            email.To.Add(MailboxAddress.Parse(users.Email));
            email.Subject = users.Subject;
            var builder=new BodyBuilder();
            builder.HtmlBody = getHtmlContent();
            email.Body=builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(emailSettings.Host, emailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(emailSettings.Email, emailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        private string getHtmlContent()
        {
            string response = "<div><h1>Hiiiii</h1></div>";
            return response;
        }
    }

}
