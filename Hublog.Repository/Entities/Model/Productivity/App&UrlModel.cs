using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.Productivity
{
    public class App_UrlModel
    {
        public string UsageType { get; set; }
        public string Name { get; set; }
        public int TotalSeconds { get; set; }
        public string TotalUsage { get; internal set; }
      
    }
}
