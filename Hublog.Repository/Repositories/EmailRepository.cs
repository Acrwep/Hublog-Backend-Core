using System.Threading.Tasks;
using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.Organization;
using Hublog.Repository.Entities.Model.OTPRequest;
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
        private readonly IOtpRepository _otpService;
        private readonly Dapperr _dapper;
        private readonly TimeSpan _otpExpiry = TimeSpan.FromMinutes(5);
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
        <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0""
            style=""background-color: #f4f4f7; padding: 40px 0;"">
            <tr>
                <td align=""center"" style=""padding: 30px;"">
                    <table role=""presentation"" width=""600"" cellpadding=""0"" cellspacing=""0""
                        style=""background: #ffffff; border-radius: 5px; overflow: hidden; box-shadow: 0 4px 20px rgba(0, 0, 0, 0.05); font-family: Arial, sans-serif; color: #333;"">
                        <tr>
                            <td align=""center"" style=""padding: 20px 0;"">
                                <img style=""display: block; width: 200px;"" src=""cid:logoImage"" alt=""Hublog Logo""/>
                            </td>
                        </tr>
                        <tr><td style=""border-top: 1px solid #eaeaec;""></td></tr>
                        <tr>
                            <td style=""padding: 30px 40px 10px; font-size: 16px;"">
                                <p style=""margin: 0; font-weight: 600;"">Hi {users.First_Name + " " + users.Last_Name},</p>
                                <p style=""line-height: 1.6; margin-top: 12px;"">
                                    Welcome to <strong>Workstatus</strong>! Your employee account has been successfully created.
                                    You can now access our platform and start exploring its benefits.
                                </p>
                            </td>
                        </tr>
                        <tr>
                            <td style=""padding: 0 40px;"">
                                <p style=""font-weight: 600; margin-top: 0px; margin-bottom: 10px;"">Your Login Details:</p>
                                <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                    <tr><td style=""padding: 5px 0;""><strong>Email:</strong></td><td style=""padding: 5px 0;"">{users.Email}</td></tr>
                                    <tr>
                                        <td style=""padding: 5px 0;""><strong>Set Password:</strong></td>
                                        <td style=""padding: 5px 0;"">
                                            <a href=""https://workstatus.qubinex.com/setpassword"" target=""_blank""
                                               style=""color: #15c; text-decoration: underline;"">https://workstatus.qubinex.com/setpassword</a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style=""padding: 5px 0;""><strong>Login URL:</strong></td>
                                        <td style=""padding: 5px 0;"">
                                            <a href=""https://workstatus.qubinex.com/login"" target=""_blank""
                                               style=""color: #15c; text-decoration: underline;"">https://workstatus.qubinex.com/login</a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style=""padding: 5px 0;""><strong>Download App:</strong></td>
                                        <td style=""padding: 5px 0;"">
                                            <a href=""https://workstatus.qubinex.com/downloads"" target=""_blank""
                                               style=""color: #15c; text-decoration: underline;"">https://workstatus.qubinex.com/downloads</a>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td style=""padding: 10px 40px 0;"">
                                <p style=""font-size: 14px; line-height: 1.5;"">
                                    If you need help, please reach out to our support team at
                                    <a href=""mailto:workstatus@gmail.com""
                                       style=""color: #15c; text-decoration: underline;"">workstatus@gmail.com</a>.
                                </p>
                            </td>
                        </tr>
                        <tr>
                            <td style=""padding: 10px 40px 30px;"">
                                <p style=""margin: 0;"">Best Regards,</p>
                                <p style=""margin: 2px 0 0;"">The Workstatus Team</p>
                            </td>
                        </tr>
                        <tr><td style=""border-top: 1px solid #eaeaec;""></td></tr>
                        <tr>
                            <td align=""center"" style=""font-size: 12px; color: #999; padding: 20px;"">
                                &copy; 2025 Workstatus. All rights reserved.
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
            You have been successfully registered at Workstatus!
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
              <p style=""margin-top: 2px;"">Workstatus Team</p>
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
            var builder = new BodyBuilder();
            email.Sender = MailboxAddress.Parse(emailSettings.Email);
            email.To.Add(MailboxAddress.Parse(otpRequest.Email));
            email.Subject = "Your OTP Code";

            // Get the absolute path of the image
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo-re-3.png");

            // Attach image and get CID
            var image = builder.LinkedResources.Add(imagePath);
            image.ContentId = "logoImage";

            builder.HtmlBody = GetOtpHtmlContent(otp, userDetails.FirstName, userDetails.LastName);
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(emailSettings.Host, emailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(emailSettings.Email, emailSettings.Password);
            await smtp.SendAsync(email);

            string query = "INSERT INTO dbo.OTPLogs (Email, OTPCode, ExpireDate, CreatedDate) VALUES (@Email, @OTPCode, @ExpireDate, @CreatedDate);";
            int affectedRow = await _dapper.ExecuteAsync(query, new
            {
                Email = otpRequest.Email,
                OTPCode = otp,
                ExpireDate = DateTimeOffset.Now.Add(_otpExpiry),
                CreatedDate = DateTimeOffset.Now
            });

            smtp.Disconnect(true);
        }
        private string GetOtpHtmlContent(string otp, string firstName, string lastName)
        {
            string response = $@"
        <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0""
            style=""background-color: #f4f4f7; padding: 40px 0;"">
            <tr>
                <td align=""center"" style=""padding: 30px;"">
                    <table role=""presentation"" width=""600"" cellpadding=""0"" cellspacing=""0""
                        style=""background: #ffffff; border-radius: 5px; overflow: hidden; box-shadow: 0 4px 20px rgba(0, 0, 0, 0.05); font-family: Arial, sans-serif; color: #333;"">
                        <tr>
                            <td align=""center"" style=""padding: 20px 0;"">
                                <img style=""display: block; width: 200px;"" src=""cid:logoImage"" alt=""Hublog Logo""/>
                            </td>
                        </tr>
                        <tr><td style=""border-top: 1px solid #eaeaec;""></td></tr>
                        <tr>
                            <td style=""padding: 30px 40px 0px; font-size: 16px;"">
                                <p style=""margin: 0; font-weight: 600;"">Hi {firstName + " " + lastName},</p>
                                <p style=""line-height: 1.6; margin-top: 12px;"">
                                    Your One Time Password (OTP) is <strong>{otp}</strong>. This OTP will be valid for next 5 mins.
                                </p>
                            </td>
                        </tr>
                     
                        <tr>
                            <td style=""padding: 4px 40px 30px;"">
                                <p style=""margin: 0;"">Best Regards,</p>
                                <p style=""margin: 2px 0 0;"">The Workstatus Team</p>
                            </td>
                        </tr>
                        <tr><td style=""border-top: 1px solid #eaeaec;""></td></tr>
                        <tr>
                            <td align=""center"" style=""font-size: 12px; color: #999; padding: 20px;"">
                                &copy; 2025 Workstatus. All rights reserved.
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>";
            return response;
        }

        public async Task<(string FirstName, string LastName)> GetUserDetailsByEmailAsync(string email)
        {
            string query = @"SELECT First_Name, Last_Name FROM Users WHERE Email = @Email";

            return await _dapper.QueryFirstOrDefaultAsync<(string, string)>(query, new { Email = email });
        }

        public async Task<bool> ValidateOTP(OtpValidationRequest otpValidation)
        {
            var query = @"select top 1 Id, Email, OTPCode, IsUsed, ExpireDate from dbo.OTPLogs where Email = @Email and OTPCode = @OTPCode order by CreatedDate desc";
            var parameter = new
            {
                Email = otpValidation.EmailId,
                OTPCode = otpValidation.Otp
            };
            var result = await _dapper.GetAsync<dynamic>(query, parameter);

            if (result == null)
                throw new Exception("Invalid OTP");

            if (result.IsUsed)
                throw new Exception("OTP already used");

            if (DateTimeOffset.Now > result.ExpireDate)
                throw new Exception("OTP expired. Please request a new one.");

            string updateQuery = @"update dbo.OTPLogs set IsUsed = 1 where Id = @Id";
            int count = await _dapper.ExecuteAsync(updateQuery, new { Id = result.Id });

            if (count > 0)
                return true;
            return false;
        }
    }
}
