using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hublog.Repository.Common;
using Hublog.Repository.Entities.Model.Break;
using Hublog.Repository.Entities.Model.Goals;
using Hublog.Repository.Entities.Model.Shift;
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


        #region GetGoals
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
                    var getUpdatedValue= await _dapper.GetAsync<Goal>(selectQuery, new { Id = goal.Id });
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
    }
}
