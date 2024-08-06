using Hublog.Repository.Entities.Model;
using Hublog.Repository.Interface;
using Hublog.Service.Interface;

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

        public async Task<List<Users>> GetAllUser()
        {
            return await _adminRepository.GetAllUser();
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
