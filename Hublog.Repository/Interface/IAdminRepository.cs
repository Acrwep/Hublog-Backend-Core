using Hublog.Repository.Entities.Model.Break;
using Hublog.Repository.Entities.Model.Shift;
using Hublog.Repository.Entities.Model.UserModels;

namespace Hublog.Repository.Interface
{
    public interface IAdminRepository
    {
        Task<List<BreakMaster>> GetBreakMasters(int organizationId,string? searchQuery);

        Task<BreakMaster> InsertBreakMaster(BreakMaster breakMaster);

        Task<BreakMaster> UpdateBreakMaster(BreakMaster breakMaster);


        //shiftmaster
        Task<bool> IsNameExistsAsync(string name, int organizationId);
       

        Task<ShiftMaster> InsertShiftMaster(ShiftMaster shiftMaster);

        Task<List<ShiftMaster>> GetShiftMasters(int organizationId, string? searchQuery);

        Task<ShiftMaster> UpdateShiftMaster(ShiftMaster shiftMaster);

        Task<int> DeleteShiftMaster(int organizationId, int shiftId);

    }
}
