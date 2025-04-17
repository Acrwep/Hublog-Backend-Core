using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.Productivity
{
    public class MostAndLeastTeams
    {
        public decimal grandTotalpercentage { get; set; }
        public string grandtotalTimeDuration { get; set; }
        public Data Data { get; set; }
    }

    public class Data
    {
        public List<Teams> top { get; set; }
        public List<Teams> bottom { get; set; }
    }

    public class Teams
    {
        public string team_name { get; set; }
        public string productive_duration { get; set; }
        public string unproductive_duration { get; set; }
        public string neutral_duration { get; set; }
        public string total_duration { get; set; }
        public decimal productive_percent { get; set; }
    }
}
