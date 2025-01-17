using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.WellNess_Model
{
    public class TeamWiseUsers
    {
        public int Teamid { get; set; }
        public string TeamName { get; set; }
        public int? UserId { get; set; }
        public string? Fullname { get; set; }
        public int? ActiveTime { get; set; }

    }
}
