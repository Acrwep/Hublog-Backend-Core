using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hublog.Repository.Entities.Model.Break;
using Hublog.Repository.Entities.Model.Goals;
using Hublog.Repository.Entities.Model.Productivity;
using Microsoft.AspNetCore.Mvc;

namespace Hublog.Repository.Interface
{
    public interface IGoalRepository
    {
        Task<List<Goal>> GetGoals(int organizationId);

        Task<Goal> InsertGoals(Goal goal);

        Task<Goal> UpdateGoals(Goal goal);

        Task<dynamic> GetGoalsDetails(int organizationId, int? teamId,[FromQuery] DateTime fromDate, [FromQuery] DateTime toDate);

        Task<List<AppUsage>> GetAppUsages(int organizationId, int? teamId,int? userId, DateTime fromDate, DateTime toDate);
    }
}
