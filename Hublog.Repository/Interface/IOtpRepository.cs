using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Interface
{
    public interface IOtpRepository
    {
      
        string GenerateOtp(string userId, int length = 6);
        bool ValidateOtp(string userId, string enteredOtp);
    }
}
