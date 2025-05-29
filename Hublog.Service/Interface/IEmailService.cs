using Hublog.Repository.Entities.Model.Organization;
using Hublog.Repository.Entities.Model.OTPRequest;
using Hublog.Repository.Entities.Model.UserModels;

namespace Hublog.Service.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(Users users);
        Task SendOrganizationEmailAsync(Organizations organizations);

        Task SendOtpAsync(OtpRequest otpRequest);

        Task<bool> ValidateOTP(OtpValidationRequest otpValidation);
    }
}
