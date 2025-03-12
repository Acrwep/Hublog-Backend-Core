using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.Goals
{
    public class Goal
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; } 
        public string WorkingTime { get; set; }
        public string ProductiveTime { get; set; }
        
    }
}
