namespace Hublog.Repository.Entities.Model.Attendance
{
    public class InOutLogs
    {
        public string Full_Name { get; set; }
        public string Team_Name { get; set; }
        public DateTime Start_Time { get; set; }
        public DateTime End_Time { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string DayStatus { get; set; }
    }
}
