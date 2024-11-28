using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.Productivity
{
    public class DailyProductivityDuration
    {
        public string Date { get; set; }
        public double TotalDuration { get; set; } // Duration in seconds
        public double ProductiveDuration { get; set; }
        public double UnproductiveDuration { get; set; }
        public double NeutralDuration { get; set; }

        // Add these properties to store formatted durations
        public string Total_Duration { get; set; }
        public string Productive_Duration { get; set; }
        public string Unproductive_Duration { get; set; }
        public string Neutral_Duration { get; set; }
    }
}
