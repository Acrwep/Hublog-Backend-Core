using Hublog.Repository.Entities.Model.Break;
using Hublog.Repository.Entities.Model.Shift;
using Hublog.Repository.Entities.Model.UserModels;

namespace Hublog.Service.Interface
{
    public interface IAdminService
    {

        Task<List<BreakMaster>> GetBreakMasters(int organizationId, string? searchQuery);

        Task<BreakMaster> InsertBreakMaster(BreakMaster breakMaster);

        Task<BreakMaster> UpdateBreakMaster(BreakMaster breakMaster);


        //shiftmaster
        Task<ShiftMaster> InsertShiftMaster(ShiftMaster shiftMaster);

        Task<List<ShiftMaster>> GetShiftMasters(int organizationId, string? searchQuery);

        Task<ShiftMaster> UpdateShiftMaster(ShiftMaster shiftMaster);

        Task<bool> DeleteShiftMaster(int organizationId, int shiftId);

      
    }
}
