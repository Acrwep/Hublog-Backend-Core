using Hublog.Repository.Entities.Model;

namespace Hublog.Service.Interface
{
    public interface IAdminService
    {
        //Task<List<Users>> GetAllUser();

        Task<List<Users>> GetAllUser(string loggedInUserEmail, int organizationId); 

        Task<List<BreakMaster>> GetBreakMasters();

        Task<BreakMaster> InsertBreakMaster(BreakMaster breakMaster);

        Task<BreakMaster> UpdateBreakMaster(BreakMaster breakMaster);
    }
}
