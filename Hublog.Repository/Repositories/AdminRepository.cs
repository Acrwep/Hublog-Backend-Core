using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Interface;
using Microsoft.Data.SqlClient;

namespace Hublog.Repository.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly Dapperr _dapper;
        public AdminRepository(Dapperr dapper)
        {
            _dapper = dapper;
        }

        #region UpdateBreakMaster
        public async Task<BreakMaster> UpdateBreakMaster(BreakMaster breakMaster)
        {
            try
            {
                string query = @" UPDATE BreakMaster 
                                  SET Name = @Name, Max_Break_Time = @Max_Break_Time, Active = @Active, OrganizationId = @OrganizationId
                                  WHERE Id = @Id";

                var result = await _dapper.ExecuteAsync(query, breakMaster);

                if(result > 0)
                {
                    string selectQuery = @"
                                           SELECT Id, Name, Max_Break_Time, Active, OrganizationId
                                           FROM BreakMaster
                                           WHERE Id = @Id";

                    return await _dapper.GetAsync<BreakMaster>(selectQuery, new { Id = breakMaster.Id });
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating Breakmaster", ex);
            }
        }
        #endregion

        #region InsertBreakMaster
        public async Task<BreakMaster> InsertBreakMaster(BreakMaster breakMaster)
        {
            try
            {
                const string query = @"INSERT INTO BreakMaster (Name, Max_Break_Time, Active, OrganizationId)
                VALUES (@Name, @Max_Break_Time, @Active, @OrganizationId);
                SELECT CAST(SCOPE_IDENTITY() as int)";

                var createdBreakmaster = await _dapper.ExecuteAsync(query, breakMaster);
                breakMaster.Id = createdBreakmaster;
                return breakMaster;

            }
            catch (Exception ex)
            {
                throw new Exception("Error creating Breakmaster", ex);
            }
        }
        #endregion

        #region GetBreakMasters
        public async Task<List<BreakMaster>> GetBreakMasters()
        {
            try
            {
                const string query = @"SELECT * FROM BreakMaster";
                return await _dapper.GetAllAsync<BreakMaster>(query);
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching breakmaster record", ex);
            }
        }
        #endregion

        #region GetAllUser
        public async Task<List<Users>> GetAllUser()
        {
            try
            {
                const string query = @"SELECT * FROM Users WITH (NOLOCK)";
                return await _dapper.GetAllAsync<Users>(query);
            }
            catch (SqlException Sqlex)
            {
                throw new Exception("Database query Failed", Sqlex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching the data", ex);
            }
        }
        #endregion
    }
}
