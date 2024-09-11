using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.UrlModel
{
    public class UrlUsage
    {
        public int UserId { get; set; }
        public string Url { get; set; }
        public string TotalUsage { get; set; }
        public string Details { get; set; }
        public DateTime UsageDate { get; set; }
    }
}
