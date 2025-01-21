namespace Hublog.Repository.Entities.Model.DashboardModel
{
    public class AllAttendanceSummary
    {
        public DateTime AttendanceDate { get; set; }
        public string Full_Name { get; set; }   
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public float AttendancePercentage { get; set; }
        public string? TotalWorkingTime { get; set; }
        public string AverageWorkingTime { get; set; }
    }
}
