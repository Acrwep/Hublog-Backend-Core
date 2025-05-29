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

        Task<(string FirstName, string LastName)> GetUserDetailsByEmailAsync(string email);

        Task<bool> ValidateOTP(OtpValidationRequest otpValidation);
    }
}
