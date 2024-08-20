using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.Repository.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly Dapperr _dapper;
        public RoleRepository(Dapperr dapper)
        {
            _dapper = dapper;
        }

        #region  GetRoleByOrganizationId
        public async Task<List<Role>> GetRoleByOrganizationId(int organizationId)
        {
            var query = @"SELECT * FROM Role WHERE OrganizationId = @OrganizationId";
            var parameter = new { OrganizationId = organizationId };

            return await _dapper.GetAllAsync<Role>(query, parameter);
        }
        #endregion

        #region GetRoleAll
        public async Task<List<Role>> GetRoleAll()
        {
            var query = @"SELECT * FROM Role";
            return await _dapper.GetAllAsync<Role>(query);
        }
        #endregion

        #region InsertRole
        public async Task<int> InsertRole(Role role)
        {
            var query = @"INSERT INTO Role (Name, AccessLevel, Description, Admin, URLS, ScreenShot, LiveStream, OrganizationId) 
                          VALUES (@Name, @AccessLevel, @Description, @Admin, @URLS, @ScreenShot, @LiveStream, @OrganizationId)";

            return await _dapper.ExecuteAsync(query, role);
        }
        #endregion

        #region UpdateRole
        public async Task<int> UpdateRole(Role role)
        {
            var query = @"UPDATE Role 
                            SET Name = @Name, 
                                AccessLevel = @AccessLevel, 
                                Description = @Description, 
                                Admin = @Admin, 
                                URLS = @URLS, 
                                ScreenShot = @ScreenShot, 
                                LiveStream = @LiveStream, 
                                OrganizationId = @OrganizationId 
                            WHERE Id = @Id";

            return await _dapper.ExecuteAsync(query, role); 
        }
        #endregion

        #region DeleteRole
        public async Task<int> DeleteRole(int roleId)
        {
            var query = @"DELETE FROM Role WHERE Id = @Id";
            var parameter = new { Id = roleId };

            return await _dapper.ExecuteAsync(query, parameter);
        }
        #endregion
    }
}
