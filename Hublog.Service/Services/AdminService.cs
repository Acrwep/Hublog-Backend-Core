using Hublog.Repository.Entities.Model;
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
            //defaul code
            //return await _adminRepository.InsertShiftMaster(shiftMaster);

            var createdShiftMaster = await _adminRepository.InsertShiftMaster(shiftMaster);
            if (createdShiftMaster == null)
            {
                throw new InvalidOperationException("The name already exists");
            }
            return createdShiftMaster;
        }
        public async Task<List<ShiftMaster>> GetShiftMasters(int organizationId, string? searchQuery)
        {
            return await _adminRepository.GetShiftMasters(organizationId, searchQuery);
        }

        public async Task<ShiftMaster> UpdateShiftMaster(ShiftMaster shiftMaster)
        {
            //defaul code
            //return await _adminRepository.UpdateShiftMaster(shiftMaster);

            var updatedShiftMaster = await _adminRepository.UpdateShiftMaster(shiftMaster);
            if (updatedShiftMaster == null)
            {
                throw new InvalidOperationException("The name already exists");
            }
            return updatedShiftMaster;
        }

        public async Task<bool> DeleteShiftMaster(int organizationId, int shiftId)
        {
            var result = await _adminRepository.DeleteShiftMaster(organizationId, shiftId);
            return result > 0;
        }

    }
}
