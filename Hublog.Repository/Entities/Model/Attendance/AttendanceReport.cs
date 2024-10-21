namespace Hublog.Repository.Entities.Model.Attendance
{
    public class AttendanceReport
    {
        public int Id { get; set; } 
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Full_Name { get; set; }       
        public DateTime InTime { get; set; }
        public DateTime Out { get; set; }
        public DateTime TotalTime { get; set; }
        public DateTime AttendanceDate { get; set; }
    }
}
