using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.Goals
{
    public class UserMeetingWorkingTime
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string TotalTime { get; set; } // Since Total_Time is stored as varchar
    }
}
