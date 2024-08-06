﻿using Hublog.Repository.Entities.Model;

namespace Hublog.Repository.Interface
{
    public interface IAdminRepository
    {
        Task<List<Users>> GetAllUser();

        Task<List<BreakMaster>> GetBreakMasters();

        Task<BreakMaster> InsertBreakMaster(BreakMaster breakMaster);

        Task<BreakMaster> UpdateBreakMaster(BreakMaster breakMaster);
    }
}
