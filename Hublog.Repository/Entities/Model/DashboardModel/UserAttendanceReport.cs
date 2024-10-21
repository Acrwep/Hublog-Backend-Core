namespace Hublog.Repository.Entities.Model.DashboardModel
{
    public class UserAttendanceReport
    {
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string WorkingTime { get; set; }
        public string BreakTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Attendance { get; set; }
        public int AbsentCount { get; set; }
    }
}
