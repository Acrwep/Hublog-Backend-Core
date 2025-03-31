using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.Organization;
using Hublog.Repository.Entities.Model.OTPRequest;
using Hublog.Repository.Entities.Model.UserModels;

namespace Hublog.Repository.Interface
{
    public interface IEmailRepository
    {
        Task SendEmailAsync(Users users);
        Task SendOrganizationEmailAsync(Organizations organizations);

        Task SendOtpEmailAsync(OtpRequest otpRequest, string otp);
    }
}
