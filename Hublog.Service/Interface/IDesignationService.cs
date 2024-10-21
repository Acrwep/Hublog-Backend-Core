using Hublog.Repository.Entities.Model;

namespace Hublog.Service.Interface
{
    public interface IDesignationService
    {
        Task<List<Designation>> GetDesignationAll(int organizationId, string searchQuery);

        Task<Designation> GetDesignationById(int organizationId, int designationId);

        Task<Designation> InsertDesignation(Designation designation);  
        
        Task<(Designation UpdatedDesignation, string Message)> UpdateDesignation(Designation designation);

        Task<bool> DeleteDesignation(int organizationId, int designationId);
    }
}
