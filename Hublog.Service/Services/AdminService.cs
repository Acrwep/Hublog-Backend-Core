﻿using Hublog.Repository.Entities.Model.Break;
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

        public async Task<List<BreakMaster>> GetBreakMasters(string searchQuery)
        {
            return await _adminRepository.GetBreakMasters(searchQuery);
        }

        public async Task<List<Users>> GetAllUser(string loggedInUserEmail, int organizationId)
        {
            var users = await _adminRepository.GetAllUser(organizationId);

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
