using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hublog.Repository.Entities.Model.Attendance
{
    public class LateAttendanceModel
    {
        public string FullName { get; set; }
        public TimeSpan StartTime { get; set; }
    }
}
