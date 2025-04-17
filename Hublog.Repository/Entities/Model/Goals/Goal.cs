using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.Goals
{
    public class Goal
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; } 
        public int WorkingTime { get; set; }
        public int ProductiveTime { get; set; }
        
    }

    public class GoalStatsDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public int AchievedDays { get; set; }
        public int TotalWorkingSeconds { get; set; }
        public int TotalProductiveSeconds { get; set; }
    }

}
