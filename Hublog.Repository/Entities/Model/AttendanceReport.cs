namespace Hublog.Repository.Entities.Model
{
    public class AttendanceReport
    {
        public string First_Name { get; set; }
        public string Last_Name { get; set; }   
        public DateTime InTime { get; set; }    
        public DateTime Out { get; set; }
        public DateTime TotalTime { get; set; }
        public DateTime AttendanceDate { get; set; }    
    }
}
