using Hublog.Repository.Entities.Model;

namespace Hublog.Repository.Interface
{
    public interface IDesignationRepository
    {
        Task<List<Designation>> GetDesignationAll(int organizationId);

        Task<Designation> GetDesignationById(int organizationId ,int designationId);

        Task<int> InsertDesignation(Designation designation);
    }
}
