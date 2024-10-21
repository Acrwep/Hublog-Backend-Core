using Hublog.Repository.Entities.Model;

namespace Hublog.Repository.Interface
{
    public interface IDesignationRepository
    {
        Task<List<Designation>> GetDesignationAll(int organizationId, string searchQuery);

        Task<Designation> GetDesignationById(int organizationId ,int designationId);

        Task<int> InsertDesignation(Designation designation);  

        Task<(int RowsAffected, string Message)> UpdateDesignation(Designation designation);

        Task<int> DeleteDesignation(int organizationId, int designationId);

        Task<bool> IsDesignationMappedToUser(int designationId);
    }
}
