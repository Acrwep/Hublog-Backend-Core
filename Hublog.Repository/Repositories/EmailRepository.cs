using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.Organization;
using Hublog.Repository.Entities.Model.OTPRequest;
using Hublog.Repository.Entities.Model.UserModels;
using Hublog.Repository.Interface;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using MimeKit;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Utilities.Collections;
using static Hublog.Repository.Common.CommonConstant;

namespace Hublog.Repository.Repositories
{
    public class EmailRepository : IEmailRepository
    {
        private readonly EmailSettings emailSettings;
        private readonly IOtpRepository _otpService;
        private readonly Dapperr _dapper;
        public EmailRepository(IOptions<EmailSettings> options, IOtpRepository otpService, Dapperr dapper)
        {
            this.emailSettings = options.Value;
            _otpService = otpService;
            _dapper = dapper;
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

            builder.HtmlBody = getUserHtmlContent(users);
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(emailSettings.Host, emailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(emailSettings.Email, emailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }


        public async Task SendOrganizationEmailAsync(Organizations organizations)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(emailSettings.Email);
            email.To.Add(MailboxAddress.Parse(organizations.Email));
            email.Subject = organizations.Subject;
            var builder = new BodyBuilder();
            // Get the absolute path of the image
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo-re-3.png");

            // Attach image and get CID
            var image = builder.LinkedResources.Add(imagePath);
            image.ContentId = "logoImage";

            builder.HtmlBody = getHtmlContentOrganization(organizations);
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(emailSettings.Host, emailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(emailSettings.Email, emailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }



        private string getUserHtmlContent(Users users)
        {
            string response = $@"
<table role=""presentation"" width=""100%"" height=""100%"" style=""background-color: #fafafa; min-height: 100vh; border-spacing: 0;"" cellpadding=""0"" cellspacing=""0"">
  <tr>
    <td align=""center"" valign=""middle"">
      <table role=""presentation"" width=""70%"" style=""border: 1px solid #D2D1D6; padding: 19px 12px; color:#222; background-color: #ffffff;"" cellpadding=""0"" cellspacing=""0"">
        <tr>
          <td align=""center"">
            <img src=""cid:logoImage"" style=""width: 130px;margin-top:6px"" />
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
                <span style=""font-weight: 600;color:#222;"">Set password:</span>
                <a href=""https://workstatus.qubinex.com/setpassword"" target=""_blank"" style=""text-decoration: underline; color: #15c;"">
                  https://workstatus.qubinex.com/setpassword
                </a>
              </li>
              <li style=""margin-bottom: 6px;"">
                <span style=""font-weight: 600;color:#222;"">Login URL:</span>
                <a href=""https://workstatus.qubinex.com/login"" target=""_blank"" style=""text-decoration: underline; color: #15c;"">
                  https://workstatus.qubinex.com/login
                </a>
              </li>
              <li style=""margin-bottom: 6px;"">
                <span style=""font-weight: 600;color:#222;"">Download App URL:</span>
                <a href=""https://workstatus.qubinex.com/downloads"" target=""_blank"" style=""text-decoration: underline; color: #15c;"">
                  https://workstatus.qubinex.com/downloads
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




        private string getHtmlContentOrganization(Organizations organizations)
        {
            string response = $@"
<table role=""presentation"" width=""100%"" height=""100%"" style=""background-color: #fafafa; min-height: 100vh; border-spacing: 0;"" cellpadding=""0"" cellspacing=""0"">
  <tr>
    <td align=""center"" valign=""middle"">
      <table role=""presentation"" width=""70%"" style=""border: 1px solid #D2D1D6; padding: 19px 12px; color:#222; background-color: #ffffff;"" cellpadding=""0"" cellspacing=""0"">
        <tr>
          <td align=""center"">
            <img src=""cid:logoImage"" style=""width: 130px;margin-top:6px"" />
          </td>
        </tr>
        <tr>
          <td>
            <div style=""border-bottom: 1px solid rgb(210, 209, 214);margin-top:12px"" />
            <p style=""font-weight: 500; margin-top: 20px;color:#222;text-align:center;"">Hi {organizations.FirstName + " " + organizations.LastName}</p>
            <p style=""margin-top: 6px; line-height: 24px;color:#222;text-align:center;font-weight:600;"">
            You have been successfully registered at Hublog!
            </p>
            <p style=""margin-top: 12px;color:#222;"">To complete your registration, set your password using the link below:</p>
            <ul style=""margin-top: 6px; padding-left: 20px;"">
              <li style=""margin-bottom: 6px;"">
                <span style=""font-weight: 600;color:#222;"">Email:</span>   <span style=""color:#222;"">{organizations.Email}</span>
              </li>
              <li style=""margin-bottom: 6px;"">
                <span style=""font-weight: 600;color:#222;"">Password setup URL:</span>
                <a href=""https://workstatus.qubinex.com/setpassword"" target=""_blank"" style=""text-decoration: underline; color: #15c;"">
                  https://workstatus.qubinex.com/setpassword
                </a>
              </li>
              <li style=""margin-bottom: 6px;"">
                <span style=""font-weight: 600;color:#222;"">Login URL:</span>
                <a href=""https://workstatus.qubinex.com/login"" target=""_blank"" style=""text-decoration: underline; color: #15c;"">
                  https://workstatus.qubinex.com/login
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



        public async Task SendOtpEmailAsync(OtpRequest otpRequest, string otp)
        {
            var userDetails = await GetUserDetailsByEmailAsync(otpRequest.Email);

            if (string.IsNullOrEmpty(userDetails.FirstName) || string.IsNullOrEmpty(userDetails.LastName))
            {
                throw new Exception("User details not found for the given email.");
            }
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(emailSettings.Email);
            email.To.Add(MailboxAddress.Parse(otpRequest.Email));
            email.Subject = "Your OTP Code";

            var builder = new BodyBuilder { HtmlBody = GetOtpHtmlContent(otp, userDetails.FirstName, userDetails.LastName) };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(emailSettings.Host, emailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(emailSettings.Email, emailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
        private string GetOtpHtmlContent(string otp, string firstName, string lastName)
        {
            return $@"
            <div>
            <p>Dear {firstName} {lastName},</p>
            <p>Your One Time Password (OTP) is <span style=""font-weight: 600;margin-top:6px"">{otp}</span>. This OTP will be valid for next 5 mins.</p>

            <div style=""font-size: 14px;color:#222;"">
            <p style=""margin-bottom: 0px;"">Best Regards,</p>
            <p style=""margin-top: 2px;"">Hublog Team</p>
            </div>
            </div>";
        }

        public async Task<(string FirstName, string LastName)> GetUserDetailsByEmailAsync(string email)
        {
            string query = "SELECT First_Name, Last_Name FROM Users WHERE Email = @Email";

            return await _dapper.QueryFirstOrDefaultAsync<(string, string)>(query, new { Email = email });

        }
    }
}
