namespace Hublog.Repository.Entities.Model.Attendance
{
    public class AttendanceReport
    {
        public int Id { get; set; } 
        public string Full_Name { get; set; }   
        public string Team_Name { get; set; }
        public DateTime InTime { get; set; }
        public DateTime Out { get; set; }
        public DateTime TotalTime { get; set; }
        public DateTime BreakDuration { get; set; }
        public DateTime IdleDuration { get; set; }
        public DateTime ActiveTime { get; set; }
        public DateTime OnlineTime { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string Remark { get; set; }
    }
}
