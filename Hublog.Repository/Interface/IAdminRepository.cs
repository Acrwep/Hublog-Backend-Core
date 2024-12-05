using Hublog.Repository.Entities.Model.Break;
using Hublog.Repository.Entities.Model.UserModels;

namespace Hublog.Repository.Interface
{
    public interface IAdminRepository
    {
        Task<List<BreakMaster>> GetBreakMasters(int organizationId,string? searchQuery);

        Task<BreakMaster> InsertBreakMaster(BreakMaster breakMaster);

        Task<BreakMaster> UpdateBreakMaster(BreakMaster breakMaster);
    }
}
