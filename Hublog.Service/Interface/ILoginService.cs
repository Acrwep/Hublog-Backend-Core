using Hublog.Repository.Entities.Login;
using Hublog.Repository.Entities.Model;

namespace Hublog.Service.Interface
{
    public interface ILoginService
    {
        Task<(UserDTO, string)> Login(LoginModels loginModel);

        Task<(Users, string)> UserLogin(string email, string password);

        Task<(Users, string)> AdminLogin(string email, string password);

        Task<(Users user, string token)> UserLogout(LoginModels model);
    }
}
