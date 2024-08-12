using Hublog.Repository.Entities.Model;

namespace Hublog.Service.Interface
{
    public interface IDesignationService
    {
        Task<List<Designation>> GetDesignationAll(int organizationId);

        Task<Designation> GetDesignationById(int organizationId, int designationId);

        Task<Designation> InsertDesignation(Designation designation);  
        
        Task<Designation> UpdateDesignation(Designation designation);

        Task<bool> DeleteDesignation(int organizationId, int designationId);
    }
}
