using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.Activity
{
    public class DailyActivityDuration
    {
        public string Date { get; set; }
        public double TotalDuration { get; set; } 
        public double OnlineTime { get; set; }
        public double IdleDuration { get; set; }
        public double BreakDuration{  get; set; }
        public double ActiveDuration { get; set; }

        // Add these properties to store formatted durations
        public string Total_Duration { get; set; }
        public string Online_Time { get; set; }
        public string Idle_Duration { get; set; }
        public string Break_Duration { get; set; }
        public string Active_Duration { get; set; }
        public double ActiveTime { get; internal set; }
    }
}
