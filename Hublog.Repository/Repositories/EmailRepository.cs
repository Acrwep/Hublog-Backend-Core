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
            email.Sender = MailboxAddress.Parse(emailSettings.Email);
            email.To.Add(MailboxAddress.Parse(users.Email));
            email.Subject = users.Subject;
            var builder = new BodyBuilder();
            // Get the absolute path of the image
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo-re-3.png");

            // Attach image and get CID
            var image = builder.LinkedResources.Add(imagePath);
            image.ContentId = "logoImage";

            builder.HtmlBody = getHtmlContent(users);
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(emailSettings.Host, emailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(emailSettings.Email, emailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        private string getHtmlContent(Users users)
        {
            string response = $@"
<table role=""presentation"" width=""100%"" height=""100%"" style=""background-color: #fafafa; min-height: 100vh; border-spacing: 0;"" cellpadding=""0"" cellspacing=""0"">
  <tr>
    <td align=""center"" valign=""middle"">
      <table role=""presentation"" width=""70%"" style=""border: 1px solid #D2D1D6; padding: 19px 12px; color:#222; background-color: #ffffff;"" cellpadding=""0"" cellspacing=""0"">
        <tr>
          <td align=""center"">
            <img src=""cid:logoImage"" style=""width: 120px;margin-top:6px"" />
          </td>
        </tr>
        <tr>
          <td>
            <div style=""border-bottom: 1px solid rgb(210, 209, 214);margin-top:12px"" />
            <p style=""font-weight: 600; margin-top: 20px;color:#222;"">Dear {users.First_Name + " " + users.Last_Name},</p>
            <p style=""margin-top: 6px; line-height: 24px;color:#222;"">
              We are pleased to inform you that your employee account has been
              successfully created at Hublog. You can now access your account and
              begin using our platform.
            </p>
            <p style=""margin-top: 12px;color:#222;"">Here are your login details:</p>
            <ul style=""margin-top: 6px; padding-left: 20px;"">
              <li style=""margin-bottom: 6px;"">
                <span style=""font-weight: 600;color:#222;"">Email:</span>   <span style=""color:#222;"">{users.Email}</span>
              </li>
              <li style=""margin-bottom: 6px;"">
                <span style=""font-weight: 600;color:#222;"">Login URL:</span>
                <a href=""https://hublog.org/login"" target=""_blank"" style=""text-decoration: underline; color: #15c;"">
                  https://hublog.org/login
                </a>
              </li>
              <li style=""margin-bottom: 6px;"">
                <span style=""font-weight: 600;color:#222;"">Download App URL:</span>
                <a href=""https://hublog.org/downloads"" target=""_blank"" style=""text-decoration: underline; color: #15c;"">
                  https://hublog.org/downloads
                </a>
              </li>
            </ul>

            <p style=""margin-top: 16px; font-size: 14px; margin-bottom: 16px;color:#222;"">
              If you have any questions or need assistance, feel free to reach out
              to our support team at <a href=""mailto:hublog@gmail.com"" style=""text-decoration: underline; color: #15c;"">hublog@gmail.com</a>.
            </p>

            <div style=""font-size: 14px;color:#222;"">
              <p style=""margin-bottom: 0px;"">Best Regards,</p>
              <p style=""margin-top: 2px;"">Hublog Team</p>
            </div>
          </td>
        </tr>
      </table>
    </td>
  </tr>
</table>";
            return response;
        }

    }

}
