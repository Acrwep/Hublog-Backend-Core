using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.WellNess_Model
{
    public class WellnessUserDetails
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public int? TotalTime { get; set; }
        public int TotalPresent { get; set; }
        public int Healthy { get; set; }
        public int Overburdened { get; set; }
        public int Underutilized { get; set; }
        public double HealthyPercentage { get; set; }
        public double OverburdenedPercentage { get; set; }
        public double UnderutilizedPercentage { get; set; }

    }
}
