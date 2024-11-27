using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.Productivity
{
    public class ProductivityDurations
    {
        public string TotalProductiveDuration { get; set; }
        public string TotalNeutralDuration { get; set; }
        public string TotalUnproductiveDuration { get; set; }
        public string TotalDuration { get; set; }
        public string AverageDuratiopn {  get; set; }
        public string productive_percent {  get; set; }
        public string LeastProductiveDuration { get; set; }
        public string Team_name { get; set; }
        public string LeastTeamName { get; set; }
        public string LeastTeamPercentage{ get;set; }
       
    }
}
