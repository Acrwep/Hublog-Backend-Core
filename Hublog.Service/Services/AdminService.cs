using Hublog.Repository.Entities.Model.Break;
using Hublog.Repository.Entities.Model.Shift;
using Hublog.Repository.Entities.Model.UserModels;
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

        public async Task<List<BreakMaster>> GetBreakMasters(int organizationId, string? searchQuery)
        {
            return await _adminRepository.GetBreakMasters(organizationId,searchQuery);
        }

        public async Task<BreakMaster> InsertBreakMaster(BreakMaster breakMaster)
        {
            return await _adminRepository.InsertBreakMaster(breakMaster);
        }

        public async Task<BreakMaster> UpdateBreakMaster(BreakMaster breakMaster)
        {
            return await _adminRepository.UpdateBreakMaster(breakMaster);
        }

        //shiftmaster
        public async Task<ShiftMaster> InsertShiftMaster(ShiftMaster shiftMaster)
        {
            return await _adminRepository.InsertShiftMaster(shiftMaster);
        }
        public async Task<List<ShiftMaster>> GetShiftMasters(int organizationId, string? searchQuery)
        {
            return await _adminRepository.GetShiftMasters(organizationId, searchQuery);
        }
    }
}
