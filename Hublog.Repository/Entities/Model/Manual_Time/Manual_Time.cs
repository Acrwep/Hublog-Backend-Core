using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.Manual_Time
{
    public class Manual_Time
    {
        public int OrganizationId { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Summary { get; set; }
        public string? Attachment { get; set; } 
    }
}
