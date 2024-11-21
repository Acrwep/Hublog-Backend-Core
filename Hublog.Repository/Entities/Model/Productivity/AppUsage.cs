using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.Productivity
{
    public class AppUsage
    {
        public int UserId { get; set; }
        public string ApplicationName { get; set; }
        public string Details { get; set; }
        public int Duration { get; set; } 
        public DateTime UsageDate { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ProductivityName { get; set; }
        public  string TotalUsage { get; set; }
        public int TotalSeconds { get; set; }


    }
}
