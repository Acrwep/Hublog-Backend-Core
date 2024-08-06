using Hublog.Repository.Entities.Login;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Interface;
using Hublog.Repository.Repositories;
using Hublog.Service.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Hublog.Service.Services
{
    public class LoginService : ILoginService
    {
        private readonly ILoginRepository _loginRepository;
        private readonly JWTSetting _jwtSetting;

        public LoginService(ILoginRepository loginRepository, IOptions<JWTSetting> jwtSetting)
        {
            _loginRepository = loginRepository;
            _jwtSetting = jwtSetting.Value;
        }

        #region AdminLogin
        public async Task<(Users, string)> AdminLogin(string email, string password)
        {
            var admin = await _loginRepository.AdminLogin(email, password);

            if (admin != null)
            {
                var token = CreateToken(admin);
                return (admin, token);
            }
            return (null, null);
        }
        #endregion

        #region Login
        public async Task<(UserDTO, string)> Login(LoginModels loginModel)
        {
            var user = await _loginRepository.Login(loginModel.UserName, loginModel.Password);
            if (user == null)
            {
                return (null, null);
            }

            var userDto = new UserDTO
            {
                Id = user.Id,
                First_Name = user.First_Name,
                Last_Name = user.Last_Name,
                Email = user.Email,
                DOB = user.DOB,
                DOJ = user.DOJ,
                Phone = user.Phone,
                UsersName = user.UsersName,
                Gender = user.Gender,
                OrganizationId = user.OrganizationId,
                RoleId = user.RoleId,
                DesignationId = user.DesignationId,
                TeamId = user.TeamId,
                Active = user.Active,
                RoleName = user.RoleName,
                AccessLevel = user.AccessLevel,
                DesignationName = user.DesignationName,
                TeamName = user.TeamName,
                EmployeeID = user.EmployeeID
            };

            var token = CreateToken(user);
            return (userDto, token);
        }
        #endregion

        #region UserLogin
        public async Task<(Users, string)> UserLogin(string email, string password)
        {
            var user = await _loginRepository.UserLogin(email, password);

            if (user != null)
            {
                var token = CreateToken(user);
                return (user, token);
            }

            return (null, null);
        }
        #endregion

        #region CreateToken

        private string CreateToken(Users user)
        {
            var sec = _jwtSetting.Secret ?? throw new ArgumentException("Secret is not configured.");
            var issuer = _jwtSetting.Issuer;
            var audience = _jwtSetting.Audience;

            var now = DateTime.UtcNow;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(sec));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(ClaimTypes.Role, user.RoleName),
        new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(now).ToUnixTimeSeconds().ToString()),
        new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString())
    };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = now.AddDays(7),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        #endregion
    }
}
