namespace Hublog.Repository.Entities.Model.Attendance
{
    public class AttedndanceLog
    {
        public int UserId { get; set; } 
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Total_Time { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string DayStatus { get; set; }
    }
}
