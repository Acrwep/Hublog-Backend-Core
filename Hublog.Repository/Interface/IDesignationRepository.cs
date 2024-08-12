using Hublog.Repository.Entities.Model;

namespace Hublog.Repository.Interface
{
    public interface IDesignationRepository
    {
        Task<List<Designation>> GetDesignationAll(int organizationId);
    }
}
