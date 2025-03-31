using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.Organization;
using Hublog.Repository.Entities.Model.OTPRequest;
using Hublog.Repository.Entities.Model.UserModels;
using Hublog.Repository.Interface;
using Hublog.Repository.Repositories;
using Hublog.Service.Interface;

namespace Hublog.Service.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmailRepository _emailRepository;
        private readonly IOtpRepository _otpRepository;
        public EmailService(IEmailRepository emailRepository, IOtpRepository otpRepository)
        {
            _emailRepository = emailRepository;
            _otpRepository = otpRepository;
        }
        public async Task SendEmailAsync(Users users)
        {
            await _emailRepository.SendEmailAsync(users);
        }

        public async Task SendOrganizationEmailAsync(Organizations organizations)
        {
            await _emailRepository.SendOrganizationEmailAsync(organizations);
        }

        public async Task SendOtpAsync(OtpRequest otpRequest)
        {
            string otp = _otpRepository.GenerateOtp(otpRequest.Email);
            await _emailRepository.SendOtpEmailAsync(otpRequest, otp);
        }
    }
}
