using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.Activity
{
    public class Activity_Duration
    {
        public string TeamName { get; set; }  // Team name
        public int? ActiveTime { get; set; }   // Active time for the team
        public int? IdleDuration { get; set; } // Idle duration for the team
        public int? BreakDuration { get; set; }
        public int? Duration { get; set; }     // Total duration (Active + Idle)
        public int? OnlineTime { get; set; }

        // Summary information
        public int? TotalActiveTime { get; set; }
        public double? TotalActiveTimePer { get; set; }
        public int? TotalIdleDuration { get; set; }
        public int? TotalBreakDuration { get; set; }
        public int? TotalOnlineTime { get; set; }
    }
}
