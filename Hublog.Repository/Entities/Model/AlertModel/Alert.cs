using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.AlertModel
{
    public class Alert
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Full_Name { get; set; }
        public string Team_Name { get; set; }
        public string Triggered { get; set; }
        public DateTime? TriggeredTime { get; set; } 

    }
}
