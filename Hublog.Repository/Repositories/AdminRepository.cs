using System.Data;
using System.Data.Common;
using Dapper;
using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Entities.Model.AlertModel;
using Hublog.Repository.Entities.Model.Break;
using Hublog.Repository.Entities.Model.Shift;
using Hublog.Repository.Entities.Model.UserModels;
using Hublog.Repository.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

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
        public async Task<List<BreakMaster>> GetBreakMasters(int organizationId, string? searchQuery)
        {
            try
            {
                var query = @"SELECT * FROM BreakMaster WHERE organizationId = @OrganizationId
            AND (@SearchQuery IS NULL OR Name LIKE @SearchQuery)";

                var parameters = new
                {
                    OrganizationId = organizationId,
                    SearchQuery = searchQuery != null ? $"%{searchQuery}%" : null
                };

                return await _dapper.GetAllAsync<BreakMaster>(query, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching breakmaster record", ex);
            }
        }
        #endregion


        //shiftmaster

        public async Task<bool> IsNameExistsAsync(string name, int organizationId)
        {
            const string query = "SELECT COUNT(1) FROM Shift WHERE name = @Name AND OrganizationId = @OrganizationId";
            int count = await _dapper.ExecuteScalarAsync<int>(query, new { Name = name, OrganizationId = organizationId });
            return count > 0;
        }
       
        public async Task<bool> UpdateNameExistsAsync(string name, int organizationId, int id)
        {
            const string query = @"SELECT COUNT(1) FROM Shift WHERE name = @Name AND OrganizationId = @OrganizationId AND Id <> @Id";
            int count = await _dapper.ExecuteScalarAsync<int>(query, new { Name = name, OrganizationId = organizationId, Id = id });
            return count > 0;
        }
        public async Task<ShiftMaster> InsertShiftMaster(ShiftMaster shiftMaster)
        {
            try
            {
                
                // Check if the name already exists
                bool nameExists = await IsNameExistsAsync(shiftMaster.Name, shiftMaster.OrganizationId);
                if (nameExists)
                {
                    return null; // Returning null to indicate a duplicate
                }

                const string query = @"INSERT INTO Shift (OrganizationId, name, start_time, end_time, status, GraceTime)
                VALUES (@OrganizationId, @name, @start_time, @end_time, @status, @GraceTime);
                SELECT CAST(SCOPE_IDENTITY() as int)";


                ///default code


                //var createdShiftmaster = await _dapper.ExecuteAsync(query, shiftMaster);
                //shiftMaster.Id = createdShiftmaster;
                //return shiftMaster;

                int newId = await _dapper.GetSingleAsync<int>(query, shiftMaster);
                shiftMaster.Id = newId;
                return shiftMaster;

            }
            catch (Exception ex)
            {
                throw new Exception("Error creating shiftmaster", ex);
            }
        }

        #region GetShiftMasters
        public async Task<List<ShiftMaster>> GetShiftMasters(int organizationId, string? searchQuery)
        {
            try
            {
                var query = @"SELECT * FROM Shift WHERE OrganizationId = @OrganizationId
            AND (@SearchQuery IS NULL OR Name LIKE @SearchQuery)";

                var parameters = new
                {
                    OrganizationId = organizationId,
                    SearchQuery = searchQuery != null ? $"%{searchQuery}%" : null
                };

                return await _dapper.GetAllAsync<ShiftMaster>(query, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching shiftmaster record", ex);
            }
        }
        ///
        public async Task<bool> IsShiftMappedToTeam(int shiftId)
        {
            string query = "SELECT COUNT(*) FROM Team WHERE shiftId = @ShiftId";
            var count = await _dapper.ExecuteScalarAsync<int>(query, new { ShiftId = shiftId });
            return count > 0;
        }

        #endregion

        #region UpdateShiftMasters
        public async Task<object> UpdateShiftMaster(ShiftMaster shiftMaster)
        {
            try
            {
                // Check if the name already exists
                bool nameExists = await UpdateNameExistsAsync(shiftMaster.Name, shiftMaster.OrganizationId, shiftMaster.Id);
                if (nameExists)
                {
                    return null; // Returning null to indicate a duplicate
                }
                if (shiftMaster.Status == false)
                {
                    bool isMapped = await IsShiftMappedToTeam(shiftMaster.Id); 
                    if (isMapped)
                    {
                        return "Unable to inactive.Mapped to team";
                    }
                }

                string query = @" UPDATE Shift 
                                  SET OrganizationId=@OrganizationId, Name = @Name, Start_Time = @Start_Time, End_Time = @End_Time, Status = @Status, GraceTime=@GraceTime
                                  WHERE Id = @Id";

                var result = await _dapper.ExecuteAsync(query, shiftMaster);

                if (result > 0)
                {
                    string selectQuery = @"
                                           SELECT Id, OrganizationId, Name, Start_Time, End_Time, Status, GraceTime
                                           FROM Shift
                                           WHERE Id = @Id";

                    return await _dapper.GetAsync<ShiftMaster>(selectQuery, new { Id = shiftMaster.Id });
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating ShiftMaster", ex);
            }
        }


        #endregion
        public async Task<int> DeleteShiftMaster(int organizationId, int shiftId)
        {
            var query = @"DELETE FROM Shift WHERE OrganizationId = @OrganizationId AND Id = @Id";
            var parameter = new { OrganizationId = organizationId, Id = shiftId };
            return await _dapper.ExecuteAsync(query, parameter);
        }

      
    }
}
