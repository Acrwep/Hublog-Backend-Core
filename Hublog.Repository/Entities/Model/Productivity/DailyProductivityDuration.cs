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
        public double TotalDuration { get; set; }
        public double ProductiveDuration { get; set; }
        public double UnproductiveDuration { get; set; }
        public double NeutralDuration { get; set; }
    }
}
