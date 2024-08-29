namespace Hublog.Repository.Entities.Model
{
    public class AttendanceReport
    {
        public string Employee { get; set; }
        public DateTime In { get; set; }
        public DateTime Out { get; set; }
        public DateTime TotalTime { get; set; }
        public DateTime AttendanceDate { get; set; }    
    }
}
