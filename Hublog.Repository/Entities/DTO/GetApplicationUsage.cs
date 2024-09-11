using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.DTO
{
    public class GetApplicationUsage
    {
        public string ApplicationName { get; set; }
        public int UserCount { get; set; }  
        public string TotalUsage { get; set; }
        public DateTime UsageDate { get; set; }
    }
}
