using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.WellNess_Model
{
    public class WellNessBreakdown
    {
        public DateTime Date {  get; set; }
        public int Teamid { get; set; }
        public string TeamName { get; set; }
        public int? UserId { get; set; }
        public string? Fullname { get; set; }
        public int? ActiveTime { get; set; }
        public int? Healthy { get; set; }
        public int? Overburdened { get; set; }
        public int? Underutilized { get; set; }


    }
}
