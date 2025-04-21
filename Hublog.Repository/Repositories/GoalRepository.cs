using System.Data;
using Dapper;
using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model.Goals;
using Hublog.Repository.Entities.Model.Productivity;
using Hublog.Repository.Interface;

namespace Hublog.Repository.Repositories
{
    public class GoalRepository : IGoalRepository
    {

        private readonly Dapperr _dapper;

        public GoalRepository(Dapperr dapper)
        {
            _dapper = dapper;

        }

        #region GetGoals
        public async Task<List<Goal>> GetGoals(int organizationId)
        {
            try
            {
                var query = @"SELECT * FROM Goals WHERE OrganizationId = @OrganizationId";

                var parameters = new
                {
                    OrganizationId = organizationId
                };

                return await _dapper.GetAllAsync<Goal>(query, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching Goals record", ex);
            }
        }
        #endregion


        #region InsertGoals  
        public async Task<Goal> InsertGoals(Goal goal)
        {
            const string query = @"INSERT INTO Goals (OrganizationId, WorkingTime, ProductiveTime)
                VALUES (@OrganizationId, @WorkingTime, @ProductiveTime);
                SELECT CAST(SCOPE_IDENTITY() as int)";


            var createdGoals = await _dapper.ExecuteAsync(query, goal);
            goal.Id = createdGoals;
            return goal;
        }
        #endregion


        #region GetGoals
        public async Task<Goal> UpdateGoals(Goal goal)
        {
            try
            {
                string query = @" UPDATE Goals 
                                  SET OrganizationId = @OrganizationId, WorkingTime = @WorkingTime, ProductiveTime = @ProductiveTime
                                  WHERE Id = @Id"
                ;

                var result = await _dapper.ExecuteAsync(query, goal);

                if (result > 0)
                {
                    string selectQuery = @"
                                           SELECT Id, OrganizationId, WorkingTime, ProductiveTime
                                           FROM Goals
                                           WHERE Id = @Id"
                    ;
                    var getUpdatedValue = await _dapper.GetAsync<Goal>(selectQuery, new { Id = goal.Id });
                    return getUpdatedValue;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating Goals", ex);
            }
        }
        #endregion

        public async Task<dynamic> GetGoalsDetails(int organizationId, int? teamId, DateTime fromDate, DateTime toDate)
        {
            var result = await _dapper.GetAllAsyncs<GoalStatsDto>(
                "GetGoalsUserDailyStats",
                new
                {
                    OrganizationId = organizationId,
                    TeamId = teamId,
                    FromDate = fromDate,
                    ToDate = toDate
                },
                commandType: CommandType.StoredProcedure);

            var sorted = result.OrderByDescending(r => r.AchievedDays)
                   .ThenByDescending(r => r.TotalWorkingSeconds)
                   .ThenByDescending(r => r.TotalProductiveSeconds)
                   .ToList();

            var top = sorted
            .Where(x => x.AchievedDays > 0)
            .Take(3)
            .Select(x => new
            {
                userId = x.UserId,
                fullName = x.FullName,
                achievedDays = x.AchievedDays
            });


            var least = sorted.TakeLast(3)
                  .OrderBy(x => x.AchievedDays)
                  .ThenBy(x => x.TotalWorkingSeconds)
                  .ThenBy(x => x.TotalProductiveSeconds)
                  .Select(x => new
                  {
                      userId = x.UserId,
                      fullName = x.FullName,
                      achievedDays = x.AchievedDays
                  });

            return new { top, least };
        }
    }
}
