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
        public int? Duration { get; set; }     // Total duration (Active + Idle)

        // Summary information
        public int? TotalActiveTime { get; set; }
        public double? TotalActiveTimePer { get; set; }
        public int? TotalIdleDuration { get; set; }
    }
}
