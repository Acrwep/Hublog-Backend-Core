using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hublog.Repository.Entities.Model.Goals;
using Hublog.Repository.Interface;
using Hublog.Service.Interface;

namespace Hublog.Service.Services
{
    public class GoalService : IGoalService
    {
        private readonly IGoalRepository _goalRepository;
        public GoalService(IGoalRepository goalRepository)
        {
            _goalRepository = goalRepository;
        }

        public async Task<List<Goal>> GetGoals(int organizationId)
        {
           var result=await _goalRepository.GetGoals(organizationId);
            return result;
        }


        public async Task<Goal> InsertGoals(Goal goal)
        {
            var result = await _goalRepository.InsertGoals(goal);
            return result;
        }


        public async Task<Goal> UpdateGoals(Goal goal)
        {
            var result = await _goalRepository.UpdateGoals(goal);
            return result;
        }
    }
}
