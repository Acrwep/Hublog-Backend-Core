using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hublog.Repository.Interface;
using Hublog.Service.Interface;

namespace Hublog.Service.Services
{
    public class ForgotPasswordService : IForgotPasswordService
    {
        private readonly IForgotPasswordRepository _passwordRepository;
        public ForgotPasswordService(IForgotPasswordRepository passwordRepository)
        {
            _passwordRepository = passwordRepository;
        }
        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _passwordRepository.CheckEmailExistsAsync(email);
        }

        public async Task<(bool isUpdated, bool newUser)> UpdatePasswordAsync(string email, string newPassword)
        {
            return await _passwordRepository.UpdatePasswordAsync(email, newPassword);
        }

        //public async Task<bool> UpdatePasswordAsync(string email, string newPassword)
        //{
        //   return await _passwordRepository.UpdatePasswordAsync(email, newPassword);
        //}
    }
}
