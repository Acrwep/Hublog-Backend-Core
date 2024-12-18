using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.Activity
{
    public class User_Activity
    {
        public int UserId { get; set; }
        public string? TeamName { get; set; }
        public string FullName { get; set; }
        public int? AttendanceCount { get; set; }
        public long? TodalTime { get; set; }
        public long? BreakDuration { get; set; }
        public long? IdleDuration { get; set; }
        public long? ActiveTime { get; set; }
        public long? OnlineTime { get; set; }
    }
}
