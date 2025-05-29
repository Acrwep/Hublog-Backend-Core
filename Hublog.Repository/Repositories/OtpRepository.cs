using System.Security.Cryptography;
using Hublog.Repository.Interface;

namespace Hublog.Repository.Repositories
{
    public class OtpRepository : IOtpRepository
    {

        //private readonly IMemoryCache _cache;

        //public OtpRepository(IMemoryCache cache)
        //{
        //    _cache = cache;
        //}

        public string GenerateOtp(string userId, int length = 6)
        {
            const string chars = "0123456789";
            var randomBytes = new byte[length];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }

            char[] otpChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                otpChars[i] = chars[randomBytes[i] % chars.Length];
            }

            string otp = new string(otpChars);
            //_cache.Set(userId, otp, _otpExpiry);
            return otp;
        }

        //public bool ValidateOtp(string userId, string enteredOtp)
        //{
        //    if (_cache.TryGetValue(userId, out string storedOtp) && storedOtp == enteredOtp)
        //    {
        //        _cache.Remove(userId);
        //        return true;
        //    }
        //    return false;
        //}
       
    }
}
