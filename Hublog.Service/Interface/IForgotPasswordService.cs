using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Service.Interface
{
    public interface IForgotPasswordService
    {
        Task<bool> CheckEmailExistsAsync(string email);
    }
}
