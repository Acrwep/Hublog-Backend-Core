using Hublog.Repository.Entities.Model;

namespace Hublog.Repository.Interface
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetRoleByOrganizationId(int organizationId);

        Task<List<Role>> GetRoleAll();

        Task<int> InsertRole(Role role);    

        Task<int> UpdateRole(Role role);

        Task<int> DeleteRole(int roleId);
    }
}
