using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.Productivity
{
    public class AttendanceResponse
    {
        public int UserID { get; set; }
        public string team_Name { get; set; }
        public string full_Name { get; set; }
        public int AttendanceCount { get; set; }
        public dynamic total_wokingtime { get; set; }
        public dynamic BreakDuration { get; set; }
        public dynamic OnlineDuration { get; set; }
        public dynamic TotalProductiveDuration { get; set; }
        public dynamic TotalUnproductiveDuration { get; set; }
        public dynamic TotalNeutralDuration { get; set; }
        public dynamic TotalDuration { get; set; }
        public decimal PercentageProductiveDuration { get; set; }
    }
}
