using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Interface
{
    public interface IForgotPasswordRepository
    {
        Task<bool> CheckEmailExistsAsync(string email);
    }
}
