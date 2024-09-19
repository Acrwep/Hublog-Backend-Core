namespace Hublog.Repository.Entities.Model.DashboardModel
{
    public class AllAttendanceSummary
    {
        public DateTime AttendanceDate { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public float AttendancePercentage { get; set; }
        public string AverageWorkingTime { get; set; }
    }
}
