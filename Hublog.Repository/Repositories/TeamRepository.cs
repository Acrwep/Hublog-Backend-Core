using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Interface;
using Microsoft.Data.SqlClient;

namespace Hublog.Repository.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly Dapperr _dapper;
        public TeamRepository(Dapperr dapper)
        {
            _dapper = dapper;
        }

        #region UpdateTeam
        public async Task<Team> UpdateTeam(int id, Team team)
        {
            try
            {
                string query = @"UPDATE Team SET 
                                    Name = @Name, 
                                    Description = @Description, 
                                    Active = @Active, 
                                    OrganizationId = @OrganizationId, 
                                    Parentid = @Parentid WHERE Id = @Id";

                team.Id = id;

                var result = await _dapper.ExecuteAsync(query, team);

                if(result > 0)
                {
                    string selectquery = @"SELECT * FROM Team WHERE Id = @Id";
                    return await _dapper.GetAsync<Team>(selectquery, new { Id = team.Id });
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Updating Team", ex);
            }
        }
        #endregion

        #region CreateTeam
        public async Task<(bool IsSuccessful, string Message, Team CreatedTeam)> CreateTeam(Team team)
        {
            try
            {
                string checkQuery = @"SELECT COUNT(1) 
                                      FROM Team 
                                      WHERE Name = @Name AND OrganizationId = @OrganizationId";

                var teamExists = await _dapper.ExecuteScalarAsync<int>(checkQuery, new { team.Name, team.OrganizationId });

                if (teamExists > 0)
                {
                    return (false, "Team name already exist", null);
                }

                string insertQuery = @"INSERT INTO Team (Name, Active, Description, OrganizationId, Parentid) 
                                       VALUES (@Name, @Active, @Description, @OrganizationId, @Parentid)
                                       SELECT CAST(SCOPE_IDENTITY() as int)";

                var createdTeamId = await _dapper.ExecuteScalarAsync<int>(insertQuery, team);
                team.Id = createdTeamId;
                return (true, "Team created successfully.", team);
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating Team", ex);
            }
        }
        #endregion

        #region GetTeams
        public async Task<List<Team>> GetTeams(int organizationId)
        {
            try
            {
                return await _dapper.GetAllAsync<Team>("SELECT * FROM Team WHERE OrganizationId  = @OrganizationId AND Active = 1", new { OrganizationId = organizationId });
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching the Team", ex);
            }
        }
        #endregion

        #region DeleteTeam
        public async Task<string> DeleteTeam(int id)
        {
            try
            {
                string query = @"DELETE FROM Team WHERE Id = @Id";
                var result = await _dapper.ExecuteAsync(query, new { Id = id });
                return result > 0 ? "Team Deleted Successfully" : "Team cannot be deleted user exist in Team";
            }
            catch (SqlException sqlex)
            {
                throw new Exception("This team mapping some users", sqlex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error Deleteing Team", ex);
            }
        }
        #endregion
    }
}
