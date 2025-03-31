using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.Organization;
using Hublog.Repository.Entities.Model.OTPRequest;
using Hublog.Repository.Entities.Model.UserModels;
using Hublog.Repository.Repositories;

namespace Hublog.Service.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(Users users);
        Task SendOrganizationEmailAsync(Organizations organizations);

        Task SendOtpAsync(OtpRequest otpRequest);
    }
}
