using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model;
using Hublog.Repository.Interface;

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
        public async Task<Team> CreateTeam(Team team)
        {
            try
            {
                string query = @"INSERT INTO Team (Name, Active, Description, OrganizationId, Parentid) 
                                 VALUES (@Name, @Active, @Description, @OrganizationId, @Parentid)
                                 SELECT CAST(SCOPE_IDENTITY() as int)";

                var createdTeam = await _dapper.ExecuteAsync(query, team);
                team.Id = createdTeam;
                return team;

            }
            catch(Exception ex)
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
    }
}
