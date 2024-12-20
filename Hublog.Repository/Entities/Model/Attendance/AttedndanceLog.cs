namespace Hublog.Repository.Entities.Model.Attendance
{
    public class AttedndanceLog
    {
        public int UserId { get; set; } 
        public string Full_Name { get; set; }   
        public string Team_Name { get; set; }
        public string Total_Time { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string DayStatus { get; set; }
        public int PresentCount { get; set; }
    }
}
