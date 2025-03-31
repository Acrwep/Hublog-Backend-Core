using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.OTPRequest
{
    public class OtpValidationRequest
    {
        public string EmailId { get; set; }
        public string Otp { get; set; }

    }
}
