﻿using Hublog.Repository.Entities.Model;

namespace Hublog.Repository.Interface
{
    public interface IAdminRepository
    {
        Task<List<Users>> GetAllUser(int organizationId);

        Task<List<BreakMaster>> GetBreakMasters(string searchQuery);

        Task<BreakMaster> InsertBreakMaster(BreakMaster breakMaster);

        Task<BreakMaster> UpdateBreakMaster(BreakMaster breakMaster);
    }
}
