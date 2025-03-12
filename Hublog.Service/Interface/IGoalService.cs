using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hublog.Repository.Entities.Model.Break;
using Hublog.Repository.Entities.Model.Goals;

namespace Hublog.Service.Interface
{
    public interface IGoalService
    {
        Task<List<Goal>> GetGoals(int organizationId);

        Task<Goal> InsertGoals(Goal goal);

        Task<Goal> UpdateGoals(Goal goal);

    }
}
