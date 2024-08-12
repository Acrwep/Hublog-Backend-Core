using Hublog.Repository.Entities.Model;

namespace Hublog.Service.Interface
{
    public interface IDesignationService
    {
        Task<List<Designation>> GetDesignationAll(int organizationId);
    }
}
