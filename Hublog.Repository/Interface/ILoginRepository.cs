using Hublog.Repository.Entities.Model.UserModels;

namespace Hublog.Repository.Interface
{
    public interface ILoginRepository
    {
        Task<Users> Login(string email, string password);

        Task<Users> UserLogin(string email, string password);

        Task<Users> AdminLogin(string email, string password);
    }
}
