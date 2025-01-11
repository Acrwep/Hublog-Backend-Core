using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.Manual_Time
{
    public class GetManualList
    {
        public int UserId { get; set; }
        public int OrganizationId { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Summary { get; set; }
        public string? FileName { get; set; }
        public byte[]? Attachment { get; set; }
    }
}
