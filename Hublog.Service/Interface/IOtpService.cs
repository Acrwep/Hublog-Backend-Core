using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Service.Interface
{
    public interface IOtpService
    {
        bool ValidateOtp(string userId, string otp);
    }
}

