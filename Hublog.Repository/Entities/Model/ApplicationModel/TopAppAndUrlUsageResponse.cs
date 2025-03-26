using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.ApplicationModel
{
    public class TopAppAndUrlUsageResponse
    {
        public string Url { get; set; }
        public string UrlMaxUsage { get; set; }
        public string ApplicationName { get; set; }
        public string AppMaxUsage { get; set; }
    }
}
