using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.WellNess_Model
{
    public class WellNess
    {
        public int OrganizationId { get; set; }
        public int? TeamId { get; set; }
        public string? TeamName { get; set; }
        public int? Underutilized { get; set; }
        public int? Healthy { get; set; }
        public int? Overburdened { get; set; }
            
    }
}
