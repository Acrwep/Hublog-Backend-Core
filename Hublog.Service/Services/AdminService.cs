using Hublog.Repository.Entities.Model;
using Hublog.Repository.Interface;
using Hublog.Repository.Repositories;
using Hublog.Service.Interface;
using System.Security.Claims;

namespace Hublog.Service.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<List<BreakMaster>> GetBreakMasters()
        {
            return await _adminRepository.GetBreakMasters();
        }

        public async Task<List<Users>> GetAllUser(string loggedInUserEmail)
        {
            var users = await _adminRepository.GetAllUser();

            if (users != null && users.Any())
            {
                return users.OrderByDescending(u => u.Email == loggedInUserEmail).ToList();
            }

            return new List<Users>();
        }

        public async Task<BreakMaster> InsertBreakMaster(BreakMaster breakMaster)
        {
            return await _adminRepository.InsertBreakMaster(breakMaster);
        }

        public async Task<BreakMaster> UpdateBreakMaster(BreakMaster breakMaster)
        {
            return await _adminRepository.UpdateBreakMaster(breakMaster);
        }
    }
}
