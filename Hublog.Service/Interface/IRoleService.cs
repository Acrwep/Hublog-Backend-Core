using Hublog.Repository.Entities.Model;

namespace Hublog.Service.Interface
{
    public interface IRoleService
    {
        Task<List<Role>> GetRoleByOrganizationId(int organizationId); 

        Task<Role> InsertRole(Role role);

        Task<Role> UpdateRole(Role role);   

        Task<bool> DeleteRole(int roleId);

    }
}
