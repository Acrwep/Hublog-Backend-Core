using Hublog.Repository.Entities.Model;
using Hublog.Repository.Interface;
using Hublog.Service.Interface;

namespace Hublog.Service.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<List<Role>> GetRoleByOrganizationId(int organizationId)
        {
            return await _roleRepository.GetRoleByOrganizationId(organizationId);
        }

        #region InsertRole
        public async Task<Role> InsertRole(Role role)
        {
            var result = await _roleRepository.InsertRole(role);
            if(result > 0)
            {
                return role;
            }
            else
            {
                throw new Exception("Could not create role");
            }
        }
        #endregion

        #region UpdateRole
        public async Task<Role> UpdateRole(Role role)
        {
            var result = await _roleRepository.UpdateRole(role);
            if(result > 0)
            {
                return role;
            }
            return null;
        }
        #endregion

        #region DeleteRole
        public async Task<bool> DeleteRole(int roleId)
        {
            var result = await _roleRepository.DeleteRole(roleId);
            return result > 0;
        }
        #endregion
    }
}
